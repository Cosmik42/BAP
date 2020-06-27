using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using static LegoTrainProject.Hub;

namespace LegoTrainProject
{
	[Serializable]
	class RemoteHub : Hub
	{
		public RemoteHub(BluetoothLEDevice device, Types type) : base(device, type)
		{

		}

		public override void InitPorts()
		{
			// Clear any previous port
			RegistredPorts.Clear();

			Port portA = new Port("A", 0, true);
			Port portB = new Port("B", 1, true);

			RegistredPorts.Add(portA);
			RegistredPorts.Add(portB);

			portA.Function = Port.Functions.BUTTON;
			portB.Function = Port.Functions.BUTTON;
		}

		public override void SetLEDColor(Port.Colors color)
		{
			WriteMessage(new byte[] { 0x41, 0x34, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 });
			WriteMessage(new byte[] { 0x81, 0x34, 0x11, 0x51, 0x00, (byte)color });
		}
    }
}
