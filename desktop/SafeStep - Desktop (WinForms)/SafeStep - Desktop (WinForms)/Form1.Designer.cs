namespace SafeStep___Desktop__WinForms_
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            LiveChartsCore.SkiaSharpView.SKCharts.SKDefaultLegend skDefaultLegend1 = new LiveChartsCore.SkiaSharpView.SKCharts.SKDefaultLegend();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            LiveChartsCore.Drawing.Padding padding1 = new LiveChartsCore.Drawing.Padding();
            LiveChartsCore.SkiaSharpView.SKCharts.SKDefaultTooltip skDefaultTooltip1 = new LiveChartsCore.SkiaSharpView.SKCharts.SKDefaultTooltip();
            LiveChartsCore.Drawing.Padding padding2 = new LiveChartsCore.Drawing.Padding();
            comPortText = new Label();
            btnRefresh = new Button();
            btnConnect = new Button();
            btnDisconnect = new Button();
            flowLayoutPanel1 = new FlowLayoutPanel();
            lblAlarm = new Label();
            pnlAlarm = new Panel();
            flowLayoutPanel2 = new FlowLayoutPanel();
            lblDistance = new Label();
            lblDistanceTime = new Label();
            distanceChart = new LiveChartsCore.SkiaSharpView.WinForms.CartesianChart();
            flowLayoutPanel3 = new FlowLayoutPanel();
            lblBattery = new Label();
            progressBattery = new ProgressBar();
            cmbPorts = new ComboBox();
            cmbBaudRate = new ComboBox();
            statusStrip1 = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            lstMessages = new ListBox();
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            flowLayoutPanel3.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // comPortText
            // 
            comPortText.AutoSize = true;
            comPortText.Location = new Point(18, 22);
            comPortText.Name = "comPortText";
            comPortText.Size = new Size(66, 17);
            comPortText.TabIndex = 0;
            comPortText.Text = "COM Port";
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(220, 18);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(75, 27);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(445, 18);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(80, 27);
            btnConnect.TabIndex = 4;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Location = new Point(535, 18);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(92, 27);
            btnDisconnect.TabIndex = 5;
            btnDisconnect.Text = "Disconnect";
            btnDisconnect.UseVisualStyleBackColor = true;
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.BorderStyle = BorderStyle.FixedSingle;
            flowLayoutPanel1.Controls.Add(lblAlarm);
            flowLayoutPanel1.Controls.Add(pnlAlarm);
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(18, 66);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Padding = new Padding(10);
            flowLayoutPanel1.Size = new Size(270, 240);
            flowLayoutPanel1.TabIndex = 6;
            flowLayoutPanel1.WrapContents = false;
            // 
            // lblAlarm
            // 
            lblAlarm.AutoSize = true;
            lblAlarm.Location = new Point(13, 10);
            lblAlarm.Name = "lblAlarm";
            lblAlarm.Size = new Size(50, 17);
            lblAlarm.TabIndex = 0;
            lblAlarm.Text = "ALARM";
            // 
            // pnlAlarm
            // 
            pnlAlarm.BackColor = Color.LightGreen;
            pnlAlarm.Location = new Point(13, 30);
            pnlAlarm.Name = "pnlAlarm";
            pnlAlarm.Size = new Size(230, 180);
            pnlAlarm.TabIndex = 1;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.BorderStyle = BorderStyle.FixedSingle;
            flowLayoutPanel2.Controls.Add(lblDistance);
            flowLayoutPanel2.Controls.Add(lblDistanceTime);
            flowLayoutPanel2.Controls.Add(distanceChart);
            flowLayoutPanel2.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel2.Location = new Point(308, 66);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Padding = new Padding(10);
            flowLayoutPanel2.Size = new Size(520, 240);
            flowLayoutPanel2.TabIndex = 7;
            flowLayoutPanel2.WrapContents = false;
            // 
            // lblDistance
            // 
            lblDistance.AutoSize = true;
            lblDistance.Location = new Point(13, 10);
            lblDistance.Name = "lblDistance";
            lblDistance.Size = new Size(66, 17);
            lblDistance.TabIndex = 0;
            lblDistance.Text = "DISTANCE";
            // 
            // lblDistanceTime
            // 
            lblDistanceTime.AutoSize = true;
            lblDistanceTime.Location = new Point(13, 27);
            lblDistanceTime.Name = "lblDistanceTime";
            lblDistanceTime.Size = new Size(89, 17);
            lblDistanceTime.TabIndex = 1;
            lblDistanceTime.Text = "Distance Time";
            // 
            // distanceChart
            // 
            distanceChart.AutoUpdateEnabled = true;
            distanceChart.ChartTheme = null;
            skDefaultLegend1.AnimationsSpeed = TimeSpan.Parse("00:00:00.1500000");
            skDefaultLegend1.Content = null;
            skDefaultLegend1.IsValid = false;
            skDefaultLegend1.Opacity = 1F;
            padding1.Bottom = 0F;
            padding1.Left = 0F;
            padding1.Right = 0F;
            padding1.Top = 0F;
            skDefaultLegend1.Padding = padding1;
            skDefaultLegend1.RemoveOnCompleted = false;
            skDefaultLegend1.RotateTransform = 0F;
            skDefaultLegend1.X = 0F;
            skDefaultLegend1.Y = 0F;
            distanceChart.Legend = skDefaultLegend1;
            distanceChart.Location = new Point(13, 47);
            distanceChart.MatchAxesScreenDataRatio = false;
            distanceChart.Name = "distanceChart";
            distanceChart.Size = new Size(480, 170);
            distanceChart.TabIndex = 2;
            skDefaultTooltip1.AnimationsSpeed = TimeSpan.Parse("00:00:00.1500000");
            skDefaultTooltip1.Content = null;
            skDefaultTooltip1.IsValid = false;
            skDefaultTooltip1.Opacity = 1F;
            padding2.Bottom = 0F;
            padding2.Left = 0F;
            padding2.Right = 0F;
            padding2.Top = 0F;
            skDefaultTooltip1.Padding = padding2;
            skDefaultTooltip1.RemoveOnCompleted = false;
            skDefaultTooltip1.RotateTransform = 0F;
            skDefaultTooltip1.Wedge = 10;
            skDefaultTooltip1.X = 0F;
            skDefaultTooltip1.Y = 0F;
            distanceChart.Tooltip = skDefaultTooltip1;
            distanceChart.TooltipFindingStrategy = LiveChartsCore.Measure.TooltipFindingStrategy.Automatic;
            distanceChart.UpdaterThrottler = TimeSpan.Parse("00:00:00.0500000");
            // 
            // flowLayoutPanel3
            // 
            flowLayoutPanel3.BorderStyle = BorderStyle.FixedSingle;
            flowLayoutPanel3.Controls.Add(lblBattery);
            flowLayoutPanel3.Controls.Add(progressBattery);
            flowLayoutPanel3.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel3.Location = new Point(848, 66);
            flowLayoutPanel3.Name = "flowLayoutPanel3";
            flowLayoutPanel3.Padding = new Padding(10);
            flowLayoutPanel3.Size = new Size(220, 240);
            flowLayoutPanel3.TabIndex = 8;
            flowLayoutPanel3.WrapContents = false;
            // 
            // lblBattery
            // 
            lblBattery.AutoSize = true;
            lblBattery.Location = new Point(13, 10);
            lblBattery.Name = "lblBattery";
            lblBattery.Size = new Size(58, 17);
            lblBattery.TabIndex = 0;
            lblBattery.Text = "BATTERY";
            // 
            // progressBattery
            // 
            progressBattery.Location = new Point(13, 30);
            progressBattery.Name = "progressBattery";
            progressBattery.Size = new Size(180, 28);
            progressBattery.TabIndex = 1;
            // 
            // cmbPorts
            // 
            cmbPorts.FormattingEnabled = true;
            cmbPorts.Location = new Point(90, 18);
            cmbPorts.Name = "cmbPorts";
            cmbPorts.Size = new Size(121, 25);
            cmbPorts.TabIndex = 1;
            // 
            // cmbBaudRate
            // 
            cmbBaudRate.FormattingEnabled = true;
            cmbBaudRate.Location = new Point(308, 18);
            cmbBaudRate.Name = "cmbBaudRate";
            cmbBaudRate.Size = new Size(121, 25);
            cmbBaudRate.TabIndex = 3;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatus });
            statusStrip1.Location = new Point(0, 513);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1088, 22);
            statusStrip1.TabIndex = 13;
            statusStrip1.Text = "statusStrip1";
            statusStrip1.ItemClicked += statusStrip1_ItemClicked;
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(86, 17);
            lblStatus.Text = "Disconnected";
            // 
            // lstMessages
            // 
            lstMessages.BackColor = SystemColors.Info;
            lstMessages.FormattingEnabled = true;
            lstMessages.Location = new Point(18, 323);
            lstMessages.Name = "lstMessages";
            lstMessages.Size = new Size(1050, 191);
            lstMessages.TabIndex = 14;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1088, 535);
            Controls.Add(lstMessages);
            Controls.Add(statusStrip1);
            Controls.Add(cmbBaudRate);
            Controls.Add(cmbPorts);
            Controls.Add(flowLayoutPanel3);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(btnDisconnect);
            Controls.Add(btnConnect);
            Controls.Add(btnRefresh);
            Controls.Add(comPortText);
            Name = "Form1";
            Text = "SafeStep Desktop";
            Load += Form1_Load;
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            flowLayoutPanel3.ResumeLayout(false);
            flowLayoutPanel3.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label comPortText;
        private Button btnRefresh;
        private Button btnConnect;
        private Button btnDisconnect;
        private FlowLayoutPanel flowLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel2;
        private FlowLayoutPanel flowLayoutPanel3;
        private Label lblAlarm;
        private Panel pnlAlarm;
        private Label lblDistance;
        private Label lblDistanceTime;
        private Label lblBattery;
        private ProgressBar progressBattery;
        private ComboBox cmbPorts;
        private ComboBox cmbBaudRate;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblStatus;
        private ListBox lstMessages;
        private LiveChartsCore.SkiaSharpView.WinForms.CartesianChart distanceChart;
    }
}