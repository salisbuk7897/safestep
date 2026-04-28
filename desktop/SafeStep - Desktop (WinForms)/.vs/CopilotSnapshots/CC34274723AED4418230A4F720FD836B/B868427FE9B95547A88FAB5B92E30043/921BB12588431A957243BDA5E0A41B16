using System.IO.Ports;
using System.Linq;
using System.Text.Json;
using Windows.Devices.Bluetooth.Advertisement;

namespace SafeStep___Desktop__WinForms_
{
    /// <summary>
    /// The Options window for the SafeStep desktop application.
    ///
    /// Responsibilities:
    ///   - Let the user select up to 4 COM ports for dongle connections
    ///   - Let the user select the baud rate (default 115200)
    ///   - Scan for nearby BLE devices using the Windows BLE watcher
    ///   - Let the user filter, pair, and assign friendly names to BLE devices
    ///   - Save the configured options for use by the main Form1 window
    ///
    /// This form is opened from Form1 as a modal dialog (ShowDialog),
    /// meaning the main window is blocked until this one is closed.
    /// </summary>
    public partial class Options : Form
    {
        /// <summary>
        /// The list of baud rates available in the dropdown.
        /// 115200 is pre-selected as it matches the SafeStep dongle's default speed.
        /// </summary>
        private readonly int[] _baudRates = { 9600, 19200, 38400, 57600, 115200 };

        /// <summary>
        /// In-memory list of all BLE devices detected during the current scan session.
        /// Each entry holds the device's hardware address (TagId), its broadcast name,
        /// an optional friendly name assigned by the user, and its paired status.
        /// This list is cleared and rebuilt every time the user clicks Refresh.
        /// </summary>
        private readonly List<DetectedDevice> _devices = new();

        /// <summary>
        /// The Windows BLE advertisement watcher.
        /// Runs in the background and fires Watcher_Received every time a
        /// nearby BLE device broadcasts an advertisement packet.
        /// Null when no scan is active (before first scan or after stop).
        /// </summary>
        private BluetoothLEAdvertisementWatcher? _watcher;

        /// <summary>
        /// File used to persist paired BLE devices across app restarts.
        /// Stored under the current user's LocalAppData.
        /// </summary>
        private readonly string _pairedDevicesFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SafeStepDesktop",
            "paired-devices.json");

        /// <summary>
        /// Constructor — initializes the form and wires up all event handlers manually.
        /// Event handlers for btnRefreshDevices, lstDevices, and btnPairDevice are
        /// attached here in code rather than the designer for clarity.
        /// Then calls LoadOptionsData() to populate all controls with initial values.
        /// </summary>
        public Options()
        {
            InitializeComponent();

            // Wire up event handlers manually (not through the designer)
            btnRefreshDevices.Click += btnRefreshDevices_Click;
            lstDevices.SelectedIndexChanged += lstDevices_SelectedIndexChanged;
            btnPairDevice.Click += btnPairDevice_Click;

            // Populate all controls with their default/initial values
            LoadOptionsData();
        }

        /// <summary>
        /// Initializes all controls in the Options window to their default state.
        /// Called once when the form opens.
        ///
        /// Order of operations:
        ///   1. Load available COM ports into all 4 port dropdowns
        ///   2. Load baud rates into the baud rate dropdown
        ///   3. Reset filter and auto-connect to defaults
        ///   4. Make TagId and DetectedName fields read-only (display only)
        ///   5. Clear the device list and start a fresh BLE scan
        /// </summary>
        private void LoadOptionsData()
        {
            LoadPorts();
            LoadBaudRates();

            // Default filter to empty (show all devices) and auto-connect to off
            txtDeviceFilter.Text = "";
            chkAutoConnect.Checked = false;

            // These two fields are for display only — the user cannot edit them
            txtTagId.ReadOnly = true;
            txtDetectedDeviceName.ReadOnly = true;

            // Start fresh — clear any previously detected devices and begin scanning
            _devices.Clear();
            LoadPairedDevices();
            RefreshDeviceList();
            StartBleScan();

            chkAutoConnect.Checked = Properties.Settings.Default.AutoConnect;

            // Load Firebase connection mode and credentials
            string mode = Properties.Settings.Default.ConnectionMode;
            rdoCloud.Checked = mode == "Cloud";
            rdoLocal.Checked = mode != "Cloud";

            txtFirebaseUrl.Text = Properties.Settings.Default.FirebaseUrl;
            txtFirebaseApiKey.Text = Properties.Settings.Default.FirebaseApiKey;

            UpdateFirebaseFieldsVisibility();
        }

        private void UpdateFirebaseFieldsVisibility()
        {
            // Firebase URL and API key are needed in both modes:
            // - Local mode: pushes tag data up to Firebase
            // - Cloud mode: polls tag data from Firebase
            lblFirebaseUrl.Visible = true;
            txtFirebaseUrl.Visible = true;
            lblFirebaseApiKey.Visible = true;
            txtFirebaseApiKey.Visible = true;
        }

        private void rdoConnectionMode_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFirebaseFieldsVisibility();
        }

        /// <summary>
        /// Scans for available COM ports on this PC and populates all 4 port dropdowns.
        /// Uses SerialPort.GetPortNames() which returns the currently available ports
        /// (e.g. "COM3", "COM4") at the moment of the call.
        ///
        /// All 4 dropdowns get the same list — the user picks which port to assign
        /// to each of the 4 possible dongle slots.
        /// Auto-selects the first available port in each dropdown if any exist.
        /// </summary>
        private void LoadPorts()
        {
            var ports = SerialPort.GetPortNames();

            // Clear all dropdowns before repopulating to avoid duplicate entries
            cmbPort1.Items.Clear();
            cmbPort2.Items.Clear();
            cmbPort3.Items.Clear();
            cmbPort4.Items.Clear();

            // Add the same port list to all 4 dropdowns
            cmbPort1.Items.AddRange(ports);
            cmbPort2.Items.AddRange(ports);
            cmbPort3.Items.AddRange(ports);
            cmbPort4.Items.AddRange(ports);

            // Auto-select the first port in each dropdown if any ports are available
            if (cmbPort1.Items.Count > 0) cmbPort1.SelectedIndex = 0;
            if (cmbPort2.Items.Count > 0) cmbPort2.SelectedIndex = 0;
            if (cmbPort3.Items.Count > 0) cmbPort3.SelectedIndex = 0;
            if (cmbPort4.Items.Count > 0) cmbPort4.SelectedIndex = 0;
        }

        /// <summary>
        /// Populates the baud rate dropdown with the available rates
        /// and pre-selects 115200, which is the SafeStep dongle's default baud rate.
        /// </summary>
        private void LoadBaudRates()
        {
            cmbOptionsBaudRate.Items.Clear();
            // Cast to object[] because Items.AddRange expects object[]
            cmbOptionsBaudRate.Items.AddRange(_baudRates.Cast<object>().ToArray());
            cmbOptionsBaudRate.SelectedItem = 115200;
        }

        /// <summary>
        /// Starts a new BLE advertisement scan using the Windows BLE watcher.
        /// Stops any existing scan first to avoid running two watchers at once.
        ///
        /// ScanningMode.Active means the watcher sends scan request packets to
        /// nearby devices asking them to respond with more data — this gives us
        /// the device's LocalName which is needed to identify SafeStep tags.
        ///
        /// The Watcher_Received event fires on a background thread every time
        /// a BLE advertisement is detected nearby.
        /// </summary>
        private void StartBleScan()
        {
            try
            {
                // Always stop the previous watcher before creating a new one
                StopBleScan();

                _watcher = new BluetoothLEAdvertisementWatcher
                {
                    // Active scanning requests the device's full advertisement data
                    // including its LocalName — needed to identify SafeStep tags
                    ScanningMode = BluetoothLEScanningMode.Active
                };

                _watcher.Received += Watcher_Received;
                _watcher.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to start BLE scan: " + ex.Message);
            }
        }

        /// <summary>
        /// Stops the active BLE scan and cleans up the watcher.
        /// Always unsubscribes the event handler before stopping to prevent
        /// ghost events firing after the watcher is disposed.
        /// Sets _watcher to null so we know no scan is running.
        /// </summary>
        private void StopBleScan()
        {
            if (_watcher != null)
            {
                // Unsubscribe first — prevents Received from firing after Stop()
                _watcher.Received -= Watcher_Received;
                _watcher.Stop();
                _watcher = null;
            }
        }

        /// <summary>
        /// Fires on a background thread every time the BLE watcher detects a nearby device.
        /// 
        /// Logic:
        ///   1. Get the device's broadcast name (LocalName) — skip if empty
        ///   2. Apply the user's filter — skip if name doesn't match
        ///   3. Use the Bluetooth hardware address as a unique TagId
        ///   4. If we've seen this device before, update its name
        ///      If it's new, add it to the _devices list
        ///   5. Refresh the device list UI
        ///
        /// Uses BeginInvoke() to marshal all UI updates to the UI thread,
        /// because this event fires on a background thread and WinForms controls
        /// can only be safely updated from the UI thread.
        /// </summary>
        private void Watcher_Received(BluetoothLEAdvertisementWatcher sender,
            BluetoothLEAdvertisementReceivedEventArgs args)
        {
            // Marshal everything to the UI thread — BLE events fire on background threads
            BeginInvoke(new Action(() =>
            {
                // Get the device's broadcast name — skip devices with no name
                // (many BLE devices advertise anonymously; we only care about named ones)
                string detectedName = args.Advertisement.LocalName;
                if (string.IsNullOrWhiteSpace(detectedName))
                    return;

                // Apply the user's filter — if a filter is set, only show devices
                // whose name contains the filter text (case-insensitive)
                string filter = txtDeviceFilter.Text.Trim();
                if (!string.IsNullOrWhiteSpace(filter) &&
                    !detectedName.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    return;

                // Use the Bluetooth hardware address as a unique device ID
                // Formatted as hex (e.g. "A1B2C3D4E5F6") — this never changes for a device
                string tagId = args.BluetoothAddress.ToString("X");

                // Check if we've already seen this device in the current scan session
                var existingDevice = _devices.FirstOrDefault(d => d.TagId == tagId);

                if (existingDevice != null)
                {
                    // Device already in list — just update its broadcast name in case it changed
                    existingDevice.DetectedName = detectedName;
                }
                else
                {
                    // New device — add it to the list with empty friendly name and unpaired status
                    _devices.Add(new DetectedDevice
                    {
                        TagId = tagId,
                        DetectedName = detectedName,
                        FriendlyName = "",
                        IsPaired = false
                    });
                }

                // Rebuild the UI list to show the updated device list
                RefreshDeviceList();
            }));
        }

        /// <summary>
        /// Rebuilds the lstDevices list box from the current _devices collection.
        /// Called after every scan event and after pairing/mapping changes.
        ///
        /// Display format:
        ///   - Unpaired device: "A1B2C3D4E5F6 - SafeStep-WB"
        ///   - Paired device:   "A1B2C3D4E5F6 - John's Wristband" (uses friendly name)
        ///
        /// Preserves the currently selected index so selection doesn't jump
        /// around as new devices are detected during an active scan.
        /// </summary>
        private void RefreshDeviceList()
        {
            // Remember which item was selected so we can restore it after rebuilding
            int selectedIndex = lstDevices.SelectedIndex;

            lstDevices.Items.Clear();

            foreach (var device in _devices)
            {
                // Show friendly name if assigned, otherwise show the raw detected name
                string displayText = string.IsNullOrWhiteSpace(device.FriendlyName)
                    ? $"{device.TagId} - {device.DetectedName}"
                    : $"{device.TagId} - {device.FriendlyName}";

                lstDevices.Items.Add(displayText);
            }

            // If the list is empty, clear the detail fields and stop here
            if (lstDevices.Items.Count == 0)
            {
                txtTagId.Text = "";
                txtDetectedDeviceName.Text = "";
                txtFriendlyName.Text = "";
                return;
            }

            // Restore the previous selection if it's still valid
            // Otherwise default to selecting the first item
            if (selectedIndex >= 0 && selectedIndex < lstDevices.Items.Count)
                lstDevices.SelectedIndex = selectedIndex;
            else if (lstDevices.SelectedIndex == -1)
                lstDevices.SelectedIndex = 0;
        }

        /// <summary>
        /// Fires when the user selects a different device in the list.
        /// Updates the three detail fields (TagId, DetectedName, FriendlyName)
        /// to show the selected device's current values.
        /// FriendlyName is editable so the user can type a new name before pairing.
        /// </summary>
        private void lstDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lstDevices.SelectedIndex;

            // Guard against invalid index (can happen during list rebuild)
            if (index < 0 || index >= _devices.Count) return;

            var selectedDevice = _devices[index];

            // Populate the detail fields with the selected device's data
            txtTagId.Text = selectedDevice.TagId;
            txtDetectedDeviceName.Text = selectedDevice.DetectedName;
            txtFriendlyName.Text = selectedDevice.FriendlyName;
        }

        /// <summary>
        /// Handles the Refresh Devices button click.
        /// Clears the current device list and starts a fresh BLE scan.
        /// Useful if the target device wasn't advertising when the form first opened,
        /// or if the user wants to clear stale devices from a previous scan.
        /// </summary>
        private void btnRefreshDevices_Click(object sender, EventArgs e)
        {
            // Clear the in-memory list and rebuild the empty UI list
            _devices.Clear();
            LoadPairedDevices();
            RefreshDeviceList();

            // Start a new scan from scratch
            StartBleScan();

            MessageBox.Show("BLE device scan refreshed.");
        }

        /// <summary>
        /// Handles the Pair Device button click.
        /// Saves the user-entered friendly name to the selected device
        /// and marks it as paired (IsPaired = true).
        ///
        /// Validates that:
        ///   - A device is actually selected in the list
        ///   - The user has entered a non-empty friendly name
        ///
        /// Pairing here is "soft pairing" — it just assigns a name and marks the device.
        /// No OS-level Bluetooth pairing is performed.
        /// </summary>
        private void btnPairDevice_Click(object sender, EventArgs e)
        {
            int index = lstDevices.SelectedIndex;

            // Validate that a device is selected
            if (index < 0 || index >= _devices.Count)
            {
                MessageBox.Show("Please select a device from the list.");
                return;
            }

            // Validate that the user has entered a friendly name
            if (string.IsNullOrWhiteSpace(txtFriendlyName.Text))
            {
                MessageBox.Show("Please enter a friendly name before pairing.");
                return;
            }

            // Save the friendly name and mark the device as paired
            _devices[index].FriendlyName = txtFriendlyName.Text.Trim();
            _devices[index].IsPaired = true;
            SavePairedDevices();

            // Refresh the list so the friendly name appears immediately
            RefreshDeviceList();
            lstDevices.SelectedIndex = index;

            MessageBox.Show($"Device paired: {_devices[index].TagId} -> {_devices[index].FriendlyName}");
        }

        /// <summary>
        /// Handles the Save Mapping button click.
        /// Updates the friendly name of the selected device without
        /// marking it as fully paired — useful for renaming an already-paired device
        /// or saving a name change without re-pairing.
        /// </summary>
        private void btnSaveMapping_Click(object sender, EventArgs e)
        {
            int index = lstDevices.SelectedIndex;

            // Validate that a device is selected
            if (index < 0 || index >= _devices.Count)
            {
                MessageBox.Show("Please select a device from the list.");
                return;
            }

            // Validate that a friendly name has been entered
            if (string.IsNullOrWhiteSpace(txtFriendlyName.Text))
            {
                MessageBox.Show("Please enter a friendly name.");
                return;
            }

            // Update only the friendly name — IsPaired status is unchanged
            _devices[index].FriendlyName = txtFriendlyName.Text.Trim();

            // Persist if this device is already paired
            if (_devices[index].IsPaired)
                SavePairedDevices();

            // Refresh the list so the new name appears immediately
            RefreshDeviceList();
            lstDevices.SelectedIndex = index;

            MessageBox.Show($"Mapping saved: {_devices[index].TagId} -> {_devices[index].FriendlyName}");
        }

        /// <summary>
        /// Handles the Save Options button click.
        /// Reads the current values from all option controls and displays a
        /// confirmation summary to the user.
        ///
        /// Note: Currently shows a MessageBox summary only.
        /// In a future version, these values should be saved to a config file
        /// or passed back to Form1 so they actually affect the connection behavior.
        /// </summary>
        private void btnSaveOptions_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoConnect = chkAutoConnect.Checked;
            Properties.Settings.Default.ConnectionMode = rdoCloud.Checked ? "Cloud" : "Local";
            Properties.Settings.Default.FirebaseUrl = txtFirebaseUrl.Text.Trim();
            Properties.Settings.Default.FirebaseApiKey = txtFirebaseApiKey.Text.Trim();
            Properties.Settings.Default.Save();
            MessageBox.Show("Settings saved!");
        }

        /// <summary>
        /// Handles the Close button click.
        /// Stops the BLE scan before closing to release the Bluetooth adapter
        /// and prevent background scan threads from running after the window closes.
        /// </summary>
        private void btnCloseOptions_Click(object sender, EventArgs e)
        {
            StopBleScan();
            Close();
        }

        /// <summary>
        /// Override of the built-in form closing event.
        /// Ensures the BLE scan is always stopped when the window closes —
        /// even if the user closes it via the X button instead of the Close button.
        /// Always calls base.OnFormClosing() so the normal close logic still runs.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopBleScan();
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Private inner class representing one BLE device detected during scanning.
        /// Stored in the _devices list and displayed in the lstDevices list box.
        ///
        /// TagId         — the device's unique Bluetooth hardware address (hex string)
        /// DetectedName  — the device's own broadcast name from its advertisement
        /// FriendlyName  — a user-assigned display name (e.g. "John's Wristband")
        /// IsPaired      — true if the user has clicked Pair Device for this device
        /// </summary>
        private class DetectedDevice
        {
            /// <summary>Unique Bluetooth hardware address, e.g. "A1B2C3D4E5F6"</summary>
            public string TagId { get; set; } = "";

            /// <summary>Name broadcast by the device itself, e.g. "SafeStep-WB"</summary>
            public string DetectedName { get; set; } = "";

            /// <summary>User-assigned name shown in the UI after pairing, e.g. "John's Wristband"</summary>
            public string FriendlyName { get; set; } = "";

            /// <summary>True if the user has paired this device via the Pair Device button</summary>
            public bool IsPaired { get; set; }
        }

        /// <summary>
        /// Loads previously paired BLE devices from local storage.
        /// </summary>
        private void LoadPairedDevices()
        {
            try
            {
                if (!File.Exists(_pairedDevicesFilePath))
                    return;

                string json = File.ReadAllText(_pairedDevicesFilePath);
                if (string.IsNullOrWhiteSpace(json))
                    return;

                var stored = JsonSerializer.Deserialize<List<PairedDeviceStoreItem>>(json);
                if (stored == null || stored.Count == 0)
                    return;

                foreach (var item in stored)
                {
                    if (string.IsNullOrWhiteSpace(item.TagId))
                        continue;

                    if (_devices.Any(d => d.TagId == item.TagId))
                        continue;

                    _devices.Add(new DetectedDevice
                    {
                        TagId = item.TagId,
                        DetectedName = item.DetectedName ?? string.Empty,
                        FriendlyName = item.FriendlyName ?? string.Empty,
                        IsPaired = true
                    });
                }
            }
            catch
            {
                // Ignore persistence errors and continue without stored devices.
            }
        }

        /// <summary>
        /// Saves currently paired BLE devices to local storage.
        /// </summary>
        private void SavePairedDevices()
        {
            try
            {
                string? directory = Path.GetDirectoryName(_pairedDevicesFilePath);
                if (!string.IsNullOrWhiteSpace(directory))
                    Directory.CreateDirectory(directory);

                var toStore = _devices
                    .Where(d => d.IsPaired)
                    .Select(d => new PairedDeviceStoreItem
                    {
                        TagId = d.TagId,
                        DetectedName = d.DetectedName,
                        FriendlyName = d.FriendlyName
                    })
                    .ToList();

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(toStore, options);
                File.WriteAllText(_pairedDevicesFilePath, json);
            }
            catch
            {
                // Ignore persistence errors to avoid blocking pairing flow.
            }
        }

        /// <summary>
        /// Storage model for paired BLE devices.
        /// </summary>
        private class PairedDeviceStoreItem
        {
            public string TagId { get; set; } = string.Empty;
            public string DetectedName { get; set; } = string.Empty;
            public string FriendlyName { get; set; } = string.Empty;
        }
    }
}