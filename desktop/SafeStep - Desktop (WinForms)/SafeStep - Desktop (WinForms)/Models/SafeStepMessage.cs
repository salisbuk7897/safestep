namespace SafeStepDesktop.Models
{
    // All possible message types
    public enum MessageType
    {
        Alarm, // Fall, SOS
        Distance, // How far away
        Message, // Text message
        Battery, // Battery %
        Heartbeat, // Still alive
        Unknown // Anything else
    }
    // One message from the dongle
    public class SafeStepMessage
    {
        public MessageType Type { get; set; }
        public string Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}