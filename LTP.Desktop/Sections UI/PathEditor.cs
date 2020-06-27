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

	public partial class PathEditor : Form
	{
		private TrainProject project;
		private Path currentPath;

		public PathEditor(TrainProject project)
		{
			this.project = project;
			InitializeComponent();
			RefreshUI();
		}

		private void toolStripButtonAddPaths_Click(object sender, EventArgs e)
		{
			Path newPath = new Path();
			project.Sections.Paths.Add(newPath);

			RefreshUI();
		}

		private void RefreshUI()
		{
			int previouslySelected = listBoxPaths.SelectedIndex;
			listBoxPaths.Items.Clear();
			foreach (Path p in project.Sections.Paths)
			{
				listBoxPaths.Items.Add(p.Name + ": " + p.SectionsToString());
			}

			listBoxPaths.SelectedIndex = previouslySelected;
		}

		private void listBoxPaths_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBoxPaths.SelectedIndex >= 0)
			{
				currentPath = project.Sections.Paths[listBoxPaths.SelectedIndex];
			}

			if (currentPath != null)
			{
				textBoxName.Text = currentPath.Name;
				textBoxName.Enabled = true;
				textBoxPath.Text = currentPath.SectionsToString();
				textBoxPath.Enabled = true;
				buttonSave.Enabled = true;
			}
			else
			{
				textBoxName.Enabled = false;
				textBoxPath.Enabled = false;
				buttonSave.Enabled = false;
			}
		}

		private void textBoxPath_TextChanged(object sender, EventArgs e)
		{
			Path validationPath = new Path();
			if (!validationPath.FromString(textBoxPath.Text))
				textBoxPath.ForeColor = Color.Red;
			else
				textBoxPath.ForeColor = Color.Black;
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			if (currentPath != null)
			{
				currentPath.Name = textBoxName.Text;
				currentPath.FromString(textBoxPath.Text);
			}

			RefreshUI();
		}

		private void toolStripButtonDeletePath_Click(object sender, EventArgs e)
		{
			if (listBoxPaths.SelectedIndex >= 0)
			{
				project.Sections.Paths.RemoveAt(listBoxPaths.SelectedIndex);
				listBoxPaths.Items.RemoveAt(listBoxPaths.SelectedIndex);
			}
		}
	}
}
