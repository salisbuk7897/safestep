using SafeStepDesktop.Models;
using System.Text;
using System.Text.Json;

namespace SafeStepDesktop.Services
{
    /// <summary>
    /// Handles communication with Firebase Realtime Database.
    ///
    /// Two responsibilities:
    ///   - Push: called when app is in Local mode — uploads latest tag data so remote viewers can see it.
    ///   - Poll: called when app is in Cloud mode — periodically reads tag data uploaded by another PC.
    ///
    /// Firebase Realtime Database REST API format:
    ///   PUT  https://<db>.firebaseio.com/safestep/tags/<tagId>.json?auth=<apiKey>
    ///   GET  https://<db>.firebaseio.com/safestep/tags.json?auth=<apiKey>
    /// </summary>
    public class FirebaseService
    {
        private readonly HttpClient _httpClient = new();

        private string _databaseUrl = "";
        private string _apiKey = "";

        /// <summary>
        /// True when both a database URL and API key have been configured.
        /// </summary>
        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(_databaseUrl) &&
            !string.IsNullOrWhiteSpace(_apiKey);

        /// <summary>
        /// Updates the Firebase URL and API key from the current app settings.
        /// Call this on startup and whenever the user saves new settings.
        /// </summary>
        public void Configure(string databaseUrl, string apiKey)
        {
            _databaseUrl = databaseUrl.TrimEnd('/');
            _apiKey = apiKey;
        }

        /// <summary>
        /// Pushes a SafeStepMessage to Firebase under safestep/tags/<tagId>.
        /// Called in Local mode after each valid telemetry message is received.
        /// </summary>
        /// <summary>Returns null on success, or an error message string on failure.</summary>
        public async Task<string?> PushTagDataAsync(SafeStepMessage message)
        {
            if (!IsConfigured) return "Firebase not configured — check URL and API key in Options.";

            try
            {
                string url = $"{_databaseUrl}/safestep/tags/{message.TagId}.json?auth={_apiKey}";

                var payload = new
                {
                    tagId = message.TagId,
                    tagName = message.TagName,
                    battery = message.Battery,
                    temperature = message.Temperature,
                    rssi = message.Rssi,
                    isAlive = message.IsAlive,
                    zone1 = message.Zone1,
                    zone2 = message.Zone2,
                    zone3 = message.Zone3,
                    timestampUtc = DateTime.UtcNow.ToString("O")
                };

                string json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(url, content);
                if (!response.IsSuccessStatusCode)
                    return $"Firebase push failed: {(int)response.StatusCode} {response.ReasonPhrase}";

                return null;
            }
            catch (Exception ex)
            {
                return $"Firebase push error: {ex.Message}";
            }
        }

        /// <summary>
        /// Reads all tag data from Firebase under safestep/tags.
        /// Called in Cloud mode on a polling interval to display remote tag state.
        /// Returns a list of SafeStepMessages parsed from the Firebase response,
        /// or an empty list if the request fails or no data exists.
        /// </summary>
        public async Task<List<SafeStepMessage>> PollTagDataAsync()
        {
            var results = new List<SafeStepMessage>();
            if (!IsConfigured) return results;

            try
            {
                string url = $"{_databaseUrl}/safestep/tags.json?auth={_apiKey}";
                string response = await _httpClient.GetStringAsync(url);

                if (string.IsNullOrWhiteSpace(response) || response == "null")
                    return results;

                // Firebase returns an object keyed by tagId:
                // { "TAG001": { tagId: ..., zone1: ..., ... }, "TAG002": { ... } }
                using var doc = JsonDocument.Parse(response);
                foreach (var entry in doc.RootElement.EnumerateObject())
                {
                    var el = entry.Value;
                    results.Add(new SafeStepMessage
                    {
                        TagId       = GetString(el, "tagId"),
                        TagName     = GetString(el, "tagName"),
                        Battery     = GetInt(el, "battery"),
                        Temperature = GetFloat(el, "temperature"),
                        Rssi        = GetString(el, "rssi"),
                        IsAlive     = GetBool(el, "isAlive"),
                        Zone1       = GetBool(el, "zone1"),
                        Zone2       = GetBool(el, "zone2"),
                        Zone3       = GetBool(el, "zone3"),
                        Timestamp   = DateTime.Now
                    });
                }
            }
            catch
            {
                // Polling failure is non-critical — UI will show stale data until next poll
            }

            return results;
        }

        private static string GetString(JsonElement el, string key) =>
            el.TryGetProperty(key, out var p) ? p.GetString() ?? "" : "";

        private static int GetInt(JsonElement el, string key) =>
            el.TryGetProperty(key, out var p) && p.TryGetInt32(out int v) ? v : 0;

        private static float GetFloat(JsonElement el, string key) =>
            el.TryGetProperty(key, out var p) && p.TryGetSingle(out float v) ? v : 0f;

        private static bool GetBool(JsonElement el, string key) =>
            el.TryGetProperty(key, out var p) && p.GetBoolean();
    }
}
