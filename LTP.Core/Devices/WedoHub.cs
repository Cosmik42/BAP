using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using static LegoTrainProject.Port;

namespace LegoTrainProject
{
	[Serializable]
    public class WedoHub : Hub
	{ 
		[NonSerialized]
		GattCharacteristic CharacteristicPort;
		[NonSerialized]
		GattCharacteristic CharacteristicSensor;
		[NonSerialized]
		GattCharacteristic CharacteristicButton;
		[NonSerialized]
		GattCharacteristic CharacteristicBattery;
		[NonSerialized]
		GattCharacteristic CharacteristicPortWrite;
		[NonSerialized]
		GattCharacteristic CharacteristicDisconnect;

		private const string WEDO2_BATTERY = "00002A19-0000-1000-8000-00805F9B34FB";
		private const string WEDO2_BUTTON = "00001526-1212-efde-1523-785feabcd123"; // "1526"
		private const string WEDO2_PORT_TYPE = "00001527-1212-efde-1523-785feabcd123"; // "1527" // Handles plugging and unplugging of devices on WeDo 2.0 Smart Hub
		private const string WEDO2_DISCONNECT = "0000152B-1212-efde-1523-785feabcd123"; // "152B"

		private const string WEDO2_SENSOR_VALUE = "00001560-1212-efde-1523-785feabcd123"; // "1560"
		private const string WEDO2_VALUE_FORMAT = "00001561-1212-efde-1523-785feabcd123"; // "1561"
		private const string WEDO2_PORT_TYPE_WRITE = "00001563-1212-efde-1523-785feabcd123"; // "1563"
		private const string WEDO2_MOTOR_VALUE_WRITE = "00001565-1212-efde-1523-785feabcd123"; // "1565"


		private const string WEDO2_NAME_ID = "00001524-1212-efde-1523-785feabcd123"; // "1524"
		private const string LPF2_ALL = "00001624-1212-efde-1623-785feabcd123";

		public WedoHub(BluetoothLEDevice device, Types type) : base (device, type)
		{

		}

		internal override async Task RenewCharacteristic()
		{
			if (Device != null)
			{
				Device = await BluetoothLEDevice.FromBluetoothAddressAsync(Device.BluetoothAddress);
				Gatt = await Device.GetGattServicesAsync(BluetoothCacheMode.Uncached);

				AllCharacteristic = await Gatt.Services.Single(s => s.Uuid == Guid.Parse("00001523-1212-efde-1523-785feabcd123")).GetCharacteristicsAsync(BluetoothCacheMode.Uncached);

				CharacteristicPort = AllCharacteristic.Characteristics.Single(c => c.Uuid == Guid.Parse(WEDO2_PORT_TYPE));
				CharacteristicButton = AllCharacteristic.Characteristics.Single(c => c.Uuid == Guid.Parse(WEDO2_BUTTON));
				CharacteristicDisconnect = AllCharacteristic.Characteristics.Single(c => c.Uuid == Guid.Parse(WEDO2_DISCONNECT));

				AllCharacteristic = await Gatt.Services.Single(s => s.Uuid == Guid.Parse("00004f0e-1212-EFDE-1523-785FEABCD123")).GetCharacteristicsAsync(BluetoothCacheMode.Uncached);

				CharacteristicSensor = AllCharacteristic.Characteristics.Single(c => c.Uuid == Guid.Parse(WEDO2_SENSOR_VALUE));
				CharacteristicPortWrite = AllCharacteristic.Characteristics.Single(c => c.Uuid == Guid.Parse(WEDO2_PORT_TYPE_WRITE));
				Characteristic = AllCharacteristic.Characteristics.Single(c => c.Uuid == Guid.Parse(WEDO2_MOTOR_VALUE_WRITE));

				AllCharacteristic = await Gatt.Services.Single(s => s.Uuid == Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB")).GetCharacteristicsAsync(BluetoothCacheMode.Uncached);

				CharacteristicBattery = AllCharacteristic.Characteristics.Single(c => c.Uuid == Guid.Parse(WEDO2_BATTERY));

				MainBoard.WriteLine("New Hub Found of type " + Enum.GetName(typeof(Hub.Types), Type), Color.Green);
			}

		}

		public override void InitPorts()
		{
			// Clear any previous port
			RegistredPorts.Clear();

			Port portA = new Port("A", 1, true);
			Port portB = new Port("B", 2, true);

			RegistredPorts.Add(portA);
			RegistredPorts.Add(portB);
		}

		protected override void ActivatePortDevice(byte port, byte type, byte mode, byte format)
		{
			WriteMessage(new byte[] { 0x01, 0x02, port, type, mode, 0x01, 0x00, 0x00, 0x00, format, 0x01 }, CharacteristicPortWrite);
		}

		internal override void InitializeNotifications()
		{
			//ActivatePortDevice(0x03, 0x15, 0x00, 0x00); // Activate voltage reports
			//ActivatePortDevice(0x04, 0x14, 0x00, 0x00); // Activate current reports
		}

		/// <summary>
		/// Enable Notifications from Train Characteristics
		/// </summary>
		public async override void StartListening(BluetoothLEDevice device)
		{
			try
			{

				// Assign the device to this hub
				Device = device;

				// Save the bluetooth address for reconnection purpose
				BluetoothAddress = device.BluetoothAddress;

				// Obtain a fresh Characteristics
				await RenewCharacteristic();

				// If it succeeded
				if (Characteristic != null)
				{
					// Immediately attach to get all data from the Hub
					CharacteristicPort.ValueChanged += CharacteristicPort_ValueChanged;
					CharacteristicSensor.ValueChanged += CharacteristicSensor_ValueChanged; 
					CharacteristicButton.ValueChanged += CharacteristicButton_ValueChanged; 
					CharacteristicBattery.ValueChanged += CharacteristicBattery_ValueChanged;

					// Ask for notifications
					GattCommunicationStatus statusPort = await CharacteristicPort.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
					GattCommunicationStatus statusSensor = await CharacteristicSensor.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
					GattCommunicationStatus statusButton = await CharacteristicButton.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
					GattCommunicationStatus statusBattery = await CharacteristicBattery.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

					if (statusPort == GattCommunicationStatus.Success &&
						statusSensor == GattCommunicationStatus.Success &&
						statusButton == GattCommunicationStatus.Success &&
						statusBattery == GattCommunicationStatus.Success)
					{
						Thread.Sleep(1000);

						InitializeNotifications();
						IsConnected = true;
						MainBoard.WriteLine($"Hub {Name} is connected!", Color.Green);

						RestoreLEDColor();
					}
					else
						MainBoard.WriteLine("Characteristic Unreachable!", Color.Red);
				}
			}
			catch (Exception ex)
			{
				MainBoard.WriteLine("FATAL ERROR while trying to connect to a train", Color.Red);
				MainBoard.WriteLine("Exception: " + ex.Message, Color.Red);

				Device = null;
			}
		}

		private void CharacteristicBattery_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
		{
			var reader = DataReader.FromBuffer(args.CharacteristicValue);
			byte[] data = new byte[reader.UnconsumedBufferLength];
			reader.ReadBytes(data);

			this.BatteryLevel = data[0];
		}

		private void CharacteristicButton_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
		{
			//throw new NotImplementedException();
		}

		private void CharacteristicSensor_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
		{

			var reader = DataReader.FromBuffer(args.CharacteristicValue);
			byte[] data = new byte[reader.UnconsumedBufferLength];
			reader.ReadBytes(data);

			if (data[0] == 0x01)
			{
				/**
				 * Emits when a button is pressed.
				 * @event WeDo2SmartHub#button
				 * @param {string} button
				 * @param {ButtonState} state
				 */
				OnRemoteTriggered(this, null, RemoteButtons.BUTTON_POWER);
				return;
			}

			Port port = this.GetPortFromPortNumber(data[1]);

			if (port == null)
			{
				return;
			}

			if (port.Connected)
			{
				switch (port.Device)
				{
					case Devices.WEDO2_DISTANCE:
						{
							int distance = data[2];
							if (data.Length > 3 && data[3] == 1)
							{
								distance = data[2] + 255;
							}

							if (MainBoard.showColorDebug)
								MainBoard.WriteLine($"{Name} - Distance Received: " + distance + " last triggers was " + (Environment.TickCount - port.LastDistanceTick));

							port.LatestDistance = (int)distance;

							if (Environment.TickCount - port.LastDistanceTick > port.DistanceColorCooldownMs)
							{
								OnDistanceTriggered(this, port, (int)distance);
								OnDataUpdated();
							}

							break;
						}

					case Devices.BOOST_DISTANCE:
						{
							int distance = data[2];

							if (MainBoard.showColorDebug)
								MainBoard.WriteLine($"{Name} - Distance Received: " + distance + " last triggers was " + (Environment.TickCount - port.LastDistanceTick));

							port.LatestDistance = (int)distance;

							if (Environment.TickCount - port.LastDistanceTick > port.DistanceColorCooldownMs)
							{
								OnDistanceTriggered(this, port, (int)distance);
								OnDataUpdated();
							}

							break;
						}
				}
			}

		}

		private void CharacteristicPort_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
		{
			var reader = DataReader.FromBuffer(args.CharacteristicValue);
			byte[] data = new byte[reader.UnconsumedBufferLength];
			reader.ReadBytes(data);

			Port port = this.GetPortFromPortNumber(data[0]);

			if (port == null)
			{
				//MainBoard.WriteLine("Could not find port");
				return;
			}

			port.Connected = (data[1] == 1);
			Port.Devices device = (data.Length > 3) ? (Port.Devices)data[3] : Port.Devices.UNKNOWN;

			RegisterDeviceAttachement(port, device);
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
										double battery = (val / 4092f * 100f) * 2.6f;
										BatteryLevel = (int)battery;
										OnDataUpdated();
									}
									else
									{
										Port port = RegistredPorts[portId];

										//MainBoard.WriteLine($"Channel {channel} " + val.ToString("X"));

										if (port.Function == Port.Functions.SENSOR)
										{
											int distance = 0;

											// Adjust to battery level
											//MainBoard.WriteLine("Val " + val);
											//val = (short)(val / (BatteryLevel / 50.0f));
											//MainBoard.WriteLine("Val2 " + val);

											val = (short)((val < 230) ? 230 : (val > 380) ? 380 : val);
											distance = (int)((val - 220) / 150f * 10f);

											if (MainBoard.showColorDebug)
												MainBoard.WriteLine($"{Name} - Distance Received: " + distance + " last triggers was " + (Environment.TickCount - port.LastDistanceTick));

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
				Dispose();
			}
		}

		public override void Disconnect()
		{
			WriteMessage(new byte[0], CharacteristicDisconnect);
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

			WriteMessage(new byte[] { (byte)portObj.Value, 0x01, 0x02, (byte)portObj.Speed });
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

			byte[] data = new byte[] { (byte)portObj.Value, 0x01, 0x02, (byte)brightness };
			WriteMessage(data);
		}

		public override void SetLEDColor(Colors color)
		{
			byte[] data = new byte[] { 0x06, 0x17, 0x01, 0x01 };
			WriteMessage(data, CharacteristicPortWrite);

			data = new byte[] { 0x06, 0x04, 0x01, (byte)color };
			WriteMessage(data);
		}
	}
}
