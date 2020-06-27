using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace LegoTrainProject.LTP.Core.Devices
{
	[Serializable]
	public class ConnectionLimitationSettings
	{
		public enum LimitationType
		{
			None = 0,
			OnlyProject = 1,
			OnlySetList = 2
		}

		public LimitationType currentLimitation = 0;
		public string setListOfDevices = "90842B04349A" + Environment.NewLine + "78842B043412";

		internal bool IsMacAddressAllowed(ulong bluetoothAddress, TrainProject project)
		{
			if (currentLimitation == LimitationType.None)
				return true;
			else if (currentLimitation == LimitationType.OnlyProject)
			{
				foreach (Hub hub in project.RegisteredTrains)
				{
					if (hub.BluetoothAddress == bluetoothAddress)
						return true;
				}
			}
			else
			{
				List<string> listStrLineElements = setListOfDevices.Split(
								new[] { "\r\n", "\r", "\n" },
								StringSplitOptions.None).ToList();
				string macAddress = string.Format("{0:X}", bluetoothAddress);

				foreach (string address in listStrLineElements)
				{
					if (address.Trim(' ') == macAddress)
						return true;
				}
			}

			return false;
		}

		public static ConnectionLimitationSettings Load(string path)
		{
			try
			{
				if (File.Exists(path))
				{
					IFormatter formatter = new BinaryFormatter();
					using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
					{
						ConnectionLimitationSettings cls = (ConnectionLimitationSettings)formatter.Deserialize(stream);
						stream.Close();

						MainBoard.WriteLine("Global connection preferences loaded!", System.Drawing.Color.Green);
						return cls;
					}
				}

				MainBoard.WriteLine("No global connection preferences file exist. Creating default ones.");
				return new ConnectionLimitationSettings();
			}
			catch (Exception ex)
			{
				MainBoard.WriteLine("ERROR - Could not open file: " + ex.Message, System.Drawing.Color.Red);
				return null;
			}

		}

		public void SaveAs(string path)
		{
			try
			{
				IFormatter formatter = new BinaryFormatter();
				using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
				{
					formatter.Serialize(stream, this);
					stream.Close();
				}
			}
			catch (Exception ex)
			{
				MainBoard.WriteLine("ERROR - Could not save file: " + ex.Message, System.Drawing.Color.Red);
			}
		}

	}
}
