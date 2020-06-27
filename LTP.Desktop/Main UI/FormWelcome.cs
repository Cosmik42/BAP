using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegoTrainProject.Main_UI
{
	public partial class FormWelcome : Form
	{
		public FormWelcome()
		{
			InitializeComponent();
		}

		private void buttonDonate_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.paypal.me/vincentvergonjeanne");
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{

		}
	}
}
