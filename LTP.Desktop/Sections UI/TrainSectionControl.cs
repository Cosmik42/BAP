using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LegoTrainProject.Sections_UI;

namespace LegoTrainProject
{
	public partial class TrainSectionControl : UserControl
	{
		TrainProject Project;
		public Hub Train;
		string PortId;
		SectionsEditor Editor;
		bool RefreshingUI = false;

		public TrainSectionControl(TrainProject project, SectionsEditor editor, Hub h, Port p)
		{
			Project = project;
			Train = h;
			PortId = p.Id;
			Editor = editor;
			Train.DataUpdated += Train_DataUpdated;

			SuspendLayout();
			InitializeComponent();
			RefreshUI();
		}

		private void Train_DataUpdated()
		{
			labelName.ForeColor = (Train.IsConnected) ? Color.DarkGreen : Color.Red;
		}

		public void ClearTrainEvent()
		{
			Train.DataUpdated -= Train_DataUpdated;
		}

		public void RefreshUI()
		{
			RefreshingUI = true;

			labelName.Text = Train.Name;
			labelName.ForeColor = (Train.IsConnected) ? Color.DarkGreen : Color.Red;
			checkBoxLoop.Checked = Train.LoopCurrentPath;

			comboBoxCurrentSection.Items.Clear();
			comboBoxCurrentSection.Items.Add("Out of network");
			if (!Project.Sections.IsTrainInNetwork(Train))
				comboBoxCurrentSection.SelectedIndex = 0;

			for (int i = 0; i < Project.Sections.GetAll().Count; i++)
			{
				Section s = Project.Sections[i];
				comboBoxCurrentSection.Items.Add(s);

				if (s.CurrentHub == Train)
					comboBoxCurrentSection.SelectedIndex = i + 1;
			}

			comboBoxPath.Items.Clear();
			comboBoxPath.Items.Add("Stop - No Path");
			if (Train.CurrentPath == null)
				comboBoxPath.SelectedIndex = 0;

			for (int i = 0; i < Project.Sections.Paths.Count; i++)
			{
				Path p = Project.Sections.Paths[i];
				comboBoxPath.Items.Add(p);

				if (Train.CurrentPath == p)
					comboBoxPath.SelectedIndex = i + 1;
			}

			RefreshingUI = false;
		}

		private void ComboBoxCurrentSection_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (RefreshingUI)
				return;

			// If we picked a section
			if (comboBoxCurrentSection.SelectedIndex > 0)
			{
				Section selectedSection = (Section)comboBoxCurrentSection.Items[comboBoxCurrentSection.SelectedIndex];
				
				if (selectedSection.CurrentHub != Train)
				{
					// Clean if this was anywhere else before
					Project.Sections.ClearHub(Train);

					// Assign hub to new spot
					selectedSection.CurrentHub = Train;

					// Refresh the UI!
					Editor.RefreshTrainProperties();
				}
			}
			else
			{
				// Clean if this was anywhere else before
				Project.Sections.ClearHub(Train);
			}
		}

		private void comboBoxPath_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (RefreshingUI)
				return;

			if (!Project.Sections.IsRunning)
			{
				if (comboBoxPath.SelectedIndex > 0)
					Train.CurrentPath = (Path)comboBoxPath.SelectedItem;
				else
					Train.CurrentPath = null;
			}
		}

		private async void startButton_Click(object sender, EventArgs e)
		{
			if (!Project.Sections.IsRunning)
			{
				MessageBox.Show("You first need to start the program");
				return;
			}

			startButton.Enabled = false;

			if (comboBoxPath.SelectedIndex > 0)
			{
				if (await Train.InitNewAutomatedPath(Project.Sections, (Path)comboBoxPath.SelectedItem))
				{
					if (!Train.IsPathProgramRunning && Train.CurrentPath != null && Train.CurrentPathPositionIndex >= 0)
					{
						Train.IsPathProgramRunning = true;
						Section currentSection = Project.Sections[Train.CurrentPath.Sections[Train.CurrentPathPositionIndex]];

						await Project.Sections.ReserveNextTrainSection(currentSection, Train);
					}
				}
			}
			else
			{
				await Train.ClearAutomatedPathAndStop(true);
			}

			startButton.Enabled = true;
		}

		private void buttonConfigure_Click(object sender, EventArgs e)
		{
			TrainOptions optionForm = new TrainOptions(Train);
			optionForm.ShowDialog();
		}

		private async void buttonStop_Click(object sender, EventArgs e)
		{
			if (!Project.Sections.IsRunning)
			{
				MessageBox.Show("You first need to start the program");
				return;
			}

			await Train.ClearAutomatedPathAndStop(true);
			comboBoxPath.SelectedIndex = 0;
		}

		private void checkBoxLoop_CheckedChanged(object sender, EventArgs e)
		{
			Train.LoopCurrentPath = checkBoxLoop.Checked;
		}
	}
}
