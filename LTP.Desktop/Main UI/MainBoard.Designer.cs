namespace LegoTrainProject
{
    partial class MainBoard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainBoard));
			this.ConsoleBox = new System.Windows.Forms.RichTextBox();
			this.flowLayoutTrainsPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.tabProgramsControl = new System.Windows.Forms.TabControl();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.createNewProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.devicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.limitDevicesPairingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.connectToEV3HubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.programsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.globalCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showColorDebugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showBluetoothConnectionDebugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.howToConnectToEV3ViaBluetoothToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.howToUseTheSelfDrivingTrainModuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.howToUseTheProgramAndCustomEventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.createdByVincentVergonjeanneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.inspiredByTheGreatWorkOfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.makeADonationToHelpSupportThisDevelopmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabelBluetooth = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonEnableScanning = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonAddProgram = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonStopAllTrains = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonSelfDriving = new System.Windows.Forms.ToolStripButton();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// ConsoleBox
			// 
			this.ConsoleBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ConsoleBox.Location = new System.Drawing.Point(13, 64);
			this.ConsoleBox.Name = "ConsoleBox";
			this.ConsoleBox.Size = new System.Drawing.Size(1454, 133);
			this.ConsoleBox.TabIndex = 4;
			this.ConsoleBox.Text = "";
			// 
			// flowLayoutTrainsPanel
			// 
			this.flowLayoutTrainsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutTrainsPanel.AutoScroll = true;
			this.flowLayoutTrainsPanel.BackColor = System.Drawing.SystemColors.ControlLight;
			this.flowLayoutTrainsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutTrainsPanel.Location = new System.Drawing.Point(6, 21);
			this.flowLayoutTrainsPanel.Name = "flowLayoutTrainsPanel";
			this.flowLayoutTrainsPanel.Size = new System.Drawing.Size(1443, 341);
			this.flowLayoutTrainsPanel.TabIndex = 15;
			// 
			// tabProgramsControl
			// 
			this.tabProgramsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabProgramsControl.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabProgramsControl.ImageList = this.imageList;
			this.tabProgramsControl.Location = new System.Drawing.Point(6, 21);
			this.tabProgramsControl.Multiline = true;
			this.tabProgramsControl.Name = "tabProgramsControl";
			this.tabProgramsControl.SelectedIndex = 0;
			this.tabProgramsControl.Size = new System.Drawing.Size(1444, 284);
			this.tabProgramsControl.TabIndex = 16;
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "Program");
			this.imageList.Images.SetKeyName(1, "SelfDriving");
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.tabProgramsControl);
			this.groupBox1.Location = new System.Drawing.Point(11, 577);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(1456, 321);
			this.groupBox1.TabIndex = 19;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Programs";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.flowLayoutTrainsPanel);
			this.groupBox2.Location = new System.Drawing.Point(12, 203);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(1459, 368);
			this.groupBox2.TabIndex = 21;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Hubs";
			// 
			// menuStrip1
			// 
			this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.devicesToolStripMenuItem,
            this.programsToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.aboutToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1479, 28);
			this.menuStrip1.TabIndex = 22;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createNewProjectToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(44, 24);
			this.toolStripMenuItem1.Text = "File";
			// 
			// createNewProjectToolStripMenuItem
			// 
			this.createNewProjectToolStripMenuItem.Name = "createNewProjectToolStripMenuItem";
			this.createNewProjectToolStripMenuItem.Size = new System.Drawing.Size(250, 26);
			this.createNewProjectToolStripMenuItem.Text = "Create New Project";
			this.createNewProjectToolStripMenuItem.Click += new System.EventHandler(this.createNewProjectToolStripMenuItem_Click);
			// 
			// loadToolStripMenuItem
			// 
			this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
			this.loadToolStripMenuItem.Size = new System.Drawing.Size(250, 26);
			this.loadToolStripMenuItem.Text = "Load Project";
			this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(250, 26);
			this.saveToolStripMenuItem.Text = "Save Current Project";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(250, 26);
			this.saveAsToolStripMenuItem.Text = "Save Current Project As ...";
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
			// 
			// devicesToolStripMenuItem
			// 
			this.devicesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.limitDevicesPairingToolStripMenuItem,
            this.toolStripSeparator1,
            this.connectToEV3HubToolStripMenuItem});
			this.devicesToolStripMenuItem.Name = "devicesToolStripMenuItem";
			this.devicesToolStripMenuItem.Size = new System.Drawing.Size(72, 24);
			this.devicesToolStripMenuItem.Text = "Devices";
			// 
			// limitDevicesPairingToolStripMenuItem
			// 
			this.limitDevicesPairingToolStripMenuItem.Name = "limitDevicesPairingToolStripMenuItem";
			this.limitDevicesPairingToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
			this.limitDevicesPairingToolStripMenuItem.Text = "Limit Devices Pairing";
			this.limitDevicesPairingToolStripMenuItem.Click += new System.EventHandler(this.limitDevicesPairingToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(274, 6);
			// 
			// connectToEV3HubToolStripMenuItem
			// 
			this.connectToEV3HubToolStripMenuItem.Name = "connectToEV3HubToolStripMenuItem";
			this.connectToEV3HubToolStripMenuItem.Size = new System.Drawing.Size(277, 26);
			this.connectToEV3HubToolStripMenuItem.Text = "Connect to EV3 via Bluetooth";
			this.connectToEV3HubToolStripMenuItem.Click += new System.EventHandler(this.connectToEV3HubToolStripMenuItem_Click);
			// 
			// programsToolStripMenuItem
			// 
			this.programsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.globalCodeToolStripMenuItem});
			this.programsToolStripMenuItem.Name = "programsToolStripMenuItem";
			this.programsToolStripMenuItem.Size = new System.Drawing.Size(84, 24);
			this.programsToolStripMenuItem.Text = "Programs";
			// 
			// globalCodeToolStripMenuItem
			// 
			this.globalCodeToolStripMenuItem.Name = "globalCodeToolStripMenuItem";
			this.globalCodeToolStripMenuItem.Size = new System.Drawing.Size(165, 26);
			this.globalCodeToolStripMenuItem.Text = "Global code";
			this.globalCodeToolStripMenuItem.Click += new System.EventHandler(this.globalCodeToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showColorDebugToolStripMenuItem,
            this.showBluetoothConnectionDebugToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
			this.viewToolStripMenuItem.Text = "View";
			// 
			// showColorDebugToolStripMenuItem
			// 
			this.showColorDebugToolStripMenuItem.CheckOnClick = true;
			this.showColorDebugToolStripMenuItem.Name = "showColorDebugToolStripMenuItem";
			this.showColorDebugToolStripMenuItem.Size = new System.Drawing.Size(317, 26);
			this.showColorDebugToolStripMenuItem.Text = "Show Color and Distance Debug";
			this.showColorDebugToolStripMenuItem.Click += new System.EventHandler(this.showColorDebugToolStripMenuItem_Click);
			// 
			// showBluetoothConnectionDebugToolStripMenuItem
			// 
			this.showBluetoothConnectionDebugToolStripMenuItem.CheckOnClick = true;
			this.showBluetoothConnectionDebugToolStripMenuItem.Name = "showBluetoothConnectionDebugToolStripMenuItem";
			this.showBluetoothConnectionDebugToolStripMenuItem.Size = new System.Drawing.Size(317, 26);
			this.showBluetoothConnectionDebugToolStripMenuItem.Text = "Show Bluetooth Connection Debug";
			this.showBluetoothConnectionDebugToolStripMenuItem.Click += new System.EventHandler(this.showBluetoothConnectionDebugToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.howToConnectToEV3ViaBluetoothToolStripMenuItem,
            this.howToUseTheSelfDrivingTrainModuleToolStripMenuItem,
            this.howToUseTheProgramAndCustomEventsToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// howToConnectToEV3ViaBluetoothToolStripMenuItem
			// 
			this.howToConnectToEV3ViaBluetoothToolStripMenuItem.Name = "howToConnectToEV3ViaBluetoothToolStripMenuItem";
			this.howToConnectToEV3ViaBluetoothToolStripMenuItem.Size = new System.Drawing.Size(380, 26);
			this.howToConnectToEV3ViaBluetoothToolStripMenuItem.Text = "How to connect to EV3 via Bluetooth?";
			this.howToConnectToEV3ViaBluetoothToolStripMenuItem.Click += new System.EventHandler(this.howToConnectToEV3ViaBluetoothToolStripMenuItem_Click);
			// 
			// howToUseTheSelfDrivingTrainModuleToolStripMenuItem
			// 
			this.howToUseTheSelfDrivingTrainModuleToolStripMenuItem.Name = "howToUseTheSelfDrivingTrainModuleToolStripMenuItem";
			this.howToUseTheSelfDrivingTrainModuleToolStripMenuItem.Size = new System.Drawing.Size(380, 26);
			this.howToUseTheSelfDrivingTrainModuleToolStripMenuItem.Text = "How to use the Self-Driving Train Module?";
			this.howToUseTheSelfDrivingTrainModuleToolStripMenuItem.Click += new System.EventHandler(this.howToUseTheSelfDrivingTrainModuleToolStripMenuItem_Click);
			// 
			// howToUseTheProgramAndCustomEventsToolStripMenuItem
			// 
			this.howToUseTheProgramAndCustomEventsToolStripMenuItem.Name = "howToUseTheProgramAndCustomEventsToolStripMenuItem";
			this.howToUseTheProgramAndCustomEventsToolStripMenuItem.Size = new System.Drawing.Size(380, 26);
			this.howToUseTheProgramAndCustomEventsToolStripMenuItem.Text = "How to create custom programs and events?";
			this.howToUseTheProgramAndCustomEventsToolStripMenuItem.Click += new System.EventHandler(this.howToUseTheProgramAndCustomEventsToolStripMenuItem_Click);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createdByVincentVergonjeanneToolStripMenuItem,
            this.inspiredByTheGreatWorkOfToolStripMenuItem,
            this.makeADonationToHelpSupportThisDevelopmentToolStripMenuItem});
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(62, 24);
			this.aboutToolStripMenuItem.Text = "About";
			// 
			// createdByVincentVergonjeanneToolStripMenuItem
			// 
			this.createdByVincentVergonjeanneToolStripMenuItem.Name = "createdByVincentVergonjeanneToolStripMenuItem";
			this.createdByVincentVergonjeanneToolStripMenuItem.Size = new System.Drawing.Size(445, 26);
			this.createdByVincentVergonjeanneToolStripMenuItem.Text = "Created by Vincent Vergonjeanne (All Rights Reserved)";
			this.createdByVincentVergonjeanneToolStripMenuItem.Click += new System.EventHandler(this.createdByVincentVergonjeanneToolStripMenuItem_Click);
			// 
			// inspiredByTheGreatWorkOfToolStripMenuItem
			// 
			this.inspiredByTheGreatWorkOfToolStripMenuItem.Name = "inspiredByTheGreatWorkOfToolStripMenuItem";
			this.inspiredByTheGreatWorkOfToolStripMenuItem.Size = new System.Drawing.Size(445, 26);
			this.inspiredByTheGreatWorkOfToolStripMenuItem.Text = "Inspired by the great work of Nathan Kellenicki";
			this.inspiredByTheGreatWorkOfToolStripMenuItem.Click += new System.EventHandler(this.inspiredByTheGreatWorkOfToolStripMenuItem_Click);
			// 
			// makeADonationToHelpSupportThisDevelopmentToolStripMenuItem
			// 
			this.makeADonationToHelpSupportThisDevelopmentToolStripMenuItem.Name = "makeADonationToHelpSupportThisDevelopmentToolStripMenuItem";
			this.makeADonationToHelpSupportThisDevelopmentToolStripMenuItem.Size = new System.Drawing.Size(445, 26);
			this.makeADonationToHelpSupportThisDevelopmentToolStripMenuItem.Text = "Make a Donation to Help Support this Development";
			this.makeADonationToHelpSupportThisDevelopmentToolStripMenuItem.Click += new System.EventHandler(this.makeADonationToHelpSupportThisDevelopmentToolStripMenuItem_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelBluetooth});
			this.statusStrip1.Location = new System.Drawing.Point(0, 885);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(1479, 25);
			this.statusStrip1.TabIndex = 23;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabelBluetooth
			// 
			this.toolStripStatusLabelBluetooth.Name = "toolStripStatusLabelBluetooth";
			this.toolStripStatusLabelBluetooth.Size = new System.Drawing.Size(151, 20);
			this.toolStripStatusLabelBluetooth.Text = "Looking for devices ...";
			// 
			// toolStrip1
			// 
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEnableScanning,
            this.toolStripButtonAddProgram,
            this.toolStripButtonStopAllTrains,
            this.toolStripButtonSelfDriving});
			this.toolStrip1.Location = new System.Drawing.Point(0, 28);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(1479, 27);
			this.toolStrip1.TabIndex = 24;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButtonEnableScanning
			// 
			this.toolStripButtonEnableScanning.Checked = true;
			this.toolStripButtonEnableScanning.CheckOnClick = true;
			this.toolStripButtonEnableScanning.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripButtonEnableScanning.Image = global::LegoTrainProject.Properties.Resources.icons8_bluetooth_2_48;
			this.toolStripButtonEnableScanning.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonEnableScanning.Name = "toolStripButtonEnableScanning";
			this.toolStripButtonEnableScanning.Size = new System.Drawing.Size(200, 24);
			this.toolStripButtonEnableScanning.Text = "Device Scanning Enabled";
			this.toolStripButtonEnableScanning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripButtonEnableScanning.Click += new System.EventHandler(this.toolStripButtonEnableScanning_Click);
			// 
			// toolStripButtonAddProgram
			// 
			this.toolStripButtonAddProgram.Image = global::LegoTrainProject.Properties.Resources.icons8_plus_48;
			this.toolStripButtonAddProgram.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonAddProgram.Name = "toolStripButtonAddProgram";
			this.toolStripButtonAddProgram.Size = new System.Drawing.Size(122, 24);
			this.toolStripButtonAddProgram.Text = "Add Program";
			this.toolStripButtonAddProgram.Click += new System.EventHandler(this.toolStripButtonAddProgram_Click);
			// 
			// toolStripButtonStopAllTrains
			// 
			this.toolStripButtonStopAllTrains.Image = global::LegoTrainProject.Properties.Resources.icons8_private_48;
			this.toolStripButtonStopAllTrains.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonStopAllTrains.Name = "toolStripButtonStopAllTrains";
			this.toolStripButtonStopAllTrains.Size = new System.Drawing.Size(124, 24);
			this.toolStripButtonStopAllTrains.Text = "Stop All Hubs";
			this.toolStripButtonStopAllTrains.Click += new System.EventHandler(this.toolStripButtonAllTrains_Click);
			// 
			// toolStripButtonSelfDriving
			// 
			this.toolStripButtonSelfDriving.CheckOnClick = true;
			this.toolStripButtonSelfDriving.Image = global::LegoTrainProject.Properties.Resources.icons8_stop_train_48;
			this.toolStripButtonSelfDriving.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonSelfDriving.Name = "toolStripButtonSelfDriving";
			this.toolStripButtonSelfDriving.Size = new System.Drawing.Size(280, 24);
			this.toolStripButtonSelfDriving.Text = "Self-Driving Train Module is Disabled";
			this.toolStripButtonSelfDriving.Click += new System.EventHandler(this.toolStripButtonSelfDriving_Click);
			// 
			// MainBoard
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1479, 910);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.ConsoleBox);
			this.Controls.Add(this.menuStrip1);
			this.DoubleBuffered = true;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainBoard";
			this.Text = "The Brick Automation Project";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox ConsoleBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutTrainsPanel;
        private System.Windows.Forms.TabControl tabProgramsControl;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createNewProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createdByVincentVergonjeanneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inspiredByTheGreatWorkOfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeADonationToHelpSupportThisDevelopmentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showColorDebugToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showBluetoothConnectionDebugToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem programsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem globalCodeToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelBluetooth;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButtonAddProgram;
		private System.Windows.Forms.ToolStripButton toolStripButtonSelfDriving;
		private System.Windows.Forms.ToolStripButton toolStripButtonStopAllTrains;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.ToolStripButton toolStripButtonEnableScanning;
		private System.Windows.Forms.ToolStripMenuItem devicesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem connectToEV3HubToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem howToConnectToEV3ViaBluetoothToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem howToUseTheSelfDrivingTrainModuleToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem howToUseTheProgramAndCustomEventsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem limitDevicesPairingToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
	}
}