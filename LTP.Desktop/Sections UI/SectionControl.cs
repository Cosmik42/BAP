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
	public partial class SectionControl : UserControl
	{
		TrainProject Project;
		Section Section;
		SectionsEditor Editor;

		public SectionControl(TrainProject project, Section section, SectionsEditor editor, bool showDelete)
		{
			Project = project;
			Section = section;
			Editor = editor;

			InitializeComponent();
			RefreshUI();

			if (showDelete)
				pictureBoxDelete.Show();
		}

		public void RefreshUI()
		{
			switchSetup1.Init(Section, Project);
			labelSectionName.Text = Section.Name; 

			comboBoxDetector.Items.Clear();
			foreach (Hub h in Project.RegisteredTrains)
			{
				foreach (Port p in h.RegistredPorts)
				{
					if (p.Function == Port.Functions.SENSOR)
						comboBoxDetector.Items.Add(new DetectorItem(h, p));

					if (Section.IsDetectorPresent(h, p))
						comboBoxDetector.SelectedIndex = comboBoxDetector.Items.Count - 1;
				}
			}
		}

		private void comboBoxDetector_SelectedIndexChanged(object sender, EventArgs e)
		{
	
			DetectorItem item = (DetectorItem)comboBoxDetector.SelectedItem;
			Section.Detector = item.GetFullID();
		}

		private void pictureBoxDelete_Click(object sender, EventArgs e)
		{
			Project.Sections.sections.Remove(Section);
			Editor.RecreateSectionsUI();
		}

		private void buttonConfigure_Click(object sender, EventArgs e)
		{
			TrainSectionBehavior stb = new TrainSectionBehavior(Section, Project);
			stb.ShowDialog();
		}
	}
}
