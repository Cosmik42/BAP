using LegoTrainProject.LTP.Core.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegoTrainProject.LTP.Desktop.Main_UI
{
	public partial class ConnectionLimit : Form
	{
		private ConnectionLimitationSettings _connectionLimitationSettings;

		public ConnectionLimit(ConnectionLimitationSettings connectionLimitationSettings)
		{
			_connectionLimitationSettings = connectionLimitationSettings;

			InitializeComponent();

			richTextBoxDevices.Text = connectionLimitationSettings.setListOfDevices;
			if (connectionLimitationSettings.currentLimitation == ConnectionLimitationSettings.LimitationType.None)
				radioButton1.Checked = true;
			else if (connectionLimitationSettings.currentLimitation == ConnectionLimitationSettings.LimitationType.OnlyProject)
				radioButton2.Checked = true;
			else if (connectionLimitationSettings.currentLimitation == ConnectionLimitationSettings.LimitationType.OnlySetList)
				radioButton3.Checked = true;
		}

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton1.Checked)
				_connectionLimitationSettings.currentLimitation = ConnectionLimitationSettings.LimitationType.None;
		}

		private void radioButton2_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton2.Checked)
				_connectionLimitationSettings.currentLimitation = ConnectionLimitationSettings.LimitationType.OnlyProject;
		}

		private void radioButton3_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton3.Checked)
				_connectionLimitationSettings.currentLimitation = ConnectionLimitationSettings.LimitationType.OnlySetList;
		}

		private void richTextBoxDevices_TextChanged(object sender, EventArgs e)
		{
			_connectionLimitationSettings.setListOfDevices = richTextBoxDevices.Text;
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			_connectionLimitationSettings.SaveAs(AppDomain.CurrentDomain.BaseDirectory + "connectionSettings.bap");
			Close();
		}
	}
}
