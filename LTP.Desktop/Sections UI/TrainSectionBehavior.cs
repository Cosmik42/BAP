using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegoTrainProject.Sections_UI
{
	public partial class TrainSectionBehavior : Form
	{
		Section Section;
		TrainProject Project;

		public TrainSectionBehavior(Section section, TrainProject project)
		{
			Section = section;
			Project = project;

			InitializeComponent();

			textBoxMaxSpeed.Text = Section.MaxSpeed.ToString();
			checkBox2Ahead.Checked = Section.NeedsTwoSectionReleased;

			if (Section.Action == Section.ActionOnRelease.Resume_Speed)
				this.radioButtonResume.Checked = true;
			else
				this.radioButtonExecute.Checked = true;
		}

		private void checkBox2Ahead_CheckedChanged(object sender, EventArgs e)
		{
			Section.NeedsTwoSectionReleased = checkBox2Ahead.Checked;
		}

		private void textBoxMaxSpeed_TextChanged(object sender, EventArgs e)
		{
			if (!Int32.TryParse(textBoxMaxSpeed.Text, out Section.MaxSpeed))
				textBoxMaxSpeed.Text = "100";
		}

		private void radioButtonResume_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButtonResume.Checked)
				Section.Action = Section.ActionOnRelease.Resume_Speed;
		}

		private void radioButtonExecute_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButtonExecute.Checked)
				Section.Action = Section.ActionOnRelease.Execute_Code;
		}

		private void buttonEditCode_Click(object sender, EventArgs e)
		{
			if (Section.CustomCodeEvent == null)
			{
				Section.CustomCodeEvent = new TrainProgramEvent(TrainProgramEvent.EventType.User_Triggerd);
				Section.CustomCodeEvent.CodeToRun = @"
Hub trainInSection = Hub[%CurrentTrainIndex%];
trainInSection.SetMotorSpeed(trainInSection.TrainMotorPort, 70);
				";
			}

			FormCodeEditor codeForm = new FormCodeEditor(Section.CustomCodeEvent, Project, false);
			codeForm.ShowDialog();
		}

		private void label3_Click(object sender, EventArgs e)
		{

		}
	}
}
