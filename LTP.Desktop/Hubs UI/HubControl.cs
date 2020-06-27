using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static LegoTrainProject.PFxHub;

namespace LegoTrainProject
{
	public partial class HubControl : UserControl
	{
		private Hub Hub;
		HubEditor Editor;
		bool hubIsTrain = false;

		public delegate void RefreshTrainLabelThreadSafeDelegate(Hub train);
		public delegate void RefreshUIThreadSafeDelegate();
		public event RefreshUIThreadSafeDelegate PortTypeRefreshed;

		public HubControl(Hub hub)
		{
			Hub = hub;
			Hub.PortTypeUpdated += RefreshUI;
			Hub.DataUpdated += UpdateLabels;

			Editor = new HubEditor(Hub);

			InitializeComponent();
			InitControl();
		}

		private void InitControl()
		{
			int width = 0;

			foreach (Port p in Hub.RegistredPorts)
			{
				switch (p.Function)
				{
					case Port.Functions.MOTOR:
					case Port.Functions.TRAIN_MOTOR:
						{
							width += 65;

							PictureBox pb = new PictureBox();
							pb.Width = 30;
							pb.Height = 30;
							pb.Margin = new Padding(10, 0, 0, 0);
							pb.SizeMode = PictureBoxSizeMode.StretchImage;
							pb.Image = Properties.Resources.icons8_train_48;

							Label labelSpeed = new Label();
							labelSpeed.Text = $"- Port {p.Id} - " + Environment.NewLine + "Speed: " + p.Speed;
							labelSpeed.Padding = new Padding(0, 0, 0, 0);
							labelSpeed.Width = 60;
							labelSpeed.Height = 24;
							labelSpeed.Font = new Font(new FontFamily("Segoe UI"), 7.1f);

							TrackBar tb = new TrackBar();
							tb.Width = 50;
							tb.Height = 100;
							tb.Minimum = -100;
							tb.Maximum = 100;
							tb.TickFrequency = 10;
							tb.Orientation = Orientation.Vertical;
							tb.SmallChange = 10;
							tb.LargeChange = 20;
							tb.Margin = new Padding(15, 0, 0, 0);
							tb.Value = 0;

							// Connect the trackbar to the label
							labelSpeed.Tag = tb;
							p.label = labelSpeed;

							tb.Tag = new object[] { Hub, labelSpeed, p.Id };
							tb.Scroll += Tb_Scrolled; 

							Button buttonStop = new Button();
							buttonStop.Text = "Stop";
							buttonStop.Tag = new object[] { Hub, p.Id }; ;
							buttonStop.Click += ButtonStopTrain_Click;
							buttonStop.Width = 50;

							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, pb, false);
							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, labelSpeed, false);
							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, tb, false);
							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, buttonStop, true);

							break;
						}
					case Port.Functions.SWITCH_DOUBLECROSS:
					case Port.Functions.SWITCH_STANDARD:
					case Port.Functions.SWITCH_TRIXBRIX:
						{
							width += 70;

							PictureBox pb = new PictureBox();
							pb.Width = 30;
							pb.Height = 30;
							pb.Margin = new Padding(10, 0, 0, 0);
							pb.SizeMode = PictureBoxSizeMode.StretchImage;
							pb.Image = Properties.Resources.icons8_train_track_48;

							Label labelSpeed = new Label();
							labelSpeed.Text = $"- Port {p.Id} - " + Environment.NewLine + "Pos: " + ((p.TargetSpeed == 0) ? "Unknown" : (p.TargetSpeed < 0) ? "Left" : "Right");
							labelSpeed.Padding = new Padding(0, 0, 0, 0);
							labelSpeed.Width = 60;
							labelSpeed.Height = 24;
							labelSpeed.Font = new Font(new FontFamily("Segoe UI"), 7.1f);
							p.label = labelSpeed;

							Button buttonLeft = new Button();
							buttonLeft.Text = "Left";
							buttonLeft.Tag = new object[] { Hub, p.Id, -100 }; ;
							buttonLeft.Click += ButtonActivateSwitch_Click;
							buttonLeft.Width = 50;

							Button buttonRight = new Button();
							buttonRight.Text = "Right";
							buttonRight.Tag = new object[] { Hub, p.Id, 100 }; ;
							buttonRight.Click += ButtonActivateSwitch_Click;
							buttonRight.Width = 50;

							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, pb, false);
							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, labelSpeed, false);
							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, buttonLeft, false);
							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, buttonRight, true);

							break;
						}
					case Port.Functions.SENSOR:
						{
							width += 70;

							PictureBox pb = new PictureBox();
							pb.Width = 30;
							pb.Height = 30;
							pb.Margin = new Padding(10, 0, 0, 0);
							pb.SizeMode = PictureBoxSizeMode.StretchImage;
							pb.Image = Properties.Resources.icons8_motion_sensor_48;

							Label labelColor = new Label();
							labelColor.Text = $"- Port {p.Id} - " + Environment.NewLine + "Color:" + Enum.GetName(typeof(Port.Colors), p.LatestColor) + Environment.NewLine + "Distance: 0";
							labelColor.Font = new Font(new FontFamily("Segoe UI"), 7.1f);

							labelColor.Padding = new Padding(0, 0, 0, 0);
							labelColor.Width = 65;
							labelColor.Height = 80;

							// Connect the trackbar to the label
							p.label = labelColor;

							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, pb, false);
							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, labelColor, true);

							break;
						}
					case Port.Functions.LIGHT:
						{
							width += 70;

							PictureBox pb = new PictureBox();
							pb.Width = 30;
							pb.Height = 30;
							pb.Margin = new Padding(10, 0, 0, 0);
							pb.SizeMode = PictureBoxSizeMode.StretchImage;
							pb.Image = Properties.Resources.icons8_light_48;

							Label labelLight = new Label();
							labelLight.Text = $"- Port {p.Id} - " + Environment.NewLine + "Light is " + ((p.TargetSpeed == 0) ? "Off" : "On");
							labelLight.Padding = new Padding(0, 0, 0, 0);
							labelLight.Width = 60;
							labelLight.Height = 36;
							labelLight.Font = new Font(new FontFamily("Segoe UI"), 7.1f);

							// Connect the trackbar to the label
							p.label = labelLight;

							Button buttonOn = new Button();
							buttonOn.Text = "On";
							buttonOn.Tag = new object[] { Hub, p.Id, 100 }; ;
							buttonOn.Click += ButtonOnOff_Click;
							buttonOn.Width = 50;

							Button buttonOff = new Button();
							buttonOff.Text = "Off";
							buttonOff.Tag = new object[] { Hub, p.Id, 0 }; ;
							buttonOff.Click += ButtonOnOff_Click;
							buttonOff.Width = 50;

							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, labelLight, false);
							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, pb, false);
							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, buttonOn, false);
							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, buttonOff, true);

							break;
						}
					case Port.Functions.BUTTON:
						{
							width += 75;

							PictureBox pb = new PictureBox();
							pb.Width = 30;
							pb.Height = 30;
							pb.Margin = new Padding(15, 0, 0, 0);
							pb.SizeMode = PictureBoxSizeMode.StretchImage;
							pb.Image = (p.Id == "A") ? Properties.Resources.icons8_xbox_a_48 : Properties.Resources.icons8_xbox_b_48;

							Label labelLight = new Label();
							labelLight.Text = $"  - Port {p.Id} - " + Environment.NewLine + ((p.Id == "A") ? "Left Buttons" : "Right Buttons");
							labelLight.Padding = new Padding(0, 0, 0, 0);
							labelLight.Width = 65;
							labelLight.Height = 36;
							labelLight.Font = new Font(new FontFamily("Segoe UI"), 7.1f);

							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, labelLight, false);
							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, pb, true);

							break;
						}
					case Port.Functions.PFX_SPEAKER:
						{
							width += 100;

							PictureBox pb = new PictureBox();
							pb.Width = 30;
							pb.Height = 30;
							pb.Margin = new Padding(10, 0, 0, 0);
							pb.SizeMode = PictureBoxSizeMode.StretchImage;
							pb.Image = Properties.Resources.icons8_speaker_48;

							Label labelLight = new Label();
							labelLight.Text = $"  - Port {p.Id} - " + Environment.NewLine + "PFx - Speaker";
							labelLight.Padding = new Padding(0, 0, 0, 0);
							labelLight.Width = 75;
							labelLight.Height = 36;
							labelLight.Font = new Font(new FontFamily("Segoe UI"), 7.1f);


							ComboBox comboFiles = new ComboBox();
							comboFiles.Width = 65;
							comboFiles.DropDownWidth = 100;
							comboFiles.DropDownStyle = ComboBoxStyle.DropDownList;

							for (int i = 0; i < 64; i++)
								comboFiles.Items.Add("File id " + i);

							Button buttonPlay = new Button();
							buttonPlay.Text = "Play";
							buttonPlay.Tag = comboFiles;
							buttonPlay.Click += ButtonPlay_Click; ;
							buttonPlay.Width = 50;

							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, pb, false);
							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, labelLight, false);
							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, comboFiles, false);
							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, buttonPlay, true);

							break;
						}

					case Port.Functions.PFX_LIGHT_CONTROLER:
						{
							width += 150;

							PictureBox pb = new PictureBox();
							pb.Width = 30;
							pb.Height = 30;
							pb.Margin = new Padding(10, 0, 0, 0);
							pb.SizeMode = PictureBoxSizeMode.StretchImage;
							pb.Image = Properties.Resources.icons8_light_automation_48;

							Label labelLight = new Label();
							labelLight.Text = $"  - Port {p.Id} - " + Environment.NewLine + "PFx - 8x Light Hub";
							labelLight.Padding = new Padding(0, 0, 0, 0);
							labelLight.Width = 95;
							labelLight.Height = 36;
							labelLight.Font = new Font(new FontFamily("Segoe UI"), 7.1f);

							ComboBox comboFiles = new ComboBox();
							comboFiles.Width = 135;
							comboFiles.DropDownWidth = 170;
							comboFiles.DropDownStyle = ComboBoxStyle.DropDownList;

							foreach (PFxLightFx fx in Enum.GetValues(typeof(PFxLightFx)))
								comboFiles.Items.Add(fx.ToString());

							Button buttonPlay = new Button();
							buttonPlay.Text = "Test Light";
							buttonPlay.Tag = comboFiles;
							buttonPlay.Click += ButtonPlayLight_Click;
							buttonPlay.Width = 50;

							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, pb, false);
							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, labelLight, false);
							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, comboFiles, false);
							MainBoard.AddControlToFlowPanel(flowLayoutPanel1, buttonPlay, true);

							break;

						}

				}
			}

			MainBoard.SetControlPropertyThreadSafe(this, "Width", (width < 110) ? 110 : width);
		}

		private void ButtonPlayLight_Click(object sender, EventArgs e)
		{
			ComboBox files = (ComboBox)((Control)sender).Tag;
			PFxHub pFxHub = (PFxHub)Hub;

			Enum.TryParse((string)files.SelectedItem, out PFxLightFx light);
			pFxHub.LightFx("1,2,3,4,5,6,7,8", (byte)light, new byte[0]);
		}

		private void ButtonPlay_Click(object sender, EventArgs e)
		{
			ComboBox files = (ComboBox)((Control)sender).Tag;
			PFxHub pFxHub = (PFxHub)Hub;
			pFxHub.PlayAudioFile((byte)files.SelectedIndex);
		}

		private void RefreshUI()
		{
			if (!hubIsTrain && Hub.IsTrain())
			{
				hubIsTrain = true;
				// Let the MainBoard know we have a new train
				PortTypeRefreshed?.Invoke();
			}

			//We clear all elements 
			MainBoard.AddControlToFlowPanel(flowLayoutPanel1, null, false);
			// And rebuild the control
			InitControl();
		}

		public void UpdateLabels()
		{
			UpdateHubLabel(Hub);
		}

		public void UpdateHubLabel(Hub train)
		{
			if (richTextBoxLabelHub.InvokeRequired)
			{
				richTextBoxLabelHub.Invoke(new RefreshTrainLabelThreadSafeDelegate(UpdateHubLabel),
					new object[] { train });
			}
			else
			{
				foreach (Port p in Hub.RegistredPorts)
					// Update the speed on the trackbar for this train if necessary
					if (p.label != null)
					{
						if (p.Function == Port.Functions.MOTOR || p.Function == Port.Functions.TRAIN_MOTOR)
						{
							TrackBar tb = (TrackBar)(p.label.Tag);
							if (tb != null && p.Speed != tb.Value)
							{
								tb.Value = (p.Speed == 127) ? -1 : (p.Speed < -100) ? -100 : (p.Speed > 100) ? 100 : p.Speed;
								p.label.Text = $"- Port {p.Id} -" + Environment.NewLine + $"Speed: {tb.Value}";
							}
						}
						else if (p.Function == Port.Functions.SWITCH_TRIXBRIX || p.Function == Port.Functions.SWITCH_STANDARD || p.Function == Port.Functions.SWITCH_DOUBLECROSS)
						{
							p.label.Text = $"- Port {p.Id} -" + Environment.NewLine + "Pos: " + ((p.TargetSpeed == 0) ? "Unknown" : (p.TargetSpeed < 0) ? "Left" : "Right");
						}
						else if (p.Function == Port.Functions.LIGHT)
						{
							p.label.Text = $"- Port {p.Id} - " + Environment.NewLine + "Light is " + ((p.TargetSpeed == 0) ? "Off" : "On");
						}
						else if (p.Function == Port.Functions.SENSOR)
						{
							p.label.Text = $"- Port {p.Id} - " + Environment.NewLine;

							if (p.Device == Port.Devices.BOOST_DISTANCE || p.Device == Port.Devices.EV3_COLOR_SENSOR)
								p.label.Text += "Color:" + Enum.GetName(typeof(Port.Colors), p.LatestColor) + Environment.NewLine;

							if (p.Device == Port.Devices.EV3_SENSOR || p.Device == Port.Devices.NXT_SENSOR)
								p.label.Text += "Raw : " + p.LatestDistance;
							else if (p.Device != Port.Devices.EV3_COLOR_SENSOR)
								p.label.Text += Environment.NewLine + "Distance: " + p.LatestDistance;
						}
					}

				// Update the Label!
				richTextBoxLabelHub.Text = train.Name + " - " + train.BatteryLevel + "%";

				using (Graphics g = CreateGraphics())
				{
					SizeF size = g.MeasureString(richTextBoxLabelHub.Text, richTextBoxLabelHub.Font, 495);
					richTextBoxLabelHub.Width = (int)Math.Ceiling(size.Width);

					if (Width < richTextBoxLabelHub.Width + 25)
						MainBoard.SetControlPropertyThreadSafe(this, "Width", richTextBoxLabelHub.Width + 25);
				}

				pictureBoxStatus.Image = (train.IsConnected) ? Port.colorBitmaps[(int)train.LEDColor] : Properties.Resources.disconnected;
				buttonDisconnect.Visible = train.IsConnected;
			}
		}


		private void ButtonOnOff_Click(object sender, EventArgs e)
		{
			Hub hub = (Hub)((object[])((Button)sender).Tag)[0];
			string port = (String)((object[])((Button)sender).Tag)[1];
			int brightness = (int)((object[])((Button)sender).Tag)[2];

			hub.SetLightBrightness(port, brightness);
		}

		private void ButtonActivateSwitch_Click(object sender, EventArgs e)
		{
			Hub hub = (Hub)((object[])((Button)sender).Tag)[0];
			string port = (String)((object[])((Button)sender).Tag)[1];
			int speed = (int)((object[])((Button)sender).Tag)[2];

			hub.ActivateSwitch(port, speed < 0);
		}

		/// <summary>
		/// Changing the speed value
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Tb_Scrolled(object sender, EventArgs e)
		{
			Hub hub = (Hub)((object[])((TrackBar)sender).Tag)[0];
			Label label = (Label)((object[])((TrackBar)sender).Tag)[1];
			string port = (String)((object[])((TrackBar)sender).Tag)[2];

			hub.SetMotorSpeed(port, ((TrackBar)sender).Value);
			label.Text = $"- Port {port} -" + Environment.NewLine + $"Speed: {((TrackBar)sender).Value}";
		}


		/// <summary>
		/// Click on Stop a Train
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonStopTrain_Click(object sender, EventArgs e)
		{
			Hub train = (Hub)((object[])((Button)sender).Tag)[0];
			string port = (String)((object[])((Button)sender).Tag)[1];
			train.Stop(port, true);
		}

		private void ButtonConfigure_Click(object sender, EventArgs e)
		{
			// Wait for the Dialog
			DialogResult r = Editor.ShowDialog();

			// Refresh all Ports
			RefreshUI();

			// Refresh Combo Boxes
			PortTypeRefreshed?.Invoke();

			// Update the label of the Hub
			UpdateLabels();
		}

		internal void ClearAllEvents()
		{
			Hub.PortTypeUpdated -= RefreshUI;
			Hub.DataUpdated -= UpdateLabels;
		}

		private void buttonDisconnect_Click(object sender, EventArgs e)
		{
			Hub.Dispose();
			UpdateHubLabel(Hub);
		}
	}
}
