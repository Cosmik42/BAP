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
    public class SbrickHub : Hub
    {
		[NonSerialized]
		Timer pingTimer = new Timer();

		[NonSerialized]
		GattCharacteristic CharacteristicCommands;

		[NonSerialized]
		private int LastCalibrationTick;

		[NonSerialized]
		private bool CalibrationIsDown;

		[NonSerialized]
		int MaxCurrent = 0;

		public SbrickHub(BluetoothLEDevice device, Types type) : base (device, type)
		{

		}

		internal override async Task RenewCharacteristic()
		{
			if (Device != null)
			{
				Device = await BluetoothLEDevice.FromBluetoothAddressAsync(Device.BluetoothAddress);
				Gatt = await Device.GetGattServicesAsync(BluetoothCacheMode.Uncached);
				AllCharacteristic = await Gatt.Services.Single(s => s.Uuid == Guid.Parse("4dc591b0-857c-41de-b5f1-15abda665b0c")).GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
				Characteristic = AllCharacteristic.Characteristics.Single(c => c.Uuid == Guid.Parse("489a6ae0-c1ab-4c9c-bdb2-11d373c1b7fb"));
				CharacteristicCommands = AllCharacteristic.Characteristics.Single(c => c.Uuid == Guid.Parse("02b8cbcc-0e25-4bda-8790-a15f53e6010f"));

				MainBoard.WriteLine("New Hub Found of type " + Enum.GetName(typeof(Hub.Types), Type), Color.Green);
			}

			// If we reconnect, let's recalibrate sensors
			CalibrationIsDown = false;
			LastCalibrationTick = Environment.TickCount;

			// If we come back, we stop and start over.
			if (pingTimer != null)
			{
				pingTimer.Stop();
				pingTimer.Elapsed -= PingTimer_Elapsed;
			}
			else
				pingTimer = new Timer();

			pingTimer.Interval = 150;
			pingTimer.Elapsed += PingTimer_Elapsed;
			pingTimer.Start();
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

		private void PingTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			WriteMessage(new byte[] { 0x02 }, CharacteristicCommands);

			if (!CalibrationIsDown && Environment.TickCount - LastCalibrationTick > 5000)
			{
				foreach (Port p in RegistredPorts)
				{
					p.MaxDistance = 0;
					p.MinDistance = 0;
				}

				CalibrationIsDown = true;
				LastCalibrationTick = Environment.TickCount;
			}
		}

		internal override void InitializeNotifications()
		{
			WriteMessage(new byte[] { 0x2C, 0x01, 0x03, 0x05, 0x07, 0x08}, CharacteristicCommands);
			WriteMessage(new byte[] { 0x2E, 0x01, 0x03, 0x05, 0x07, 0x08}, CharacteristicCommands);

			foreach (Port p in RegistredPorts)
				Stop(p.Id, true);

			CalibrationIsDown = false;
			LastCalibrationTick = Environment.TickCount;
		}

		/// <summary>
		/// Treat Incoming Data from the train
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		internal override void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
		{
			try
			{
				int numberOfMessage = 0;
				// An Indicate or Notify reported that the value has changed.
				var reader = DataReader.FromBuffer(args.CharacteristicValue);

				while (reader.UnconsumedBufferLength > 0)
				{
					numberOfMessage++;

					// We read the length and the rest of the body
					byte len = reader.ReadByte();
					byte[] message = new byte[len];
					reader.ReadBytes(message);

					//MainBoard.WriteLine(message[0] + " - " + message[1]);

					switch (message[0])
					{
						case 0x04:
							{ 
								ParseStatus(message);
								break;
							};
						case 0x06:
							{
								int portId = 0;

								for (int i = 0; i < message.Length - 1; i += 2 )
								{
									var val = (short)(((message[i + 1] & 0xF0) >> 4) | (message[i + 2] << 4));
									int channel = message[i + 1] & 0x0F;

									if (channel == 8)
									{
										MaxCurrent = (int)(val / 2.85f);
										double battery = (val / 4092f * 100f) * 2.6f;
										BatteryLevel = (int)battery;
										OnDataUpdated();
									}
									else
									{
										portId = (channel == 1) ? 0 : (channel == 3) ? 2 : (channel == 5) ? 1 : 3;
										Port port = RegistredPorts[portId];

										if (port.Function == Port.Functions.SENSOR)
										{
											int distance = 0;

											port.MaxDistance = MaxCurrent; // (val > port.MaxDistance) ? val : port.MaxDistance;
											port.MinDistance = (port.MinDistance == 0) ? port.MaxDistance - 50 : (val < port.MinDistance) ? val : port.MinDistance;

											float distRatio = (float)(val - port.MinDistance) / (float)(port.MaxDistance - port.MinDistance);
											distance = (int)(distRatio * 10f);

											if (MainBoard.showColorDebug)
												MainBoard.WriteLine($"{Name} (Port {port.Id} - Distance: " + distance + $" (Val: {val} Min: {port.MinDistance} Max: {port.MaxDistance} - last triggers was " + (Environment.TickCount - port.LastDistanceTick));

											if (Environment.TickCount - port.LastDistanceTick > port.DistanceColorCooldownMs)
											{
												port.LatestDistance = (int)distance;
												OnDistanceTriggered(this, port, (int)distance);
												OnDataUpdated();
											}
										}

										portId++;
									}
								}

								break;
							}
					}
				}
			}
			catch (Exception ex)
			{
				MainBoard.WriteLine("FATAL SBRICK: Something went wrong while reading messages!" + ex.Message, Color.DarkRed);
			}
		}

		private static void ParseStatus(byte[] message)
		{
			switch (message[1])
			{
				case 0x00:
					{
						//MainBoard.WriteLine("SBRICK - ACK - ");
						break;
					}
				case 0x01:
					{
						MainBoard.WriteLine("SBRICK - Invalid Data Length - ");
						break;
					}
				case 0x02:
					{
						MainBoard.WriteLine("SBRICK - Invalid Parameter - ");
						break;
					}
				case 0x03:
					{
						MainBoard.WriteLine("SBRICK - No Such Command - ");
						break;
					}
				case 0x04:
					{
						MainBoard.WriteLine("SBRICK - No authentication needed - ");
						break;
					}
				case 0x05:
					{
						MainBoard.WriteLine("SBRICK - Authentication error - ");
						break;
					}
				case 0x06:
					{
						MainBoard.WriteLine("SBRICK - Authentication needed - ");
						break;
					}
				case 0x08:
					{
						MainBoard.WriteLine("SBRICK - Thermal protection is active - ");
						break;
					}
				case 0x09:
					{
						MainBoard.WriteLine("SBRICK - The system is in a state where the command does not make sense - ");
						break;
					}
			}
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
						await characteristic.WriteValueAsync(writer.DetachBuffer(), GattWriteOption.WriteWithoutResponse);
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
			WriteMessage(new byte[] { 0x01, (byte)portObj.Value, (byte)((speed > 0) ? 0 : 1), (byte)Math.Abs(portObj.Speed * 2.5) }, CharacteristicCommands);
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
			WriteMessage(new byte[] { 0x01, (byte)portObj.Value, 1, (byte)(portObj.Speed * 2.5) }, CharacteristicCommands);
		}
	}
}
