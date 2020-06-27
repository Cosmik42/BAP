using LegoTrainProject.Main_UI;
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
	public partial class SplashForm : Form
	{
		public SplashForm()
		{
			InitializeComponent();

			this.BackColor = Color.FromArgb(5, 50, 86);
			pictureBox1.BackColor = Color.FromArgb(5, 50, 86);

			Timer t = new Timer();
			t.Interval = 1000;
			t.Tick += (e, f) =>
			{
				t.Stop();
				this.Hide();
				FormWelcome fw = new FormWelcome();
				fw.ShowDialog();
			};
			t.Start();
		}
	}
}
