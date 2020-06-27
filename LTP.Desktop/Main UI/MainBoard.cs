using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using static LegoTrainProject.TrainProgramEvent;
using System.Net.Mail;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using static LegoTrainProject.PFxHub;
using LegoTrainProject.LTP.Desktop.Main_UI;
using LegoTrainProject.LTP.Core.Devices;

namespace LegoTrainProject
{
    public partial class MainBoard : Form
	{
		/// <summary>
		/// TODO
		/// 
		/// Add donors
		/// - Add color trigger paramters
		/// - Interative Map
		/// - Tilt Support
		/// 
		/// </summary>

		private delegate void SetControlPropertyThreadSafeDelegate(Control control, string propertyName, object propertyValue);
        private delegate void WriteLineThreadSafeDelegate(string text, Color color);
        private delegate void AddControlToFlowPanelThreadSafeDelegate(FlowLayoutPanel host, Control control, bool addLineBreak);
        private delegate void RefreshTrainEventComboBoxThreadSafeDelegate();

        // Trains & Program Logics
        TrainProject currentProject = new TrainProject();

        // Hardware Devices
        public static List<ulong> registeredBluetoothDevices = new List<ulong>();
		public static List<ulong> rejectedBluetoothDevices = new List<ulong>();
		List<BluetoothLEAdvertisementReceivedEventArgs> devicesScanned = new List<BluetoothLEAdvertisementReceivedEventArgs>();

		BluetoothLEAdvertisementWatcher BleWatcher = new BluetoothLEAdvertisementWatcher
		{
			ScanningMode = BluetoothLEScanningMode.Active
        };

        // Helpers for UI
        List<ComboBox> targetComboxControls = new List<ComboBox>();
        public static RichTextBox DebugConsoleBox;
		public SectionsEditor sectionEditor;
        public static bool showColorDebug = false;
		public ConnectionLimitationSettings connectionLimitationSettings;
		public static bool showBLEDebug = false;
		public string Version = "V1.5 - 08/21/19";
		/// <summary>
		/// Constructor
		/// </summary>
		public MainBoard()
        {
            try
            {
				SplashForm sf = new SplashForm();
				sf.Show();

                InitializeComponent();

				var parent = this.FindForm();
				parent.Resize += Parent_Resize;

				this.Disposed += MainBoard_Disposed;
				DebugConsoleBox = ConsoleBox;

				aboutToolStripMenuItem.DropDownItems.Add($"Version {Version}");

				// Load global connection settings
				connectionLimitationSettings = ConnectionLimitationSettings.Load(AppDomain.CurrentDomain.BaseDirectory + "connectionSettings.bap");
				UpdateDeviceScanningButtonLabel();

				AddSelfDrivingTab();
				SearchForTrains();
			}
            catch (Exception ex)
            {
                MainBoard.WriteLine("FATAL ERROR:" + ex.Message, Color.Red);
                MainBoard.WriteLine(ex.StackTrace, Color.Red);
            }
        }

		/// <summary>
		/// Start a Bluetooth Scan
		/// </summary>
		private async void SearchForTrains()
        {
            try
            {
				BleWatcher.Stopped += (s, e) =>
				{
					MainBoard.WriteLine("Bluetooth scanning has stopped ...", Color.Red);
				};

				// When you find a Hub, process it
				BleWatcher.Received += (w, btAdv) =>
				{
					lock (registeredBluetoothDevices)
					{
						if (!registeredBluetoothDevices.Contains(btAdv.BluetoothAddress) && !devicesScanned.Contains(btAdv))
						{
							devicesScanned.Add(btAdv);
						}
					}
				};

                MainBoard.WriteLine($"Scanning is active! following for the following devices:");
				WriteLine(" - Powered Up Hubs & Remotes");
				WriteLine(" - WeDo 2.0 & Boost Hubs");
				WriteLine(" - SBrick & BuWizz Hubs");
				WriteLine(" - EV3 Hubs (Connect from the Device menu)");

				BleWatcher.Start();
				await TryToConnect();
			}
            catch(Exception ex)
            {
                MainBoard.WriteLine("FATAL ERROR: We could not initialize the bluetooth watcher", Color.Red);
                MainBoard.WriteLine("Exception: " + ex.Message, Color.Red);
            }
        }

		private async Task TryToConnect()
		{
			while (true)
			{
				BluetoothLEAdvertisementReceivedEventArgs currentDevice = null;

				// Update toolTip
				toolStripStatusLabelBluetooth.Text = "Bluetooth Watcher is scanning (status: " + BleWatcher.Status.ToString() + ") - " + registeredBluetoothDevices.Count + " device(s) connected - " + devicesScanned.Count + " device(s) in queue";

				if (devicesScanned.Count > 0)
				{
					lock (registeredBluetoothDevices)
					{
						currentDevice = devicesScanned[0];
						devicesScanned.RemoveAt(0);
					}
				}
				else
					await Task.Delay(1000);

				if (currentDevice != null && !registeredBluetoothDevices.Contains(currentDevice.BluetoothAddress) && !rejectedBluetoothDevices.Contains(currentDevice.BluetoothAddress))
				{
					try
					{
						var device = await BluetoothLEDevice.FromBluetoothAddressAsync(currentDevice.BluetoothAddress);

						if (device != null && (currentDevice.Advertisement.ServiceUuids.Count > 0 || device.Name == "HUB NO.4" || device.Name.Contains("SBrick") || device.Name.Contains("PFx") || device.Name.Contains("Wizz")))
						{
							Hub.HubManufacturerID type = Hub.HubManufacturerID.UNKNOWN;

							if (device.Name.Contains("SBrick"))
								type = Hub.HubManufacturerID.SBRIK;
							else if (device.Name.Contains("PFx"))
								type = Hub.HubManufacturerID.PFX;
							else if (device.Name.Contains("Wizz"))
								type = Hub.HubManufacturerID.BUWIZZ;
							else
								type = GetTypeOfHub(currentDevice);

							if (type != Hub.HubManufacturerID.UNKNOWN)
							{
								MainBoard.WriteLine(string.Format("New device found - Mac Address {0:X}", currentDevice.BluetoothAddress));

								// Check if device is limited
								if (!connectionLimitationSettings.IsMacAddressAllowed(currentDevice.BluetoothAddress, currentProject))
								{
									// Remember that this address is already rejected
									rejectedBluetoothDevices.Add(currentDevice.BluetoothAddress);
									MainBoard.WriteLine(string.Format("Mac Address {0:X} is not allowed by current limitation", currentDevice.BluetoothAddress), Color.Red);
									continue;
								}

								var gatt = await device.GetGattServicesAsync(BluetoothCacheMode.Uncached);

								if (gatt.Status != GattCommunicationStatus.Success)
								{
									MainBoard.WriteLine("Error opening communication:" + gatt.Status, Color.Red);
									continue;
								}

								// Remember that this address is already attached
								registeredBluetoothDevices.Add(currentDevice.BluetoothAddress);

								// Create a Hub of the right type
								Hub newTrain = CreateNewTrain(device, type);

								// Add all trains again
								if (newTrain != null)
								{
									// Adding the train to the layout
									AddTrainToFlowLayout(newTrain);
								}

								// New Hub in town! Let's everyone know about it.
								RefreshAllTrainEventCombox();
							}
						}
						else
						{
							if (device != null && showBLEDebug)
								MainBoard.WriteLine("Connected but discarding " + device.Name);
							else if (device == null && showBLEDebug)
								MainBoard.WriteLine("Could not connect to device - Discarding " + currentDevice.BluetoothAddress);
						}
					}
					catch (Exception ex)
					{
						MainBoard.WriteLine($"Exception while connecting " + ex.Message + ex.StackTrace, Color.Red);
					}
				}
			}
		}

		private Hub.HubManufacturerID GetTypeOfHub(BluetoothLEAdvertisementReceivedEventArgs btAdv)
		{
			foreach (Guid uid in btAdv.Advertisement.ServiceUuids)
			{
				if (uid == Guid.Parse("00001623-1212-efde-1623-785feabcd123"))
				{
					if (btAdv.Advertisement.ManufacturerData.Count > 0)
					{
						var manufacturerData = btAdv.Advertisement.ManufacturerData[0];
						var data = new byte[manufacturerData.Data.Length];
						using (var reader = DataReader.FromBuffer(manufacturerData.Data))
						{
							reader.ReadBytes(data);
						}
						// Print the company ID + the raw data in hex format

						foreach (Hub.HubManufacturerID ev in (Hub.HubManufacturerID[])Enum.GetValues(typeof(Hub.HubManufacturerID)))
							if (ev != Hub.HubManufacturerID.UNKNOWN && data[1] == (byte)ev)
							{
								return ev;
							}
					}

					return Hub.HubManufacturerID.POWERED_UP_HUB;
				}
				else if (uid == Guid.Parse("4dc591b0-857c-41de-b5f1-15abda665b0c"))
				{
					return Hub.HubManufacturerID.SBRIK;
				}
				else if (uid == Guid.Parse("00001523-1212-efde-1523-785feabcd123"))
				{
					return Hub.HubManufacturerID.WEDO; 
				}
			}

			return Hub.HubManufacturerID.UNKNOWN;
		}

		private Hub CreateNewTrain(BluetoothLEDevice device, Hub.HubManufacturerID manufacturerID)
        {
			Hub newTrain;
			// Do we already know this train?
			foreach (Hub t in currentProject.RegisteredTrains)
                if (t.DeviceId == device.DeviceId)
                {
                    newTrain = t;
                    newTrain.StartListening(device);
                    return null;
                }

			// Create a Hub of the right type
			if (manufacturerID == Hub.HubManufacturerID.BOOST_MOVE_HUB)
				newTrain = new Hub(device, Hub.Types.BOOST_MOVE_HUB);
			else if (manufacturerID == Hub.HubManufacturerID.SBRIK)
				newTrain = new SbrickHub(device, Hub.Types.SBRICK);
			else if (manufacturerID == Hub.HubManufacturerID.PFX)
				newTrain = new PFxHub(device, Hub.Types.PFX);
			else if (manufacturerID == Hub.HubManufacturerID.BUWIZZ)
				newTrain = new BuWizzHub(device, Hub.Types.BUWIZZ);
			else if (manufacturerID == Hub.HubManufacturerID.POWERED_UP_REMOTE)
				newTrain = new RemoteHub(device, Hub.Types.POWERED_UP_REMOTE);
			else if (manufacturerID == Hub.HubManufacturerID.WEDO)
				newTrain = new WedoHub(device, Hub.Types.WEDO_2_SMART_HUB);
			else
				newTrain = new Hub(device, Hub.Types.POWERED_UP_HUB);

			// Add it to the list of hub of the project
			if (newTrain.Device != null)
				currentProject.RegisteredTrains.Add(newTrain);

			return newTrain;
        }

        private void AddTrainToFlowLayout(Hub newHub)
        {
			// Create the control
			HubControl hubControl = new HubControl(newHub);

			// Make sure to be aware of any data changes on the hub
			hubControl.PortTypeRefreshed += RefreshAllTrainEventCombox;

			// Add the control
			AddControlToFlowPanel(flowLayoutTrainsPanel, hubControl, true);

			// Update the Label
			hubControl.UpdateLabels();
		}







		/////////////////////////
		/// 
		/// 
		/// UI Events for Hubs 
		/// 
		/// 
		/////////////////////////

















        /////////////////////////
        /// 
        /// 
        /// Programs 
        /// 
        /// 
        /////////////////////////






                    
            



        /// <summary>
        /// Generate a Tab + Panel for a Program
        /// </summary>
        /// <param name="newProgram"></param>
        /// <returns></returns>
        private FlowLayoutPanel GenerateProgramUI(TrainProgram newProgram, out TabPage page)
        {
            // Creates a new Tab
            page = new TabPage(newProgram.Name);
			page.ImageIndex = 0;

			tabProgramsControl.TabPages.Add(page);

			page.Tag = new List<ComboBox>();
			page.Resize += Page_Resize;

			// Create a Panel for that Train
			FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.Tag = newProgram;
            panel.Width = (page.Width > 1250) ? page.Width : 1250;
            panel.Height = page.Height;
            panel.AutoScroll = true;
            page.Controls.Add(panel);

			ToolStripProgram tsp = new ToolStripProgram();

			tsp.toolStripButtonStart.Click += StartProgramButton_Click;

			tsp.toolStripButtonAddSensor.Click += AddEventButton_Click;
			tsp.toolStripButtonAddSensor.Tag = new object[] { page, panel, EventType.Sensor_Triggered };

			tsp.toolStripButtonAddSequence.Click += AddEventButton_Click;
			tsp.toolStripButtonAddSequence.Tag = new object[] { page, panel, EventType.User_Triggerd };

			tsp.toolStripButtonDelete.Click += DeleteProgramButton_Click;
			tsp.toolStripButtonDelete.Tag = new object[] { page, newProgram }; 

			tsp.Width = panel.Width;
			tsp.Height = 40;

			panel.Controls.Add(tsp);
			panel.SetFlowBreak(tsp, true);

			return panel;
        }

        private void Page_Resize(object sender, EventArgs e)
        {
            foreach (TabPage p in tabProgramsControl.TabPages)
            {
				if (p.Controls[0].GetType() == typeof(FlowLayoutPanel))
				{
					FlowLayoutPanel f = (FlowLayoutPanel)p.Controls[0];
					f.Width = p.Width;
					f.Height = p.Height;
				}
            }
        }


        /// <summary>
        /// Start The Program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartProgramButton_Click(object sender, EventArgs e)
        {
			ToolStripButton startButton = (ToolStripButton)sender;
			int tabIndex = (currentProject.ShowSectionProgram) ? tabProgramsControl.SelectedTab.TabIndex - 1 : tabProgramsControl.SelectedTab.TabIndex;

			if (currentProject.RegisteredTrains.Count == 0)
            {
				WriteLine("Init Error for " + currentProject.Programs[tabIndex].Name + ": Need at least 1 connected train to start a program!");
                return;
            }

			TrainProgram p = currentProject.Programs[tabIndex];

			if (p.IsRunning)
				p.Stop();
			else
				p.Start(currentProject);

			AdjustStartButtonLook(p, startButton);
		}

		private void AdjustStartButtonLook(TrainProgram program, ToolStripButton startButton)
		{
			if (!program.IsRunning)
			{
				startButton.Image = Properties.Resources.icons8_start_48;
				startButton.Text = "Start Sensor Events";

			}
			else
			{
				startButton.Image = Properties.Resources.icons8_stop_48;
				startButton.Text = "Stop Sensor Events";
			}
		}

		/// <summary>
		/// Delete This Program
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DeleteProgramButton_Click(object sender, EventArgs e)
        {
            // We get the panel and program
            TabPage tab = (TabPage)((object[])((ToolStripButton)sender).Tag)[0];
            TrainProgram program = (TrainProgram)((object[])((ToolStripButton)sender).Tag)[1];

            tabProgramsControl.TabPages.Remove(tab);
            program.Stop();
            currentProject.Programs.Remove(program);
        }

        /// <summary>
        /// Change the name of the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            // We get the panel and program
            TabPage tab = (TabPage)((object[])((ToolStripButton)sender).Tag)[0];
            TrainProgram program = (TrainProgram)((object[])((ToolStripButton)sender).Tag)[1];

            program.Name = tab.Text = ((TextBox)sender).Text;     
        }

        /// <summary>
        /// Click on the "Add Event" button on a panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddEventButton_Click(object sender, EventArgs e)
        {
            // We get the panel and program
            TabPage page = (TabPage)((object[])((ToolStripButton)sender).Tag)[0];
            FlowLayoutPanel panel = (FlowLayoutPanel)((object[])((ToolStripButton)sender).Tag)[1];
            EventType type = (EventType)((object[])((ToolStripButton)sender).Tag)[2];
            TrainProgram program = (TrainProgram)panel.Tag;

            // Generate a new event & add it to the program
            TrainProgramEvent newEvent = new TrainProgramEvent(type);
            program.Events.Add(newEvent);

            // Setup the default value of the event
            newEvent.TrainDeviceID = "";
            newEvent.Action = TrainProgramEvent.ActionType.Do_Nothing;
            newEvent.Trigger = TrainProgramEvent.TriggerType2.No_Event;
            GenerateEventUI(page, panel, program, newEvent);
        }






        /////////////////////////
        /// 
        /// 
        /// Events 
        /// 
        /// 
        /////////////////////////








        /// <summary>
        /// Generate all UI Associated to a Program's Event
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="program"></param>
        /// <param name="newEvent"></param>
        private void GenerateEventUI(TabPage page, FlowLayoutPanel panel, TrainProgram program, TrainProgramEvent newEvent)
        {
            List<ComboBox> targetComboxControls = (List<ComboBox>)page.Tag;

			// Look for the 'Start Program' Button
			ToolStripButton startProgramButton = null;
			foreach (Control c in panel.Controls)
				if (c.GetType() == typeof(ToolStripProgram))
					startProgramButton = ((ToolStripProgram)c).toolStripButtonStart;

			// Add a label;
			Label label = new Label();
            label.Width = 100;
            label.Padding = new Padding(4, 6, 0, 0);
            label.Text = "Sensor Event #" + (program.Events.IndexOf(newEvent) + 1);

            if (newEvent.Type == EventType.User_Triggerd)
            {
                label.Text = (newEvent.Name == null) ? "Sequence #" + (program.Events.IndexOf(newEvent) + 1) : newEvent.Name; 

                Button startCodeButton = new Button();
                startCodeButton.Width = 38;
				startCodeButton.Height = 29;
				startCodeButton.Image = Properties.Resources.icons8_play_blue_48;
                startCodeButton.Tag = new object[] { program, newEvent, startProgramButton };
                startCodeButton.Click += StartCodeButton_Click;

                Button writeCodeButton = new Button();
				writeCodeButton.Width = 90;
				writeCodeButton.Height = 29;
				writeCodeButton.TextImageRelation = TextImageRelation.ImageBeforeText;
				writeCodeButton.Image = Properties.Resources.icons8_edit_24;
				writeCodeButton.Text = "Edit Code";
				writeCodeButton.Tag = new object[] { program, newEvent, label };
                writeCodeButton.Click += WriteCodeButton_Click;

                Button deleteEventButton = new Button();
                deleteEventButton.Width = 30;
				deleteEventButton.Height = 29;
				deleteEventButton.BackColor = Color.Red;
				deleteEventButton.Image = Properties.Resources.icons8_close_window_48;
                deleteEventButton.Tag = new object[] { newEvent, panel, label, startCodeButton, writeCodeButton, deleteEventButton };
                deleteEventButton.Click += DeleteEventButton_Click;

                AddControlToFlowPanel(panel, label, false);
                AddControlToFlowPanel(panel, startCodeButton, false);
                AddControlToFlowPanel(panel, writeCodeButton, false);
                AddControlToFlowPanel(panel, deleteEventButton, true);
            }
            else
            {

                ComboBox comboTrainBox = new ComboBox();
                comboTrainBox.BindingContext = new BindingContext();
                comboTrainBox.DisplayMember = "WhenName";
                comboTrainBox.ValueMember = "DeviceId";
                comboTrainBox.DataSource = currentProject.RegisteredTrains;
                comboTrainBox.SelectedIndex = GetIndexByDeviceID(newEvent.TrainDeviceID);

                comboTrainBox.DropDownStyle = ComboBoxStyle.DropDownList;
                comboTrainBox.Tag = newEvent;
                comboTrainBox.SelectedValueChanged += TrainBox_SelectedValueChanged;

				// Save that combo to refresh it later if name changes
                targetComboxControls.Add(comboTrainBox);

				ComboBox comboTrainPortBox = new ComboBox();
				comboTrainPortBox.Width = 85;
				comboTrainPortBox.Items.Add("On Any Ports");
				if (newEvent.TrainPort == null)
					comboTrainPortBox.SelectedIndex = 0;

				foreach (string name in Enum.GetNames(typeof(Hub.MotorPorts)))
				{
					comboTrainPortBox.Items.Add(new { Text = $"on Port {name}", Value = name });
					if (newEvent.TrainPort == name)
						comboTrainPortBox.SelectedIndex = comboTrainPortBox.Items.Count - 1;
				}

				comboTrainPortBox.DisplayMember = "Text";
				comboTrainPortBox.ValueMember = "Value";
				comboTrainPortBox.DropDownStyle = ComboBoxStyle.DropDownList;
				comboTrainPortBox.Tag = newEvent;
				comboTrainPortBox.SelectedValueChanged += ComboTrainPortBox_SelectedValueChanged;

				ComboBox comboEventBox = new ComboBox();
                comboEventBox.Width = 130;
                foreach (TrainProgramEvent.TriggerType2 ev in (TrainProgramEvent.TriggerType2[])Enum.GetValues(typeof(TrainProgramEvent.TriggerType2)))
                    comboEventBox.Items.Add(Enum.GetName(typeof(TrainProgramEvent.TriggerType2), ev).Replace('_', ' '));
                comboEventBox.SelectedIndex = (int)newEvent.Trigger;
                comboEventBox.DropDownStyle = ComboBoxStyle.DropDownList;
                comboEventBox.SelectedValueChanged += EventBox_SelectedValueChanged;

				ComboBox comboEventColorBox = new ComboBox();
				comboEventColorBox.Width = 130;
				foreach (Port.Colors ev in (Port.Colors[])Enum.GetValues(typeof(Port.Colors)))
					comboEventColorBox.Items.Add(Enum.GetName(typeof(Port.Colors), ev) + ((ev == Port.Colors.RED || ev == Port.Colors.YELLOW || ev == Port.Colors.WHITE) ? " (Recommanded)" : "") );
				comboEventColorBox.SelectedIndex = (int)newEvent.TriggerColorParam;
				comboEventColorBox.DropDownStyle = ComboBoxStyle.DropDownList;
				comboEventColorBox.Tag = newEvent;
				comboEventColorBox.SelectedValueChanged += ComboEventColorBox_SelectedValueChanged;

				TextBox paramEventDistance = new TextBox();
				paramEventDistance.Text = newEvent.TriggerDistanceParam.ToString();
				paramEventDistance.Tag = newEvent;
				paramEventDistance.TextChanged += ParamEventDistance_TextChanged;

				paramEventDistance.Visible = false;
				comboEventColorBox.Visible = false;

				if (newEvent.Trigger == TriggerType2.Distance_is_above ||
					newEvent.Trigger == TriggerType2.Distance_is_below ||
					newEvent.Trigger == TriggerType2.Raw_Value_is_above ||
					newEvent.Trigger == TriggerType2.Raw_Value_is_below
					)
					paramEventDistance.Visible = true;
				else if (newEvent.Trigger == TriggerType2.Color_Change_To)
					comboEventColorBox.Visible = true;

				comboEventBox.Tag = new object[] { newEvent, paramEventDistance, comboEventColorBox };

				Label labelThen = new Label();
                labelThen.Width = 35;
                labelThen.Text = "then";
                labelThen.Padding = new Padding(0, 6, 0, 0);

                ComboBox comboActionBox = new ComboBox();
                comboActionBox.Width = 130;
                foreach (TrainProgramEvent.ActionType ev in (TrainProgramEvent.ActionType[])Enum.GetValues(typeof(TrainProgramEvent.ActionType)))
                    comboActionBox.Items.Add(Enum.GetName(typeof(TrainProgramEvent.ActionType), ev).Replace('_', ' '));
                comboActionBox.SelectedIndex = (int)newEvent.Action;
                comboActionBox.DropDownStyle = ComboBoxStyle.DropDownList;
                comboActionBox.SelectedValueChanged += ActionBox_SelectedValueChanged;

                ComboBox comboTargetBox = new ComboBox();
                comboTargetBox.BindingContext = new BindingContext();
                comboTargetBox.DisplayMember = "OfName";
                comboTargetBox.ValueMember = "DeviceId";
                comboTargetBox.DataSource = currentProject.RegisteredTrains;
                comboTargetBox.SelectedIndex = GetIndexByDeviceID(newEvent.TargetDeviceID);

                comboTargetBox.DropDownStyle = ComboBoxStyle.DropDownList;
                comboTargetBox.Tag = newEvent;
                comboTargetBox.SelectedValueChanged += TargetBox_SelectedValueChanged;
                targetComboxControls.Add(comboTargetBox);

                ComboBox comboMotorBox = new ComboBox();
                comboMotorBox.Width = 70;
                foreach (string name in Enum.GetNames(typeof(Hub.MotorPorts)))
                {
                    comboMotorBox.Items.Add(new { Text = $"on Port {name}", Value = name });
                    if (name == newEvent.TargetPort)
                        comboMotorBox.SelectedIndex = comboMotorBox.Items.Count - 1;
                }
                comboMotorBox.DisplayMember = "Text";
                comboMotorBox.ValueMember = "Value";
                comboMotorBox.DropDownStyle = ComboBoxStyle.DropDownList;
                comboMotorBox.Enabled = newEvent.HasPortSelection();
                comboMotorBox.Tag = newEvent;
                comboMotorBox.SelectedValueChanged += ComboTargetMotorBox_SelectedValueChanged;

				ComboBox comboPfxLight = new ComboBox();
				comboPfxLight.Width = 100;

				foreach (PFxLightFx fx in Enum.GetValues(typeof(PFxLightFx)))
				{
					comboPfxLight.Items.Add(fx.ToString());
					if (newEvent.PFxLightParam == fx)
						comboPfxLight.SelectedIndex = comboPfxLight.Items.Count - 1;
				}

				comboPfxLight.DropDownStyle = ComboBoxStyle.DropDownList;
				comboPfxLight.Enabled = true;
				comboPfxLight.Tag = newEvent;
				comboPfxLight.SelectedValueChanged += ComboPfxLight_SelectedValueChanged;
				comboPfxLight.Visible = newEvent.HasPfxLightParam();

				TextBox param1 = new TextBox();
                param1.Enabled = newEvent.HasParam(0);
                param1.Text = newEvent.GetParamText(0);
                param1.Tag = new object[] { 0, newEvent };
                param1.TextChanged += Param_TextChanged;

                TextBox param2 = new TextBox();
                param2.Enabled = newEvent.HasParam(1);
                param2.Text = newEvent.GetParamText(1);
                param2.Tag = new object[] { 1, newEvent };
                param2.TextChanged += Param_TextChanged;

                Button writeCodeButton = new Button();
                writeCodeButton.Width = 60;
                writeCodeButton.Text = "Edit Code";
                writeCodeButton.Tag = new object[] { program, newEvent, label };
                writeCodeButton.Click += WriteCodeButton_Click;
                writeCodeButton.Hide();

                Button deleteEventButton = new Button();
                deleteEventButton.Width = 30;
                deleteEventButton.BackColor = Color.Red;
                deleteEventButton.Text = "X";
                deleteEventButton.Tag = new object[] { newEvent, panel, label, comboTrainBox, comboEventColorBox, paramEventDistance, comboTrainPortBox, comboEventBox, labelThen, comboTargetBox, comboActionBox, comboMotorBox, param1, param2, writeCodeButton, deleteEventButton };
                deleteEventButton.Click += DeleteEventButton_Click;

                comboActionBox.Tag = new object[] { param1, param2, comboMotorBox, writeCodeButton, comboTargetBox, comboPfxLight, newEvent };

                AddControlToFlowPanel(panel, label, false);
                AddControlToFlowPanel(panel, comboTrainBox, false);
				AddControlToFlowPanel(panel, comboEventBox, false);
				AddControlToFlowPanel(panel, comboEventColorBox, false);
				AddControlToFlowPanel(panel, paramEventDistance, false);
				AddControlToFlowPanel(panel, comboTrainPortBox, false);
				AddControlToFlowPanel(panel, labelThen, false);
                AddControlToFlowPanel(panel, comboActionBox, false);
                AddControlToFlowPanel(panel, comboTargetBox, false);
                AddControlToFlowPanel(panel, comboMotorBox, false);
				AddControlToFlowPanel(panel, comboPfxLight, false);
				AddControlToFlowPanel(panel, param1, false);
                AddControlToFlowPanel(panel, param2, false);

                AddControlToFlowPanel(panel, writeCodeButton, false);
                AddControlToFlowPanel(panel, deleteEventButton, true);

                // If we are in code edition then we hide a bunch a controls.
                if (newEvent.HasCodeEdition())
                {
                    writeCodeButton.Show();
                    param1.Hide();
                    param2.Hide();
                    comboMotorBox.Hide();
                    comboTargetBox.Hide();
                }
                else
                {
                    writeCodeButton.Hide();
                    param1.Show();
                    param2.Show();

					if (newEvent.Action == ActionType.Play_Sound)
						comboMotorBox.Hide();
					else
						comboMotorBox.Show();

					if (newEvent.Action == ActionType.Play_Sound)
						comboTargetBox.Hide();
					else
						comboTargetBox.Show();
                }
            }
        }

		private void ComboPfxLight_SelectedValueChanged(object sender, EventArgs e)
		{
			TrainProgramEvent trainEvent = (TrainProgramEvent)((ComboBox)sender).Tag;
			ComboBox lightBox = ((ComboBox)sender);

			Enum.TryParse((string)lightBox.SelectedItem, out PFxLightFx light);
			trainEvent.PFxLightParam = light;
		}

		private void ComboEventColorBox_SelectedValueChanged(object sender, EventArgs e)
		{
			TrainProgramEvent trainEvent = (TrainProgramEvent)((ComboBox)sender).Tag;
			ComboBox colorBox = ((ComboBox)sender);

			if (colorBox.SelectedIndex >= 0)
				trainEvent.TriggerColorParam = (Port.Colors)colorBox.SelectedIndex;
		}

		private void ParamEventDistance_TextChanged(object sender, EventArgs e)
		{
			TrainProgramEvent trainEvent = (TrainProgramEvent)((TextBox)sender).Tag;
			TextBox distanceBox = ((TextBox)sender);

			if (Int32.TryParse(distanceBox.Text, out int result))
			{
				distanceBox.ForeColor = Color.Black;
				trainEvent.TriggerDistanceParam = result;
			}
			else
				distanceBox.ForeColor = Color.Red;
		}

		private void ComboTrainPortBox_SelectedValueChanged(object sender, EventArgs e)
		{
			TrainProgramEvent trainEvent = (TrainProgramEvent)((ComboBox)sender).Tag;
			ComboBox trainPortBox = ((ComboBox)sender);

			if (trainPortBox.SelectedIndex == 0)
				trainEvent.TrainPort = null;
			else
				trainEvent.TrainPort = (trainPortBox.SelectedItem as dynamic).Value;
		}

		private void StartCodeButton_Click(object sender, EventArgs e)
        {
            TrainProgram program = (TrainProgram)((object[])((Control)sender).Tag)[0];
            TrainProgramEvent trainEvent = (TrainProgramEvent)((object[])((Control)sender).Tag)[1];
			ToolStripButton startProgramButton = (ToolStripButton)((object[])((Control)sender).Tag)[2];

			// Initialize Trains
			if (!program.Start(currentProject))
			{
				MainBoard.WriteLine($"You need at least 1 registered hub to start a program. '{Name}' cannot be started.", Color.Red);
				return;
			}

			// Make sure to start the section program
			if (!currentProject.Sections.IsRunning)
				currentProject.Sections.Start(currentProject);

			AdjustStartButtonLook(program, startProgramButton);
			program.StartSequence(trainEvent);

		}

        private void WriteCodeButton_Click(object sender, EventArgs e)
        {
            TrainProgram program = (TrainProgram)((object[])((Button)sender).Tag)[0];
            TrainProgramEvent trainEvent = (TrainProgramEvent)((object[])((Button)sender).Tag)[1];
			Label label = (Label)((object[])((Button)sender).Tag)[2];

			FormCodeEditor codeForm = new FormCodeEditor(trainEvent, currentProject, false);
            codeForm.Show();

			if (trainEvent.Type == EventType.User_Triggerd)
				label.Text = (trainEvent.Name == null) ? "Sequence #" + (program.Events.IndexOf(trainEvent) + 1) : trainEvent.Name;
		}

		private void ComboTargetMotorBox_SelectedValueChanged(object sender, EventArgs e)
        {
            TrainProgramEvent trainEvent = (TrainProgramEvent)((ComboBox)sender).Tag;
            trainEvent.TargetPort = (((ComboBox)sender).SelectedItem as dynamic).Value;
        }


        private int GetIndexByDeviceID(string targetDeviceID)
        {
            for (int i = 0; i < currentProject.RegisteredTrains.Count; i++)
                if (currentProject.RegisteredTrains[i].DeviceId == targetDeviceID)
                    return i;

            return -1;
        }

        /// <summary>
        /// Update event when the Source Train ID is updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrainBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;

            // Is it just being cleared up?
            if (box.SelectedIndex == -1)
                return;

            // Otherwise, let's update the reference
            TrainProgramEvent selectedEvent = (TrainProgramEvent)((Control)sender).Tag;
            selectedEvent.TrainDeviceID = currentProject.RegisteredTrains[box.SelectedIndex].DeviceId;
        }


        /// <summary>
        /// Update event when the Target Train ID is updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;

            // Is it just being cleared up?
            if (box.SelectedIndex == -1)
                return;

            // Otherwise, let's update the reference
            TrainProgramEvent selectedEvent = (TrainProgramEvent)((Control)sender).Tag;
            selectedEvent.TargetDeviceID = currentProject.RegisteredTrains[box.SelectedIndex].DeviceId;
        }

        /// <summary>
        /// Update event when the Event Type is updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            TrainProgramEvent selectedEvent = (TrainProgramEvent)((object[])((Control)sender).Tag)[0];
			TextBox paramEventDistance = (TextBox)((object[])((Control)sender).Tag)[1];
			ComboBox comboEventColorBox = (ComboBox)((object[])((Control)sender).Tag)[2];

			selectedEvent.Trigger = (TrainProgramEvent.TriggerType2)box.SelectedIndex;

			paramEventDistance.Visible = false;
			comboEventColorBox.Visible = false;

			if (selectedEvent.Trigger == TriggerType2.Distance_is_above ||
				selectedEvent.Trigger == TriggerType2.Distance_is_below ||
				selectedEvent.Trigger == TriggerType2.Raw_Value_is_above ||
				selectedEvent.Trigger == TriggerType2.Raw_Value_is_below)
				paramEventDistance.Visible = true;
			else if (selectedEvent.Trigger == TriggerType2.Color_Change_To)
				comboEventColorBox.Visible = true;
		}

        /// <summary>
        /// Update event when the Action Type is updated
        /// Updates parameters availability
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActionBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            TextBox[] paramTextboxes = new TextBox[2];
            ComboBox targetMotorComboBox;
            Button editCodeButton;
            ComboBox targetComboBox;
			ComboBox lightComboBox;

			paramTextboxes[0] = (TextBox)((object[])((Control)sender).Tag)[0];
            paramTextboxes[1] = (TextBox)((object[])((Control)sender).Tag)[1];
            targetMotorComboBox = (ComboBox)((object[])((Control)sender).Tag)[2];
            editCodeButton = (Button)((object[])((Control)sender).Tag)[3];
            targetComboBox = (ComboBox)((object[])((Control)sender).Tag)[4];
			lightComboBox = (ComboBox)((object[])((Control)sender).Tag)[5];

			TrainProgramEvent selectedEvent = (TrainProgramEvent)((object[])((Control)sender).Tag)[6];
            selectedEvent.Action = (ActionType)box.SelectedIndex;
            TrainParamType[] paramTypes = TrainProgramEvent.ParamTypes[(int)selectedEvent.Action];

            if (selectedEvent.HasCodeEdition())
            {
                editCodeButton.Show();
                paramTextboxes[0].Hide();
                paramTextboxes[1].Hide();
                targetMotorComboBox.Hide();
                targetComboBox.Hide();
				lightComboBox.Hide();

			}
            else
            {
                editCodeButton.Hide();
                paramTextboxes[0].Show();
                paramTextboxes[1].Show();
                targetComboBox.Show();
				targetMotorComboBox.Hide();
				lightComboBox.Hide();

				int currentParam = 0;
				if (paramTypes.Length > 0)
				{
					if (paramTypes[0] == TrainParamType.Port)
					{
						targetMotorComboBox.Show();
						targetMotorComboBox.Enabled = true;
						currentParam++;
					}
					else if (paramTypes[0] == TrainParamType.PfxLight)
					{
						lightComboBox.Show();
						currentParam++;
					}
					else if (paramTypes[0] == TrainParamType.Path)
					{
						targetComboBox.Hide();
					}

					for (int i = 0; i < 2; i++)
					{
						paramTextboxes[i].Enabled = false;
						if (i + currentParam < paramTypes.Length)
						{
							paramTextboxes[i].Enabled = true;
							if (paramTypes[i + currentParam] == TrainParamType.Path)
								paramTextboxes[i].Text = "C:\\Sounds\\example.mp4";
							if (paramTypes[i + currentParam] == TrainParamType.Speed)
								paramTextboxes[i].Text = "100";
							if (paramTypes[i + currentParam] == TrainParamType.Time_In_Ms)
								paramTextboxes[i].Text = "1000";
							if (paramTypes[i + currentParam] == TrainParamType.PfxFile)
								paramTextboxes[i].Text = "1";
							if (paramTypes[i + currentParam] == TrainParamType.Lights)
								paramTextboxes[i].Text = "1,2,3,4,5,6,7,8";
						}
						else
							paramTextboxes[i].Text = "Not Used";
					}
				}
            }
        }


        /// <summary>
        /// Update event when an Event's Parameter is updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Param_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int paramId = (int)((object[])textBox.Tag)[0];
            TrainProgramEvent trainEvent = (TrainProgramEvent)((object[])textBox.Tag)[1];

			if (trainEvent.Action == ActionType.PFx_Light_Fx)
				trainEvent.Lights = textBox.Text;
			else if (trainEvent.Action == ActionType.Play_Sound)
				trainEvent.Path = textBox.Text;
			else
				Int32.TryParse(textBox.Text, out trainEvent.Param[paramId]);
        }

        /// <summary>
        /// Delete an Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteEventButton_Click(object sender, EventArgs e)
        {
            object[] data = (object[])((Control)sender).Tag;
            FlowLayoutPanel panel = (FlowLayoutPanel)data[1];
            TrainProgramEvent eventToRemove = (TrainProgramEvent)data[0];
            TrainProgram program = (TrainProgram)panel.Tag;

            // Remove event in logic side
            program.Events.Remove(eventToRemove);

            // Remove UI link to the event
            for (int i = 2; i < data.Length; i++)
                panel.Controls.Remove((Control)data[i]);
        }

        private void MainBoard_Disposed(object sender, EventArgs e)
        {
            Debug.WriteLine("Disposing of all devices");
            foreach (Hub t in currentProject.RegisteredTrains)
                t.Dispose();
        }










        /////////////////////////
        /// 
        /// 
        /// Helpers 
        /// 
        /// 
        /////////////////////////



        public static void WriteLine(string text)
        {
            WriteLine(text, Color.Black);
        }


        /// <summary>
        /// Write to Debug Console
        /// </summary>
        /// <param name="text"></param>
        public static void WriteLine(string text, Color color)
        {
            Control control = DebugConsoleBox;
            if (control.InvokeRequired)
            {
                // For Debug purpose, we log everything in the console
                Debug.WriteLine(text);

                // Then invoke the thread-safe function
                control.Invoke(new WriteLineThreadSafeDelegate
                (WriteLine),
                new object[] { text, color });
            }
            else
            {
                AppendTextWithColor(DebugConsoleBox, text + Environment.NewLine, color);
                DebugConsoleBox.SelectionStart = DebugConsoleBox.Text.Length;
                // scroll it automatically
                DebugConsoleBox.ScrollToCaret();
            }
        }

        /// <summary>
        /// Add a Control to FlowPanel
        /// </summary>
        /// <param name="control"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public static void AddControlToFlowPanel(
            FlowLayoutPanel host, Control control,
            bool lineBreaker)
        {
			try
			{
				if (host.InvokeRequired)
				{
					host.Invoke(new AddControlToFlowPanelThreadSafeDelegate
					(AddControlToFlowPanel),
					new object[] { host, control, lineBreaker });
				}
				else
				{
					// Special Case - if null, we clear all controls
					if (control == null)
					{
						host.Controls.Clear();
						return;
					}

					host.Controls.Add(control);
					if (lineBreaker)
						host.SetFlowBreak(control, true);
				}
			}
			catch
			{
				MainBoard.WriteLine("Cross thread issue while adding a control", Color.Red);
			}
        }

        /// <summary>
        /// Helper Function
        /// </summary>
        /// <param name="control"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public static void SetControlPropertyThreadSafe(
            Control control,
            string propertyName,
            object propertyValue)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new SetControlPropertyThreadSafeDelegate
                (SetControlPropertyThreadSafe),
                new object[] { control, propertyName, propertyValue });
            }
            else
            {
                control.GetType().InvokeMember(
                    propertyName,
                    BindingFlags.SetProperty,
                    null,
                    control,
                    new object[] { propertyValue });
            }
        }

        public static void AppendTextWithColor(RichTextBox box, string text, Color color)
        {
			try
			{
				box.SelectionStart = box.TextLength;
				box.SelectionLength = 0;

				box.SelectionColor = color;
				box.AppendText(text);
				box.SelectionColor = box.ForeColor;
			}
			catch
			{
				// Ignore out of synch threading issues
			}
        }

        private void RefreshAllTrainEventCombox()
        {
            if (tabProgramsControl.InvokeRequired)
            {
                tabProgramsControl.Invoke(new RefreshTrainEventComboBoxThreadSafeDelegate(RefreshAllTrainEventCombox), 
                    new object[] { });   
            }
            else
            {
				if (sectionEditor != null)
					sectionEditor.RefreshSectionProperties();

				// Then go on each program and update the names of each combos
				foreach (TabPage page in tabProgramsControl.TabPages)
                {
                    List<ComboBox> targetComboBoxControls = (List<ComboBox>)page.Tag;

					if (targetComboBoxControls != null)
					{
						foreach (ComboBox b in targetComboBoxControls)
						{
							string previousDisplayMember = b.DisplayMember;
							b.DataSource = null;
							b.DisplayMember = previousDisplayMember;
							b.ValueMember = "DeviceId";
							b.DataSource = currentProject.RegisteredTrains;
							b.Refresh();
						}
					}
                }
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // We first clean any running program
            foreach (TrainProgram program in currentProject.Programs)
                program.Stop();

            // Get an openFileDia
            OpenFileDialog openFileDialog1 = GetOpenFileDialog();

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Load the project
                TrainProject newProject = TrainProject.Load(openFileDialog1.FileName);

                // If we fail to load, we keep what we have
                if (newProject == null)
                    return;

                // We check if currently connected train exist in the new project
                foreach (Hub train in currentProject.RegisteredTrains)
                    if (train.IsConnected)
                    {
                        bool wasFound = false;
                        for (int i = 0; i < newProject.RegisteredTrains.Count; i++)
                        {
							// We keep the live Hub and save what matters
                            if (newProject.RegisteredTrains[i].DeviceId == train.DeviceId)
                            {
                                // Update the name for consistency
                                train.Name = newProject.RegisteredTrains[i].Name;
								train.TrainMotorPort = newProject.RegisteredTrains[i].TrainMotorPort;
								train.CurrentPath = newProject.RegisteredTrains[i].CurrentPath;
								train.LEDColor = newProject.RegisteredTrains[i].LEDColor;
								train.RestoreLEDColor();

								for (int j = 0; j < train.RegistredPorts.Count; j++)
								{
									// We save the function
									train.RegistredPorts[j].Function = newProject.RegisteredTrains[i].RegistredPorts[j].Function;
									// If it is a Switch, we set it up on the left
									if (train.RegistredPorts[j].Function == Port.Functions.SWITCH_DOUBLECROSS ||
										train.RegistredPorts[j].Function == Port.Functions.SWITCH_STANDARD ||
										train.RegistredPorts[j].Function == Port.Functions.SWITCH_TRIXBRIX)
										train.ActivateSwitchToLeft(train.RegistredPorts[j].Id);
									else
										train.Stop(train.RegistredPorts[j].Id);

									train.RegistredPorts[j].MinDistance = 0;
									train.RegistredPorts[j].MaxDistance = 0;
								}

								// Override the train with the current one, already connected
								newProject.RegisteredTrains[i] = train;
								newProject.RegisteredTrains[i].CleanAllEvents();

								// We found a train
								wasFound = true;
                                continue;
                            }
                        }

                        // If we did not find it, then we add it to the current project as-is
                        if (!wasFound)
                            newProject.RegisteredTrains.Add(train);
					}

				currentProject = newProject;

				// Make sure such global code exist first
				if (currentProject.GlobalCode == null)
					currentProject.GlobalCode = new TrainProgramEvent(EventType.Global_Code);

				// Make sure such Sections exist first
				if (currentProject.Sections == null)
					currentProject.Sections = new Sections();

				// Make sure such paths exist
				if (currentProject.Sections.Paths == null)
					currentProject.Sections.Paths = new List<Path>();

				currentProject.Sections.ClearAllTrains();
				toolStripButtonSelfDriving.Checked = currentProject.ShowSectionProgram;

				// Clear the Flow Panel
				InitializeUIForNewProject();
            }
        }

        private void InitializeUIForNewProject()
		{
			// First we clean up all event hook up
			foreach (HubControl hc in flowLayoutTrainsPanel.Controls)
				hc.ClearAllEvents();

			// then we clear all hubs
			AddControlToFlowPanel(flowLayoutTrainsPanel, null, false);

			// Add all Hubs
			foreach (Hub train in currentProject.RegisteredTrains)
				AddTrainToFlowLayout(train);

			// Rebuild all Program Tabs
			RefreshProgramTabs();

			// Update the Save tool strip
			saveToolStripMenuItem.Text = $"Save {currentProject.Path}";
		}

		private void RefreshProgramTabs()
		{
			tabProgramsControl.TabPages.Clear();

			// First we add the Section Program
			AddSelfDrivingTab();

			// Add all programs
			foreach (TrainProgram program in currentProject.Programs)
			{
				TabPage page;
				FlowLayoutPanel panel = GenerateProgramUI(program, out page);

				foreach (TrainProgramEvent trainEvent in program.Events)
				{
					GenerateEventUI(page, panel, program, trainEvent);
				}
			}
		}

		private void AddSelfDrivingTab()
		{
			toolStripButtonSelfDriving.Text = (currentProject.ShowSectionProgram) ? "Disable Self-Driving Train Module" : "Enable Self-Driving Train Module";

			if (currentProject.ShowSectionProgram)
			{
				TabPage page = new TabPage("Self Driving Module");
				sectionEditor = new SectionsEditor(currentProject);
				sectionEditor.Size = page.Size;
				page.Resize += (s, e) =>
				{
					sectionEditor.Size = page.Size;
				};

				page.Controls.Add(sectionEditor);
				tabProgramsControl.TabPages.Add(page);
			}
		}

		private static OpenFileDialog GetOpenFileDialog()
        {
            return new OpenFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Title = "Select A Project",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "ltp",
                Filter = "ltp files (*.ltp)|*.ltp",
                FilterIndex = 2,
                RestoreDirectory = false,

                ReadOnlyChecked = true,
                ShowReadOnly = false
            };
        }

        private static SaveFileDialog GetSaveFileDialog()
        {
            return new SaveFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Title = "Save A Project",

                CheckPathExists = true,

                DefaultExt = "ltp",
                Filter = "ltp files (*.ltp)|*.ltp",
                FilterIndex = 2,
                RestoreDirectory = false,
            };
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!currentProject.Save())
            {
                saveAsToolStripMenuItem_Click(sender, e);
            }
            else
                WriteLine("Project saved to " + currentProject.Path);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = GetSaveFileDialog();
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                currentProject.SaveAs(saveFileDialog1.FileName);
                WriteLine("Project saved to " + currentProject.Path);
            }
        }

        private void createNewProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // We first clean any running program
            foreach (TrainProgram program in currentProject.Programs)
                program.Stop();

            // Create a fresh project
            TrainProject newProject = new TrainProject();

            foreach (Hub train in currentProject.RegisteredTrains)
                if (train.IsConnected)
                    newProject.RegisteredTrains.Add(train);

            currentProject = newProject;

            // And refresh the UI
            InitializeUIForNewProject();
        }

        private void createdByVincentVergonjeanneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("vincent@vergonjeanne.fr");
            mailMessage.Subject = "About The Lego Train Project";
            var filename = System.IO.Path.GetTempPath() + "tempmessage.eml";

            //save the MailMessage to the filesystem
            SaveEmail(mailMessage, filename);
            Process.Start(filename);
        }

        //Extension method for MailMessage to save to a file on disk
        public void SaveEmail(MailMessage message, string filename)
        {
            using (var filestream = File.Open(filename, FileMode.Create))
            {
                var assembly = typeof(SmtpClient).Assembly;
                var mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

                // Get reflection info for MailWriter contructor
                var mailWriterContructor = mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Stream) }, null);

                // Construct MailWriter object with our FileStream
                var mailWriter = mailWriterContructor.Invoke(new object[] { filestream });

                // Get reflection info for Send() method on MailMessage
                var sendMethod = typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);

                sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { mailWriter, true, true }, null);

                // Finally get reflection info for Close() method on our MailWriter
                var closeMethod = mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);

                // Call close method
                closeMethod.Invoke(mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);
            }
        }

        private void makeADonationToHelpSupportThisDevelopmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.me/vincentvergonjeanne");
        }

        private void inspiredByTheGreatWorkOfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/nathankellenicki/node-poweredup");
        }
    
        private void showColorDebugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainBoard.showColorDebug = showColorDebugToolStripMenuItem.Checked;
        }

		private void showBluetoothConnectionDebugToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MainBoard.showBLEDebug = showBluetoothConnectionDebugToolStripMenuItem.Checked;
		}

		private void globalCodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			currentProject.GlobalCode.Name = "Global Functions & Code";
			FormCodeEditor codeForm = new FormCodeEditor(currentProject.GlobalCode, currentProject, true);
			codeForm.Show();
		}

		private void toolStripButtonAllTrains_Click(object sender, EventArgs e)
		{
			foreach (var train in currentProject.RegisteredTrains)
			{
				train.Stop();
			}
		}

		private void toolStripButtonAddProgram_Click(object sender, EventArgs e)
		{
			// Create a new program and add it
			TrainProgram newProgram = new TrainProgram();
			currentProject.Programs.Add(newProgram);
			newProgram.Name = "Program #" + currentProject.Programs.Count;

			// Containing Page to be received
			TabPage page;
			GenerateProgramUI(newProgram, out page);

			// We focus on the last one created
			tabProgramsControl.SelectedIndex = tabProgramsControl.TabPages.Count - 1;
		}

		private void toolStripButtonEnableScanning_Click(object sender, EventArgs e)
		{
			if (toolStripButtonEnableScanning.Checked)
			{
				BleWatcher.Start();
				UpdateDeviceScanningButtonLabel();
				toolStripButtonEnableScanning.Image = Properties.Resources.icons8_bluetooth_2_48;
				WriteLine("Bluetooth is now scanning for devices. Looking for Hubs ...");
			}
			else
			{
				BleWatcher.Stop();
				toolStripButtonEnableScanning.Text = "Device Scanning Disabled";
				toolStripButtonEnableScanning.Image = Properties.Resources.icons8_bluetooth_red__2_48;
			}
		}

		private void toolStripButtonSelfDriving_Click(object sender, EventArgs e)
		{
			currentProject.ShowSectionProgram = toolStripButtonSelfDriving.Checked;
			RefreshProgramTabs();
		}

		private async void connectToEV3HubToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string result = Microsoft.VisualBasic.Interaction.InputBox("Enter the COM number of your EV3" + Environment.NewLine + "To discover your COM number check out the tutorial in the Help section.", "EV3 Bluetooth COM Number", "COM5");

			if (result != String.Empty)
			{
				// Do we already know this train?
				foreach (Hub t in currentProject.RegisteredTrains)
				{
					if (t.DeviceId == result)
					{
						EV3Hub ev3hub = (EV3Hub)t;
						ev3hub.TryToConnect();
						return;
					}
				}

				EV3Hub h = new EV3Hub(null, Hub.Types.EV3, result);
				h.TryToConnect();

				int count = 0;
				while (++count < 10 && !h.IsConnected)
				{
					await Task.Delay(500);
				}

				if (h.IsConnected)
				{ 
					currentProject.RegisteredTrains.Add(h);
					AddTrainToFlowLayout(h);
				}
			}
		}

		private void howToConnectToEV3ViaBluetoothToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.mathworks.com/help/supportpkg/legomindstormsev3io/ug/connect-to-an-ev3-brick-over-bluetooth-using-windows-1.html");
		}

		private void howToUseTheSelfDrivingTrainModuleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.eurobricks.com/forum/index.php?/forums/topic/169767-lego-train-full-layout-automation-a-break-down/");
		}

		private void howToUseTheProgramAndCustomEventsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.eurobricks.com/forum/index.php?/forums/topic/169537-automation-practical-example-of-train-automation-using-only-powered-up-devices");
		}

		private void limitDevicesPairingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ConnectionLimit cl = new ConnectionLimit(connectionLimitationSettings);
			cl.ShowDialog();
			rejectedBluetoothDevices.Clear();
			connectionLimitationSettings.SaveAs(AppDomain.CurrentDomain.BaseDirectory + "connectionSettings.bap");
			UpdateDeviceScanningButtonLabel();
		}

		private void UpdateDeviceScanningButtonLabel()
		{
			toolStripButtonEnableScanning.Text = "Device Scanning Enabled (" +
				(connectionLimitationSettings.currentLimitation == ConnectionLimitationSettings.LimitationType.None ? "No Limitations)" :
				(connectionLimitationSettings.currentLimitation == ConnectionLimitationSettings.LimitationType.OnlyProject ? "Project Devices Only)" :
				"Specific Devices Only)"));
		}

		private void Parent_Resize(object sender, EventArgs e)
		{
			if (sectionEditor != null)
			{
				sectionEditor.RecreateSectionsUI();
				sectionEditor.RefreshTrainProperties();
			}
		}
	}
}
