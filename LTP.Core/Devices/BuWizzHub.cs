using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using static LegoTrainProject.Port;

namespace LegoTrainProject
{
	[Serializable]
    public class BuWizzHub : Hub
    {
		[NonSerialized]
		Timer pingTimer = new Timer();

		public BuWizzHub(BluetoothLEDevice device, Types type) : base (device, type)
		{

		}

		internal override async Task RenewCharacteristic()
		{
			if (Device != null)
			{
				Device = await BluetoothLEDevice.FromBluetoothAddressAsync(Device.BluetoothAddress);
				Gatt = await Device.GetGattServicesAsync(BluetoothCacheMode.Uncached);
				AllCharacteristic = await Gatt.Services.Single(s => s.Uuid == Guid.Parse("4E050000-74FB-4481-88B3-9919B1676E93")).GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
				Characteristic = AllCharacteristic.Characteristics.Single(s => s.Uuid == Guid.Parse("000092d1-0000-1000-8000-00805f9b34fb"));

				MainBoard.WriteLine("New Hub Found of type " + Enum.GetName(typeof(Hub.Types), Type), Color.Green);
			}
		}

		public override void InitPorts()
		{
			// Clear any previous port
			RegistredPorts.Clear();

			Port portA = new Port("A", 0, true);
			Port portB = new Port("B", 2, true);
			Port portC = new Port("C", 1, true);
			Port portD = new Port("D", 3, true);

			RegistredPorts.Add(portA);
			RegistredPorts.Add(portB);
			RegistredPorts.Add(portC);
			RegistredPorts.Add(portD);

			portA.Function = Port.Functions.MOTOR;
			portB.Function = Port.Functions.MOTOR;
			portC.Function = Port.Functions.MOTOR;
			portD.Function = Port.Functions.MOTOR;
		}

		internal override void InitializeNotifications()
		{
			WriteMessage(new byte[] { 0x11, 4 }, false);

			pingTimer.Interval = 150;
			pingTimer.Elapsed += PingTimer_Elapsed; ;
			pingTimer.Start();
		}

		private void PingTimer_Elapsed(object sender, ElapsedEventArgs e)
		{

		}

		/// <summary>
		/// Treat Incoming Data from the train
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		internal override void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
		{
			/*
			try
			{		
				// An Indicate or Notify reported that the value has changed.
				var reader = DataReader.FromBuffer(args.CharacteristicValue);

				byte[] message = new byte[reader.UnconsumedBufferLength];
				reader.ReadBytes(message);
				MainBoard.WriteLine(BitConverter.ToString(message));
			}
			catch (Exception ex)
			{
				MainBoard.WriteLine("FATAL SBRICK: Something went wrong while reading messages!" + ex.Message, Color.DarkRed);
			}
			*/
		}


		protected override void WriteMessage(byte[] message, bool addLength)
		{
			WriteMessage(message, Characteristic);
		}


		protected async void WriteMessage(byte[] message, GattCharacteristic characteristic)
		{
			try
			{
				if (characteristic != null)
				{
					using (DataWriter writer = new DataWriter())
					{
						writer.WriteBytes(message);
						await characteristic.WriteValueAsync(writer.DetachBuffer(), GattWriteOption.WriteWithResponse);
					}
				}
			}
			catch
			{
				MainBoard.WriteLine($"{Name} just lost bluetooth connection. Attempting to reconnect.", Color.Red);

				pingTimer.Stop();
				Dispose();
			}
		}

		public override void SetLEDColor(Colors color)
		{
			// Nothing to do
		}


		public override void SetMotorSpeed(string port, int speed)
		{
			Port portObj = GetPortFromPortId(port);

			// If we can't find the port, we can't do anything!
			if (portObj == null)
			{
				MainBoard.WriteLine("Could not set Motor Speed to " + speed + " for " + Name + " because no default port are setup", Color.Red);
				return;
			}

			portObj.Speed = speed;
			IsBusy = (speed != 0);

			OnDataUpdated();

			byte[] data = new byte[6];
			data[0] = 0x10;
			foreach (Port p in RegistredPorts)
				data[RegistredPorts.IndexOf(p) + 1] = (byte)p.Speed;
			data[5] = 0;

			WriteMessage(data, false);
		}

		public override void SetLightBrightness(string port, int brightness)
		{

			Port portObj = GetPortFromPortId(port);

			// If we can't find the port, we can't do anything!
			if (portObj == null)
			{
				MainBoard.WriteLine("Could not set Light Brightness for " + Name + " because no default port are setup", Color.Red);
				return;
			}

			portObj.Speed = brightness;

			OnDataUpdated();

			byte[] data = new byte[6];
			data[0] = 0x10;
			foreach (Port p in RegistredPorts)
				data[RegistredPorts.IndexOf(p) + 1] = (byte)p.Speed;
			data[5] = 0;

			WriteMessage(data, false);
		}
	}
}
