using SafeStepDesktop.Models;
using SafeStepDesktop.Services;
using System.IO.Ports;
using System.Media;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Globalization;

namespace SafeStep___Desktop__WinForms_
{
    public partial class Form1 : Form
    {
        private SerialPort? _serialPort;
        private readonly int[] _baudRates = { 9600, 19200, 38400, 57600, 115200 };
        private List<double> _distanceValues = new() { 0 };
        private string _serialBuffer = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbBaudRate.Items.Clear();
            cmbBaudRate.Items.AddRange(_baudRates.Cast<object>().ToArray());
            cmbBaudRate.SelectedItem = 115200;

            RefreshPorts();

            lblStatus.Text = "Disconnected";
            btnDisconnect.Enabled = false;
            btnConnect.Enabled = true;

            lblAlarm.Text = "No active alarms";
            lblDistance.Text = "0.0 m";
            lblDistanceTime.Text = "Last updated: --:--";
            lblBattery.Text = "0%";
            progressBattery.Value = 0;

            distanceChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = _distanceValues,
                    Fill = null
                }
            };
           
        }

        private void RefreshPorts()
        {
            cmbPorts.Items.Clear();
            var ports = SerialPort.GetPortNames();
            cmbPorts.Items.AddRange(ports);

            if (cmbPorts.Items.Count > 0)
                cmbPorts.SelectedIndex = 0;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshPorts();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (cmbPorts.SelectedItem == null || cmbBaudRate.SelectedItem == null)
            {
                MessageBox.Show("Please select a COM port and baud rate.");
                return;
            }

            try
            {
                _serialPort = new SerialPort
                {
                    PortName = cmbPorts.SelectedItem.ToString(),
                    BaudRate = Convert.ToInt32(cmbBaudRate.SelectedItem),
                    DataBits = 8,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    ReadTimeout = 500,
                    NewLine = "\r"
                };

                _serialPort.DataReceived += SerialPort_DataReceived;
                _serialPort.Open();

                lblStatus.Text = $"Connected to {_serialPort.PortName} @ {_serialPort.BaudRate} baud";
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed: " + ex.Message);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (_serialPort?.IsOpen == true)
                {
                    _serialPort.Close();
                    _serialPort.Dispose();
                }
            }
            catch
            {
            }

            lblStatus.Text = "Disconnected";
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
        }



        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (_serialPort == null || !_serialPort.IsOpen) return;

                string incoming = _serialPort.ReadExisting();
                if (string.IsNullOrEmpty(incoming)) return;

                _serialBuffer += incoming;

                string normalized = _serialBuffer.Replace("\r\n", "\n").Replace("\r", "\n");
                string[] parts = normalized.Split('\n');

                for (int i = 0; i < parts.Length - 1; i++)
                {
                    string line = parts[i].Trim();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        Invoke(new Action(() =>
                        {
                            lstMessages.Items.Insert(0, "LINE >>> " + line);
                            ProcessIncomingData(line);
                        }));
                    }
                }

                _serialBuffer = parts[^1];
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    lstMessages.Items.Insert(0, "SERIAL ERROR: " + ex.Message);
                }));
            }
        }

        private void ProcessIncomingData(string rawData)
        {
            var msg = MessageParser.Parse(rawData);
            if (msg == null)
            {
                lstMessages.Items.Insert(0, "PARSE FAILED FOR: " + rawData);
                return;
            }

            lstMessages.Items.Insert(0, $"{msg.Timestamp:HH:mm:ss} [{msg.Type}] {msg.Value}");

            switch (msg.Type)
            {
                case MessageType.Alarm:
                    TriggerAlarmVisual(msg.Value);
                    break;

                case MessageType.Distance:
                    if (double.TryParse(msg.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double meters))
                        UpdateDistanceDisplay(meters);
                    break;

                case MessageType.Message:
                    break;

                case MessageType.Battery:
                    if (int.TryParse(msg.Value, out int percent))
                        UpdateBatteryLevel(percent);
                    break;

                case MessageType.Heartbeat:
                    UpdateHeartbeat();
                    break;
            }
        }

        private void TriggerAlarmVisual(string alarmType)
        {
            try
            {
                string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "alarm.wav");

                if (File.Exists(soundPath))
                {
                    using SoundPlayer player = new SoundPlayer(soundPath);
                    player.Load();
                    player.Play();
                }
                else
                {
                    MessageBox.Show("alarm.wav not found at: " + soundPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sound error: " + ex.Message);
            }

            string alarmText = alarmType.ToUpper() switch
            {
                "FALL" => "FALL DETECTED!",
                "SOS" => "SOS ALERT!",
                _ => alarmType
            };

            pnlAlarm.BackColor = Color.Red;
            lblAlarm.Text = "ALARM: " + alarmText;

            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 5000;
            timer.Tick += (s, e) =>
            {
                pnlAlarm.BackColor = Color.LightGreen;
                lblAlarm.Text = "No active alarms";
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }
        private void UpdateDistanceDisplay(double meters)
        {
            _distanceValues.Add(meters);

            if (_distanceValues.Count > 20)
                _distanceValues.RemoveAt(0);

            distanceChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = _distanceValues,
                    Fill = null
                }
            };

            lblDistance.Text = meters.ToString("F1") + " m";
            lblDistanceTime.Text = "Last updated: " + DateTime.Now.ToString("HH:mm:ss");
        }

        private void UpdateBatteryLevel(int percent)
        {
            percent = Math.Max(0, Math.Min(100, percent));
            progressBattery.Value = percent;
            lblBattery.Text = percent + "%";
        }

        private void UpdateHeartbeat()
        {
            lblStatus.Text = "Heartbeat OK - " + DateTime.Now.ToString("HH:mm:ss");
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }
    }
}