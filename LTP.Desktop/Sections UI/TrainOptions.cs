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
	public partial class TrainOptions : Form
	{
		Hub train;

		public TrainOptions(Hub h)
		{
			train = h;
			InitializeComponent();

			textBoxClearing.Text = h.ClearingTimeInMs.ToString();
			textBoxSpeed.Text = h.SpeedWhenAboutToStop.ToString();
			textBoxCoefficient.Text = h.SpeedCoefficient.ToString("0.0");
		}

		private void textBoxCoefficient_TextChanged(object sender, EventArgs e)
		{
			float.TryParse(textBoxCoefficient.Text, out train.SpeedCoefficient);
		}

		private void textBoxSpeed_TextChanged(object sender, EventArgs e)
		{
			Int32.TryParse(textBoxSpeed.Text, out train.SpeedWhenAboutToStop);
		}

		private void textBoxClearing_TextChanged(object sender, EventArgs e)
		{
			Int32.TryParse(textBoxClearing.Text, out train.ClearingTimeInMs);
		}
	}
}
