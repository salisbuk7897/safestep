using SafeStepDesktop.Models;

namespace SafeStepDesktop.Services
{
    /// <summary>
    /// Static helper class responsible for parsing raw serial strings from the SafeStep dongle
    /// into structured SafeStepMessage objects that the UI can use.
    ///
    /// The dongle sends one line per tag, formatted like:
    ///   ID=TAG001;NAME=John;BAT=87;Z1=0;Z2=1;Z3=0;RSSI=-62;TEMP=36.5;PING=1
    ///
    /// Fields are separated by semicolons (;).
    /// Each field is a key=value pair separated by equals (=).
    /// The line ends with \r\n which SerialPort already strips before calling this parser.
    /// </summary>
    public static class MessageParser
    {
        /// <summary>
        /// Parses one raw line from the serial port into a SafeStepMessage.
        ///
        /// Steps:
        ///   1. Split the line by ';' to get individual key=value fields
        ///   2. Split each field by '=' to get the key and value
        ///   3. Store all pairs in a dictionary for easy lookup
        ///   4. Check that the required 'ID' field exists — if not, reject the message
        ///   5. Build and return a SafeStepMessage with all parsed values
        ///
        /// Returns null if the line is empty, malformed, or missing required fields.
        /// Form1.ProcessIncomingData() checks for null and logs a parse failure if it gets one.
        /// </summary>
        /// <param name="raw">One raw line received from the serial port, e.g. "ID=TAG001;NAME=John;BAT=87;Z1=0;Z2=1;Z3=0;RSSI=-62;TEMP=36.5;PING=1"</param>
        /// <returns>A populated SafeStepMessage, or null if parsing failed.</returns>
        public static SafeStepMessage? Parse(string raw)
        {
            // Reject empty or whitespace-only lines immediately
            // Also reject ESP32 bootloader garbage logs that appear on boot
            // Valid messages always start with "ID=" — anything else is ignored
            if (string.IsNullOrWhiteSpace(raw) || !raw.StartsWith("ID="))
                return null;

            // Step 1: Split the full line by ';' into individual key=value fields
            // Example: "ID=TAG001;NAME=John;BAT=87" → ["ID=TAG001", "NAME=John", "BAT=87"]
            var fields = raw.Trim().Split(';');

            // Step 2 & 3: Split each field by '=' and store in a dictionary
            // This lets us look up any field by name, e.g. dict["BAT"] = "87"
            // Fields with no '=' sign are silently skipped (malformed field)
            var dict = new Dictionary<string, string>();
            foreach (var field in fields)
            {
                var kv = field.Split('=');
                if (kv.Length == 2)
                    dict[kv[0].Trim()] = kv[1].Trim();
            }

            // Step 4: 'ID' is the only required field — every valid message must have one.
            // If it's missing, the message is malformed and we return null.
            if (!dict.ContainsKey("ID"))
                return null;

            // Step 5: Build the SafeStepMessage from the parsed dictionary.
            // GetValueOrDefault() safely returns a fallback if the field wasn't in the message.
            // This handles optional fields like RSSI, TEMP and PING gracefully.
            return new SafeStepMessage
            {
                // Required fields
                TagId = dict.GetValueOrDefault("ID", ""),
                TagName = dict.GetValueOrDefault("NAME", "Unknown"),

                // Battery: parse to int, default to 0 if missing or not a number
                Battery = int.TryParse(dict.GetValueOrDefault("BAT", "0"), out int bat) ? bat : 0,

                // Zones: "1" means warning active, anything else means OK
                Zone1 = dict.GetValueOrDefault("Z1", "0") == "1",
                Zone2 = dict.GetValueOrDefault("Z2", "0") == "1",
                Zone3 = dict.GetValueOrDefault("Z3", "0") == "1",

                // RSSI: signal strength, e.g. "-62"
                Rssi = dict.GetValueOrDefault("RSSI", ""),

                // Temperature: parse to float, default to 0.0 if missing or not a number
                // Uses InvariantCulture to handle decimal point correctly across all systems
                Temperature = float.TryParse(
                    dict.GetValueOrDefault("TEMP", "0"),
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out float temp) ? temp : 0f,

                // IsAlive: PING=1 means the tag is responding normally
                IsAlive = dict.GetValueOrDefault("PING", "1") == "1",

                // Timestamp is set by the C# app at parse time, not sent by the dongle
                Timestamp = DateTime.Now
            };
        }
    }
}