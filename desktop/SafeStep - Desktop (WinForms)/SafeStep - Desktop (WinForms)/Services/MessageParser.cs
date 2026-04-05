using SafeStepDesktop.Models;
namespace SafeStepDesktop.Services
{
    public static class MessageParser
    {
        public static SafeStepMessage
        Parse(string raw)
        {
            // Must start with $SAFE,
            if (!raw.StartsWith("SAFE,"))
                return null;
            // Split by commas
            var parts = raw.Trim()
            .Split(',');
            if (parts.Length < 4)
                return null;
            return new SafeStepMessage
            {
                Type = parts[1] switch
                {
                    "ALARM" =>
                    MessageType.Alarm,
                    "DIST" =>
                    MessageType.Distance,
                    "MSG" =>
                    MessageType.Message,
                    "BATT" =>
                    MessageType.Battery,
                    "HEARTBEAT" =>
                    MessageType.Heartbeat,
                    _ => MessageType.Unknown
                },
                Value = parts[2],
                Timestamp = DateTimeOffset
            .FromUnixTimeSeconds(
            long.Parse(parts[3]))
            .LocalDateTime
            };
        }
    }
}