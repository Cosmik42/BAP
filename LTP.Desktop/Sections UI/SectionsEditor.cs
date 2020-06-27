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
using System.Windows.Threading;

namespace LegoTrainProject
{
	public partial class SectionsEditor : UserControl
	{
		private TrainProject Project;

		public SectionsEditor(TrainProject project)
		{
			Project = project;
			InitializeComponent();

			RecreateTrainsUI();
			RecreateSectionsUI();

			// Hook up event if any part of the sections change
			Project.Sections.DataUpdated += RefreshTrainProperties;
		}

		public void RefreshUI()
		{
			if (flowLayoutPanelSections.InvokeRequired)
			{
				flowLayoutPanelSections.Invoke(new Sections.RefreshUIThreadSafeDelegate(RefreshUI),
					new object[] { });
			}
			else
			{
				RecreateTrainsUI();
				RefreshSectionProperties();
				UpdateStartButtonStatus();
			}
		}

		public void RecreateSectionsUI()
		{
			flowLayoutPanelSections.Controls.Clear();
			int totalSections = Project.Sections.GetAll().Count;

			for (int i = 0; i < totalSections; i++)
			{
				Section s = Project.Sections[i];
				SectionControl sc = new SectionControl(Project, s, this, (i == totalSections - 1));
				sc.Width = 147;
				sc.Height = flowLayoutPanelSections.Height;
				flowLayoutPanelSections.Controls.Add(sc);
				flowLayoutPanelSections.SetFlowBreak(sc, true);
			}
		}

		private void RecreateTrainsUI()
		{
			// If we have any previous visual, they need to detach from their hub's event
			foreach (TrainSectionControl tsc in flowLayoutPanelTrains.Controls)
				tsc.ClearTrainEvent();

			// Then we clear them all
			flowLayoutPanelTrains.Controls.Clear();
			int addedTrains = 0;

			// And rebuild them!
			foreach (Hub h in Project.RegisteredTrains)
			{
				h.ClearingTimeInMs = (h.ClearingTimeInMs == 0) ? 2000 : h.ClearingTimeInMs;
				h.SpeedCoefficient = (h.SpeedCoefficient == 0) ? 1.0f : h.SpeedCoefficient;
				h.SpeedWhenAboutToStop = (h.SpeedWhenAboutToStop == 0) ? 40 : h.SpeedWhenAboutToStop;

				foreach (Port p in h.RegistredPorts)
				{
					if (p.Function != Port.Functions.TRAIN_MOTOR)
						continue;

					addedTrains++;

					TrainSectionControl tsc = new TrainSectionControl(Project, this, h, p);

					flowLayoutPanelTrains.Controls.Add(tsc);
					//flowLayoutPanelTrains.SetFlowBreak(tsc, true);
				}
			}

			if (addedTrains == 0)
				labelNoTrains.Show();
			else
				labelNoTrains.Hide();


		}

		internal void RefreshSectionProperties()
		{
			if (flowLayoutPanelSections.InvokeRequired)
			{
				flowLayoutPanelSections.Invoke(new Sections.RefreshUIThreadSafeDelegate(RefreshSectionProperties),
					new object[] { });
			}
			else
			{
				foreach (SectionControl sc in flowLayoutPanelSections.Controls)
				{
					sc.RefreshUI();
				}
			}
		}

		internal void RefreshTrainProperties()
		{
			if (flowLayoutPanelTrains.InvokeRequired)
			{
				flowLayoutPanelTrains.Invoke(new Sections.RefreshUIThreadSafeDelegate(RefreshTrainProperties),
					new object[] { });
			}
			else
			{
				foreach (TrainSectionControl tsc in flowLayoutPanelTrains.Controls)
				{
					tsc.RefreshUI();
				}
			}
		}

		private void UpdateStartButtonStatus()
		{
			if (!Project.Sections.IsRunning)
			{
				buttonStart.Image = Properties.Resources.icons8_start_48;
				buttonStart.Text = "Start Program";
			}
			else
			{
				buttonStart.Image = Properties.Resources.icons8_stop_48;
				buttonStart.Text = "Stop Program";
			}

			foreach (TrainSectionControl tsc in flowLayoutPanelTrains.Controls)
			{
				tsc.comboBoxCurrentSection.Enabled = !Project.Sections.IsRunning;
			}
		}

		private void toolStripButtonAddSection_Click(object sender, EventArgs e)
		{
			Section newSection = new Section()
			{
				Name = "Section " + Project.Sections.GetAll().Count
			};

			Project.Sections.GetAll().Add(newSection);
			RecreateSectionsUI();
			RefreshTrainProperties();
		}

		private async void toolStripButtonStart_Click(object sender, EventArgs e)
		{
			if (Project.Sections.IsRunning)
			{
				Project.Sections.Stop();

				foreach (Hub h in Project.RegisteredTrains)
				{
					h.IsPathProgramRunning = false;
					if (h.IsTrain())
						await h.ClearAutomatedPathAndStop(false);
					h.RestoreLEDColor();
				}
			}
			else
			{
				foreach (Hub h in Project.RegisteredTrains)
				{
					h.IsWaitingSection = false;
					h.AbortReserve = false;
					h.IsPathProgramRunning = false;

					if (h.IsConnected && h.IsTrain() && !Project.Sections.IsTrainInNetwork(h))
					{
						MessageBox.Show($"{h.Name} does not have a Section assigned.", "Start Is Aborted");
						return;
					}
				}

				foreach (Section s in Project.Sections)
				{
					s.IsBeingCleared = false;
				}

				Project.Sections.Start(Project);
			}

			UpdateStartButtonStatus();
		}

		private void toolStripButtonEditPaths_Click(object sender, EventArgs e)
		{
			PathEditor pathEditor = new PathEditor(Project);
			pathEditor.ShowDialog();

			RefreshTrainProperties();
		}
	}

	public class DetectorItem
	{
		public Hub hub;
		public Port port;

		public DetectorItem(Hub h, Port p)
		{
			hub = h;
			port = p;
		}

		public string GetFullID()
		{
			return port.Id + "*" + hub.DeviceId;
		}

		public override string ToString()
		{
			return hub.Name + " on port " + port.Id;
		}
	}

}
