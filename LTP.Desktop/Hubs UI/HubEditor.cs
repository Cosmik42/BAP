using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegoTrainProject
{
	public partial class HubEditor : Form
	{
		public Hub CurrentHub;

		public HubEditor(Hub hub)
		{
			CurrentHub = hub;
			InitializeComponent();

			foreach (Hub.Types ev in (Hub.Types[])Enum.GetValues(typeof(Hub.Types)))
				comboBoxType.Items.Add(Enum.GetName(typeof(Hub.Types), ev));

			foreach (Port.Colors ev in (Port.Colors[])Enum.GetValues(typeof(Port.Colors)))
				comboBoxColors.Items.Add(Enum.GetName(typeof(Port.Colors), ev));

			if (hub.Type == Hub.Types.SBRICK)
				comboBoxType.Enabled = false;
		}

		public void InitPortComponents()
		{
			flowLayoutPanelPort.Controls.Clear();

			foreach (Port p in CurrentHub.RegistredPorts)
			{
				Label labelPortName = new Label();
				labelPortName.Text = "Port " + p.Id;
				labelPortName.Padding = new Padding(0, 6, 0, 0);
				labelPortName.Width = 65;
				flowLayoutPanelPort.Controls.Add(labelPortName);

				ComboBox comboBoxPort = new ComboBox();
				comboBoxPort.Width = 150;
				comboBoxPort.DropDownStyle = ComboBoxStyle.DropDownList;
				comboBoxPort.Tag = p;

				foreach (Port.Functions t in (Port.Functions[])Enum.GetValues(typeof(Port.Functions)))
				{
					comboBoxPort.Items.Add(Enum.GetName(typeof(Port.Functions), t).Replace('_', ' '));

					if (p.Function == t)
						comboBoxPort.SelectedIndex = (int)t;
				}

				comboBoxPort.SelectedIndexChanged += ComboBoxPort_SelectedIndexChanged;

				if (p.Function == Port.Functions.SENSOR)
				{
					Label labelHeader = new Label();
					labelHeader.Text = $"Trigger Cooldown (ms)";
					labelHeader.Font = new Font(new FontFamily("Segoe UI"), 7.1f, FontStyle.Bold);

					labelHeader.Padding = new Padding(0, 0, 0, 0);
					labelHeader.Width = 80;
					labelHeader.Height = 27;

					TextBox textBoxDistanceTimer = new TextBox();
					textBoxDistanceTimer.Width = 60;
					textBoxDistanceTimer.Text = p.DistanceColorCooldownMs.ToString();
					textBoxDistanceTimer.TextChanged += (e, f) =>
					{
						if (Int32.TryParse(textBoxDistanceTimer.Text, out int value))
							p.DistanceColorCooldownMs = value;
					};

					flowLayoutPanelPort.Controls.Add(comboBoxPort);
					flowLayoutPanelPort.Controls.Add(labelHeader);
					flowLayoutPanelPort.Controls.Add(textBoxDistanceTimer);
					flowLayoutPanelPort.SetFlowBreak(textBoxDistanceTimer, true);

					this.Width = 480;
				}
				else
				{
					flowLayoutPanelPort.Controls.Add(comboBoxPort);
					flowLayoutPanelPort.SetFlowBreak(comboBoxPort, true);
				}

			}
		}

		private void ComboBoxPort_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox portBox = (ComboBox)sender;
			Port portToChange = (Port)portBox.Tag;
			portToChange.Function = (Port.Functions)portBox.SelectedIndex;

			CurrentHub.TrainMotorPort = null;
			foreach (Port port in CurrentHub.RegistredPorts)
				if (port.Function == Port.Functions.TRAIN_MOTOR)
					CurrentHub.TrainMotorPort = port.Id;

			InitPortComponents();
		}

		private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
		{
			// We change port only if the type is changed from the current one
			if (CurrentHub.Type != (Hub.Types)comboBoxType.SelectedIndex)
			{
				CurrentHub.Type = (Hub.Types)comboBoxType.SelectedIndex;
				CurrentHub.InitPorts();
				InitPortComponents();
			}
		}

		private void textBoxName_TextChanged(object sender, EventArgs e)
		{
			// Change the Hub name if needed
			if (CurrentHub.Name != textBoxName.Text)
				CurrentHub.Name = textBoxName.Text;
		}

		private void HubEditor_Shown(object sender, EventArgs e)
		{
			comboBoxType.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxType.SelectedIndex = (int)CurrentHub.Type;
			comboBoxColors.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxColors.SelectedIndex = (int)CurrentHub.LEDColor;

			textBoxName.Text = CurrentHub.Name;
			InitPortComponents();
		}

		private void comboBoxColors_SelectedIndexChanged(object sender, EventArgs e)
		{
			// We change port only if the type is changed from the current one
			if (CurrentHub.LEDColor != (Port.Colors)comboBoxColors.SelectedIndex)
			{
				CurrentHub.LEDColor = (Port.Colors)comboBoxColors.SelectedIndex;
				CurrentHub.RestoreLEDColor();
			}
		}
	}
}
