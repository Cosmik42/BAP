using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegoTrainProject
{
	public partial class SwitchSetup : UserControl
	{
		private TrainProject Project;
		private Section CurrentSection;

		public SwitchSetup()
		{
			InitializeComponent();
		}

		public void Init(Section sectionToEdit, TrainProject project)
		{
			Project = project;
			CurrentSection = sectionToEdit;
			RefreshUI();
		}

		private void RefreshUI()
		{
			comboBoxSwitch.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxSwitch.Items.Clear();

			comboBoxSwitch.Items.Add("No Switch");
			if (CurrentSection.Switch == null && CurrentSection.RightSection == null)
				comboBoxSwitch.SelectedIndex = 0;

			comboBoxSwitch.Items.Add("Manual Switch");
			if (CurrentSection.Switch == null && CurrentSection.RightSection != null)
				comboBoxSwitch.SelectedIndex = 1;

			foreach (Hub h in Project.RegisteredTrains)
			{
				foreach (Port p in h.RegistredPorts)
				{
					if (p.Function == Port.Functions.SWITCH_DOUBLECROSS || p.Function == Port.Functions.SWITCH_STANDARD || p.Function == Port.Functions.SWITCH_TRIXBRIX)
					{
						comboBoxSwitch.Items.Add(new DetectorItem(h, p));
						if (CurrentSection.IsSwitchPresent(h, p))
							comboBoxSwitch.SelectedIndex = comboBoxSwitch.Items.Count - 1;
					}
				}
			}

			comboBoxLeft.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxLeft.Items.Clear();
			foreach (Section destinationSection in Project.Sections)
			{
				comboBoxLeft.Items.Add(destinationSection);

				if (destinationSection == CurrentSection.LeftSection)
					comboBoxLeft.SelectedItem = destinationSection;
			}

			comboBoxRight.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxRight.Items.Clear();
			foreach (Section destinationSection in Project.Sections)
			{
				comboBoxRight.Items.Add(destinationSection);

				if (destinationSection == CurrentSection.RightSection)
					comboBoxRight.SelectedItem = destinationSection;
			}

			comboBoxRight.Enabled = (CurrentSection.CountForwardSections() != 1);
		}

		private void comboBoxSwitch_SelectedIndexChanged_1(object sender, EventArgs e)
		{
			comboBoxRight.Enabled = (comboBoxSwitch.SelectedIndex != 0);

			// Then we make sure to initialize things properly
			if (comboBoxSwitch.SelectedIndex == 0 || comboBoxSwitch.SelectedIndex == 1)
			{
				CurrentSection.Switch = null;

				if (comboBoxSwitch.SelectedIndex == 1 && CurrentSection.RightSection == null)
					CurrentSection.RightSection = Project.Sections[0];
				else
					comboBoxRight.SelectedIndex = -1;
			}
			else
			{
				DetectorItem item = (DetectorItem)comboBoxSwitch.SelectedItem;
				CurrentSection.Switch = item.GetFullID();

				CurrentSection.LeftSection = (CurrentSection.LeftSection == null) ? Project.Sections[0] : CurrentSection.LeftSection;
				CurrentSection.RightSection = (CurrentSection.RightSection == null) ? Project.Sections[0] : CurrentSection.RightSection;
			}
		}

		private void comboBoxRight_SelectedIndexChanged(object sender, EventArgs e)
		{
			CurrentSection.RightSection = (Section)comboBoxRight.SelectedItem;
		}

		private void comboBoxLeft_SelectedIndexChanged(object sender, EventArgs e)
		{
			CurrentSection.LeftSection = (Section)comboBoxLeft.SelectedItem;

		}
	}
}
