namespace SafeStepDesktop.Models
{
    /// <summary>
    /// Represents one parsed message received from the SafeStep dongle over serial (COM port).
    /// Each message contains data about one wristband tag — its identity, battery, zone warnings, and signal.
    /// This model is filled by MessageParser.Parse() and then used by Form1 to update the UI.
    /// </summary>
    public class SafeStepMessage
    {
        /// <summary>
        /// The unique hardware ID of the wristband tag (e.g. "TAG001").
        /// This is burned into the firmware by Alejandro and never changes.
        /// Used to identify which patient/tag this message belongs to.
        /// </summary>
        public string TagId { get; set; } = "";

        /// <summary>
        /// The human-readable display name of the patient wearing the tag (e.g. "John").
        /// Configurable in the firmware. Shown in the UI instead of the raw TagId.
        /// </summary>
        public string TagName { get; set; } = "";

        /// <summary>
        /// Battery percentage of the wristband tag (0–100).
        /// Used to update the battery progress bar and label in the UI.
        /// </summary>
        public int Battery { get; set; }

        /// <summary>
        /// Zone 1 warning flag — true means the tag is 3–5 meters from the base station.
        /// Triggers a yellow warning indicator in the UI.
        /// </summary>
        public bool Zone1 { get; set; }

        /// <summary>
        /// Zone 2 warning flag — true means the tag is 5–10 meters from the base station.
        /// Triggers an orange warning indicator + beep sound in the UI.
        /// </summary>
        public bool Zone2 { get; set; }

        /// <summary>
        /// Zone 3 warning flag — true means the tag is more than 10 meters away or signal is lost.
        /// This is the most critical zone — triggers a red alarm + loud alarm sound in the UI.
        /// </summary>
        public bool Zone3 { get; set; }

        /// <summary>
        /// Raw RSSI (signal strength) value in dBm (e.g. "-62").
        /// Provided by the dongle as an optional field for distance estimation.
        /// Not always present — stored as string because it's optional and display-only.
        /// </summary>
        public string Rssi { get; set; } = "";

        /// <summary>
        /// Heartbeat flag — true means the tag is alive and actively broadcasting.
        /// False means the tag has gone silent (lost, out of range, or turned off).
        /// Used to show a "Tag Lost" warning in the UI.
        /// </summary>
        public bool IsAlive { get; set; } = true;

        /// <summary>
        /// The time this message was received and parsed by the C# app.
        /// Set to DateTime.Now at parse time — not sent by the dongle.
        /// Used to show "Last updated: HH:mm:ss" in the UI.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}