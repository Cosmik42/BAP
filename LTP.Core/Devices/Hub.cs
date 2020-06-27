using System.Drawing;
using Windows.Devices.Bluetooth;
using System.IO;
using System;
using System.Collections.Generic;
using System.Timers;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Threading;
using static LegoTrainProject.Port;
using System.Threading.Tasks;

namespace LegoTrainProject
{
    [Serializable]
    public class Hub
    {
        // List of Services
        [NonSerialized]
        internal GattDeviceServicesResult Gatt;

        // List of Characteristics
        [NonSerialized]
		internal GattCharacteristicsResult AllCharacteristic;

        // The Device of the train
        [NonSerialized]
        internal BluetoothLEDevice Device;

        /// <summary>
        /// Characteristic for this train
        /// </summary>
        [NonSerialized]
		internal GattCharacteristic Characteristic;
		

        /// <summary>
        /// Train's Name
        /// </summary>
        public string Name = "";
        public string WhenName { get { return $"When {Name}"; } }
        public string OfName { get { return $"of {Name}"; } }

        /// <summary>
        /// Train's Device ID
        /// </summary>
        public string DeviceId = "";

		/// <summary>
		/// The Bluetooth Address
		/// </summary>
		public ulong BluetoothAddress;

		/// <summary>
		/// Is this train connected?
		/// </summary>
		[NonSerialized]
        public bool IsConnected = false;

		/// <summary>
		/// Used only for custom programs
		/// </summary>
		[NonSerialized]
		public bool IsBusy = false;

        /// <summary>
        /// Battery Level
        /// </summary>
        [NonSerialized]
        public double BatteryLevel = 0;

		/// <summary>
		/// Battery Level
		/// </summary>
		[NonSerialized]
		public double BatteryVoltage = 0;


        /// <summary>
        /// List of all Ports Connected
        /// </summary>
        public List<Port> RegistredPorts = new List<Port>();

		/// <summary>
		/// 
		/// </summary>
		[NonSerialized]
		public int[] State = new int[100];

		public enum MotorPorts
        {
            A = 0,
            B = 1,
            C = 2,
            D = 3,
			One = 4,
			Two = 5,
			Three = 6,
			Four = 7
        }

		public enum HubManufacturerID
		{
			UNKNOWN = 0,
			BOOST_MOVE_HUB = 64,
			POWERED_UP_HUB = 65,
			POWERED_UP_REMOTE = 66,
			SBRIK = 1000,
			WEDO = 1001,
			PFX = 1002,
			BUWIZZ = 1003
		}

		public enum Types
		{
			BOOST_MOVE_HUB = 0,
			POWERED_UP_HUB = 1,
			SBRICK = 2,
			WEDO_2_SMART_HUB = 3,
			EV3 = 4,
			POWERED_UP_REMOTE = 5,
			PFX = 6,
			BUWIZZ = 7
		}

		/// <summary>
		/// Hardware type of the Hub
		/// </summary>
		public Types Type;

		[NonSerialized]
		internal bool IsWaitingSection = false;

		public string TrainMotorPort = null;

		public int ClearingTimeInMs = 2000;

		public int SpeedWhenAboutToStop = 40;

		public float SpeedCoefficient = 1.0f;

		public bool LoopCurrentPath = false;

		public Path CurrentPath;

		public Colors LEDColor = Colors.GREEN;

		[NonSerialized]
		public bool IsPathProgramRunning;

		[NonSerialized]
		public int CurrentPathPositionIndex;

		[NonSerialized]
		internal bool AbortReserve = false;

		[NonSerialized]
		public const string WEDO2_SMART_HUB = "00001523-1212-efde-1523-785feabcd123";

		[NonSerialized]
		public const string LPF2_HUB = "00001623-1212-efde-1623-785feabcd123";

        public delegate void ColorTriggeredHandler(Hub train, Port p, Colors color);
        public delegate void DistanceTriggeredHandler(Hub train, Port p, int distance);
		public delegate void RemoteTriggeredHandler(Hub train, Port p, RemoteButtons button);
		public delegate void RefreshUIThreadSafeDelegate();

		internal bool IsTrain()
		{
			foreach (Port p in RegistredPorts)
				if (p.Function == Functions.TRAIN_MOTOR)
					return true;

			return false;
		}

		[field: NonSerialized]
		public event ColorTriggeredHandler ColorTriggered;
		[field: NonSerialized]
		public event DistanceTriggeredHandler DistanceTriggered;
		[field: NonSerialized]
		public event RefreshUIThreadSafeDelegate PortTypeUpdated;
		[field: NonSerialized]
		public event RefreshUIThreadSafeDelegate DataUpdated;
		[field: NonSerialized]
		public event RemoteTriggeredHandler RemoteTriggered;

		internal void CleanAllEvents()
        {
            ColorTriggered = null;
            DistanceTriggered = null;
			RemoteTriggered = null;
        }

		internal void OnDataUpdated()
		{
			DataUpdated?.Invoke();
		}

		internal void OnDistanceTriggered(Hub hub, Port port, int distance)
		{
			DistanceTriggered?.Invoke(hub, port, distance);
		}

		internal void OnColorTriggered(Hub hub, Port p, Colors color)
		{
			ColorTriggered?.Invoke(hub, p, color);
		}

		internal void OnRemoteTriggered(Hub hub, Port p, RemoteButtons button)
		{
			RemoteTriggered?.Invoke(hub, p, button);
		}

		public void OnPortTypeUpdate()
		{
			// Let HubControl know that UI needs update
			if (PortTypeUpdated != null)
				PortTypeUpdated();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="characteristicToRegister"></param>
		public Hub(BluetoothLEDevice device, Types type)
		{
			if (device != null)
			{
				Name = device.Name;
				DeviceId = device.DeviceId;

				Type = type;
				IsConnected = false;

				InitPorts();
				StartListening(device);
			}
		}

		public virtual void InitPorts()
		{
			// Clear any previous port
			RegistredPorts.Clear();

			if (Type == Types.BOOST_MOVE_HUB)
			{
					// Establish firmware version 1.0.00.0224 as a 32-bit signed integer (little endian)
				//	const fwVersion10000224 = int32ArrayToNumber([0x24, 0x02, 0x00, 0x10]);
				//	const fwHub = int32ArrayToNumber(data.slice(5, data.length));

				//if (fwHub < fwVersion10000224)
				//	{
				//		BoostPort = BoostPort10000223OrOlder;
				//		log.info('Move Hub firmware older than version 1.0.00.0224 detected. Using old port mapping.');
				//	}
				//	else
				//	{
				//		BoostPort = BoostPort10000224OrNewer;
				//	}
				//	break;
				//}

				Port portA = new Port("A", 55, true);
				Port portB = new Port("B", 56, true);
				Port portC = new Port("C", 1, true);
				Port portD = new Port("D", 2, true);

				RegistredPorts.Add(portA);
				RegistredPorts.Add(portB);
				RegistredPorts.Add(portC);
				RegistredPorts.Add(portD);
			}
			else
			{
				RegistredPorts.Add(new Port("A", 0));
				RegistredPorts.Add(new Port("B", 1));
				RegistredPorts.Add(new Port("C", 2));
				RegistredPorts.Add(new Port("D", 3));
			}
		}







		//////////////////////////////////////////
		////
		////
		////
		////
		//// Low Level Communication Functions 
		////
		////
		////
		////
		//////////////////////////////////////////








		/// <summary>
		/// Enable Notifications from Train Characteristics
		/// </summary>
		public async virtual void StartListening(BluetoothLEDevice device)
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
                    Thread.Sleep(1000);

					// Immediately attach to get all data from the Hub
					Characteristic.ValueChanged += Characteristic_ValueChanged;
					
					// Ask for notifications
					GattCommunicationStatus status = await Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
					if (status == GattCommunicationStatus.Success)
					{
						InitializeNotifications();
						IsConnected = true;
						MainBoard.WriteLine($"Hub {Name} is connected!", Color.Green);

						RestoreLEDColor();
					}
					else
					{
						MainBoard.WriteLine("Characteristic Unreachable!", Color.Red);
						Dispose();
					}
                }
            }
            catch (Exception ex)
            {
				MainBoard.WriteLine("FATAL ERROR while trying to connect to a train in StartListening", Color.Red);
				MainBoard.WriteLine("Exception: " + ex.Message, Color.Red);
				Dispose();
			}


			OnDataUpdated();
		}

		internal virtual void InitializeNotifications()
		{
			WriteMessage(new byte[] { 0x01, 0x03, 0x05 }); // Request Firmware Version
			WriteMessage(new byte[] { 0x01, 0x02, 0x02 }); // Activate button reports
			WriteMessage(new byte[] { 0x01, 0x06, 0x02 }); // Activate button reports
			WriteMessage(new byte[] { 0x41, 0x3c, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01 }); // Activate voltage reports
		}

		/// <summary>
		/// Obtain a new Characteristic
		/// </summary>
		/// <param name="device"></param>
		/// <returns></returns>
		internal virtual async System.Threading.Tasks.Task RenewCharacteristic()
        {
			if (Device != null)
			{
				Device = await BluetoothLEDevice.FromBluetoothAddressAsync(Device.BluetoothAddress);
				Gatt = await Device.GetGattServicesAsync(BluetoothCacheMode.Uncached);
				AllCharacteristic = await Gatt.Services.Single(s => s.Uuid == Guid.Parse("00001623-1212-efde-1623-785feabcd123")).GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
				Characteristic = AllCharacteristic.Characteristics.Single(c => c.Uuid == Guid.Parse("00001624-1212-efde-1623-785feabcd123"));

				MainBoard.WriteLine("New Hub Found of type " + Enum.GetName(typeof(Hub.Types), Type), Color.Green);
			}
		}


        /// <summary>
        /// Treat Incoming Data from the train
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        internal virtual void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
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
                    byte[] toread = new byte[len - 1];
                    reader.ReadBytes(toread);

                    // Ajusting so that the message is properly sized
                    byte[] message = new byte[len];
                    System.Buffer.BlockCopy(toread, 0, message, 1, toread.Length);

                    switch (message[2])
                    {
                        case 0x01:
                            {
                                this.ParseDeviceInfo(message);
                                break;
                            }
                        case 0x04:
                            {
                                this.ParsePortMessage(message);
                                break;
                            }
                        case 0x45:
                            {
                                this.ParseSensorMessage(message);
                                break;
                            }
                        case 0x82:
                            {
                                this.ParsePortAction(message);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                MainBoard.WriteLine("FATAL: Something went wrong while reading messages!" + Environment.NewLine + ex.Message, Color.DarkRed);
            }
        }

        private void ParseDeviceInfo(byte [] data)
        {
            //MainBoard.WriteLine("Device Info Message received: " + BitConverter.ToString(data));

            if (data[3] == 2)
            {
                if (data[5] == 1)
                {
					MainBoard.WriteLine("Power Button was pressed");
					OnRemoteTriggered(this, null, RemoteButtons.BUTTON_POWER);
					return;
                }
            }
			// Firmware version
			else if (data[3] == 3)
			{
				int fwVersion10000224 = BitConverter.ToInt32(new byte[] { 0x24, 0x02, 0x00, 0x10 }, 0);
				int currentVersion = BitConverter.ToInt32(data, 5);

				if (currentVersion > fwVersion10000224 && Type == Types.BOOST_MOVE_HUB)
				{
					UpdateBoostMovePortToLatestFirmware();

					OnPortTypeUpdate();
					OnDataUpdated();
				}

			}
			else if (data[3] == 0x06)
			{
				BatteryLevel = data[5];
				OnDataUpdated();
			}
		}

		private void UpdateBoostMovePortToLatestFirmware()
		{
			RegistredPorts[0].Value = 0;
			RegistredPorts[1].Value = 1;
			RegistredPorts[2].Value = 2;
			RegistredPorts[3].Value = 3;
		}

		private void ParsePortMessage(byte [] data)
        {
            try
            {
				// A port 0 and a BoostMove? It's the latest firmware. Fix ports
				if (Type == Types.BOOST_MOVE_HUB && data[3] == 0)
				{
					UpdateBoostMovePortToLatestFirmware();
				}

				Port port = GetPortFromPortNumber(data[3]);

                if (port == null)
                {
                    return;
                }

                port.Connected = (data[4] == 1 || data[4] == 2) ? true : false;
                RegisterDeviceAttachement(port, (data.Length < 6) ? Port.Devices.UNKNOWN : (Port.Devices)data[5]);
            }
            catch (Exception ex)
            {
                MainBoard.WriteLine("ERROR in Port Message: " + ex.Message, Color.Red);
            }

        }


        private void ParsePortAction(byte [] data)
        {
            //Debug.Write("Port Action received: ");
            Port port = GetPortFromPortNumber(data[3]);

            if (port != null && data[4] == 0x0a)
            {
                //MainBoard.WriteLine("Message un-parsed 0x0a received", Color.Purple);
            }
        }

        private byte[] PadMessage(byte [] data, int length)
        {
            if (data.Length < length)
            {
                byte[] rv = new byte[length];
                System.Buffer.BlockCopy(data, 0, rv, 0, data.Length);
                return rv;
            }
                
            return data;
        }


        private void ParseSensorMessage(byte[] data)
        {
			if (data[3] == 0x3b && (Type == Types.POWERED_UP_REMOTE || this.GetType() == typeof(RemoteHub)))
			{ 
				// Voltage (PUP Remote)
				data = PadMessage(data, 6);
				BatteryVoltage = ((double)BitConverter.ToUInt16(data, 4) / 500);
				return;
			}
			else if (data[3] == 0x3c)
            { 
				// Voltage
                data = PadMessage(data, 6);
				BatteryVoltage = ((double)BitConverter.ToUInt16(data, 4) / 400);
				double batteryRaw = (double)BitConverter.ToUInt16(data, 4);
				return;
            }

            Port port = GetPortFromPortNumber(data[3]);

            if (port != null && port.Connected)
            {
                switch (port.Device)
                {
                    case Port.Devices.WEDO2_DISTANCE:
                    {
                        int distance = data[4];
                        if (data.Length > 5 && data[5] == 1)
                        {
                            distance = data[4] + 255;
                        }

						if (MainBoard.showColorDebug)
							MainBoard.WriteLine($"{Name} - Distance Received: " + distance + " last triggers was " + (Environment.TickCount - port.LastDistanceTick));

						port.LatestDistance = (int)distance;

						if (Environment.TickCount - port.LastDistanceTick > port.DistanceColorCooldownMs)
						{
							DistanceTriggered?.Invoke(this, port, (int)distance);
							OnDataUpdated();
						}
                        
                        break;
                    }
                    case Port.Devices.BOOST_DISTANCE:
                    {

                            if (data.Length >= 8)
                            {
                                double distance = data[5] * 1.5f;
                                double partial = data[7];
                                double dis2 = distance;

                                if (partial > 0)
                                {
                                    dis2 += (1.0 / partial);
                                }

								port.LatestDistance = (int)distance;

								// Trigger a distance event if at least 2 seconds have passed
								if (Environment.TickCount - port.LastDistanceTick > port.DistanceColorCooldownMs)
                                {
                                    DistanceTriggered?.Invoke(this, port, (int)distance);
                                }

                                // Should we show any debugs?
                                if (MainBoard.showColorDebug)
                                    MainBoard.WriteLine($"{Name} - Distance: " + distance + " Color: " + data[4] + "(" + (Colors)data[4] + ") -> Partial: " + partial, (partial > 5) ? Color.Green : Color.Black);

                                // We don't trigger colors unless they are close enough
                                if (partial > 5)
                                {
									Colors currentColor = (Colors)data[4];
									port.LatestColor = currentColor;

									// Did at least 2 seconds passed if the color is the same as before?
									if (Environment.TickCount - port.LastColorTick > port.DistanceColorCooldownMs)
                                    {
                                        ColorTriggered?.Invoke(this, port, currentColor);
										DataUpdated();
                                    }
                                }

                            }
                            else
                                MainBoard.WriteLine("ERROR: Issue with Color Packet. It was too short!!", Color.Red);
                            
                        break;
                    }
					case Devices.POWERED_UP_REMOTE_BUTTON:
						{
							switch (data[4])
							{
								case 0x01:
									{
										MainBoard.WriteLine("button " + port.Id + " - BUTTON_PLUS was pressed");
										OnRemoteTriggered(this, port, RemoteButtons.BUTTON_PLUS);
										break;
									}
								case 0xff:
									{
										MainBoard.WriteLine("button " + port.Id + " - BUTTON_MINUS was pressed");
										OnRemoteTriggered(this, port, RemoteButtons.BUTTON_MINUS);
										break;
									}
								case 0x7f:
									{
										MainBoard.WriteLine("button " + port.Id + " - BUTTON_STOP was pressed");
										OnRemoteTriggered(this, port, RemoteButtons.BUTTON_STOP);
										break;
									}
								case 0x00:
									{
										MainBoard.WriteLine("button " + port.Id + " - ButtonState.RELEASED");
										OnRemoteTriggered(this, port, RemoteButtons.BUTTON_RELEASED);
										break;
									}
							}
							break;
						}
                }
            }
            else
            {
                MainBoard.WriteLine($"ERROR: {Name} - Port could not be found or is not connected", Color.Red);
            }
        }

        protected void WriteMessage(byte[] message)
        {
            WriteMessage(message, true);
        }

            /// <summary>
            /// Send the message to the proper device and characteristic
            /// </summary>
            /// <param name="message"></param>
        protected virtual async void WriteMessage(byte[] message, bool addLength)
        {
            try
            {
                if (Characteristic != null)
                {
                    using (DataWriter writer = new DataWriter())
                    {
                        if (addLength)
                            writer.WriteBytes(new byte[] { (byte)(message.Length + 2), 0x00 });
                        writer.WriteBytes(message);
                        await Characteristic.WriteValueAsync(writer.DetachBuffer(), GattWriteOption.WriteWithoutResponse);
                    }
                }
            }
            catch
            {
				MainBoard.WriteLine($"{Name} lost bluetooth connection", Color.Red);
				Dispose();

				OnDataUpdated();
			}
        }

		public virtual void Disconnect()
		{
			if (Type == Types.POWERED_UP_HUB || Type == Types.BOOST_MOVE_HUB)
				WriteMessage(new byte[] { 0x02, 0x01 });
		}

        public void Dispose()
        {
			Disconnect();

            Gatt = null;
            AllCharacteristic = null;
            Characteristic = null;

            if (Device != null)
                Device.Dispose();

            Device = null;
			IsConnected = false;

			// Finally, we clear this device to welcome new Advertisements
			MainBoard.registeredBluetoothDevices.RemoveAll(s => s == BluetoothAddress);
		}



		//////////////////////////////////////////
		////
		////
		////
		//// All Public Functions functions
		////
		////
		////
		////
		//////////////////////////////////////////



		public void SetMotorSpeed(int speed)
		{
			if (TrainMotorPort != null)
				SetMotorSpeed(TrainMotorPort, speed);
			else
				MainBoard.WriteLine("Could not start engine as no TRAIN_MOTOR is defined");
		}


        /// <summary>
        /// Set Motor Speed for this train
        /// </summary>
        /// <param name="port"></param>
        /// <param name="speed"></param>
        public virtual void SetMotorSpeed(string port, int speed)
        {
            byte[] message;
            Port portObj = GetPortFromPortId(port);

            // If we can't find the port, we can't do anything!
            if (portObj == null)
            {
                MainBoard.WriteLine("Could not set Motor Speed to " + speed + " for " + Name + " because no default port are setup", Color.Red);
                return;
            }

            // BOOST HUB
            if (Type == Types.BOOST_MOVE_HUB)
            {
                portObj.Busy = true;
                message = null;
                if (portObj.Id == "AB")
                {
                    message = new byte[] { 0x81, (byte)portObj.Value, 0x11, 0x02, (byte)speed, (byte)speed, 0x64, 0x7f, 0x03 };
                }
                else
                {
                    message = new byte[] { 0x81, (byte)portObj.Value, 0x11, 0x01, (byte)speed, 0x64, 0x7f, 0x03 };
                }
            }
            // ELSE TRAIN HUB
            else
            {
                if (port == "AB")
                {
                    byte p = 57;
                    byte s = (byte)speed;
                    message = new byte[] { 0x81, p, 0x11, 0x02, (byte)speed, (byte)speed };
                }
                else
                {
                    byte s = (byte)speed;
                    byte p = (port == "A") ? (byte)0x00 : (byte)0x01;
					message = new byte[] { 0x81, (byte)portObj.Value, 0x11, 0x51, 0x00, (byte)speed };
					//message = new byte[] { 0x81, p, 0x11, 0x60, 0x00, (byte)speed, 0x00, 0x00 };
                }
            }

			portObj.Speed = speed;
			IsBusy = speed != 0 && speed != 127;

			OnDataUpdated();
			WriteMessage(message);
        }

		public virtual void SetLightBrightness(string port, int brightness)
		{
		
			Port portObj = GetPortFromPortId(port);

			// If we can't find the port, we can't do anything!
			if (portObj == null)
			{
				MainBoard.WriteLine("Could not set Light Brightness for " + Name + " because no default port are setup", Color.Red);
				return;
			}

			byte[] message = new byte[] { 0x81, (byte)portObj.Value, 0x11, 0x51, 0x00, (byte)brightness };
			WriteMessage(message);

		}

		public int GetMotorSpeed(string port)
		{
			Port portObj = GetPortFromPortId(port);
			if (portObj != null)
				return portObj.Speed;

			return 0;
		}


		public void RampMotorSpeed(string port, int toSpeed, int timeinms)
		{
			Port p = GetPortFromPortId(port);

			if (p == null)
				return;

			RampMotorSpeed(port, p.Speed, toSpeed, timeinms);
		}

		/// <summary>
		/// Automatically accelerate or decelerate a train
		/// </summary>
		/// <param name="port"></param>
		/// <param name="fromSpeed"></param>
		/// <param name="toSpeed"></param>
		/// <param name="timeinms"></param>
		public void RampMotorSpeed(string port, int fromSpeed, int toSpeed, int timeinms)
        {
			Port p = GetPortFromPortId(port);

			if (p == null)
				return;

			if (p.MotorTimer != null)
            {
				p.MotorTimer.Stop();
				p.MotorTimer.Dispose();
            }

            double steps = Math.Abs(toSpeed - fromSpeed);
            double delay = timeinms / steps;
            double increment = 1;
            if (delay < 50 && steps > 0)
            {
                increment = 50 / delay;
                delay = 50;
            }
            if (fromSpeed > toSpeed)
            {
                increment = -increment;
            }

            int i = 0;

            p.MotorTimer = new System.Timers.Timer();
            p.MotorTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                int speed = (int)Math.Round(fromSpeed + (++i * increment));
                if ((toSpeed > fromSpeed && speed > toSpeed) || (fromSpeed > toSpeed && speed < toSpeed))
                {
                    speed = toSpeed;
                }

                this.SetMotorSpeed(port, speed);

                if (speed == toSpeed)
                {
					p.MotorTimer.Stop();
                }
            };

            if (p.MotorTimer != null && delay > 0 && delay < int.MaxValue && delay > int.MinValue)
            {
				p.MotorTimer.Interval = delay;
				p.MotorTimer.Start();
            }
         }

		public bool IsSwitchOnTheLeft(string port)
		{
			Port targetPort = GetPortFromPortId(port);
			return (targetPort != null && targetPort.TargetSpeed < 0);
		}

		public void ActivateSwitchToRight(string port)
		{
			ActivateSwitch(port, false);
		}

		public void ActivateSwitchToLeft(string port)
		{
			ActivateSwitch(port, true);
		}

		public void InvertSwitch(string port)
		{
			Port targetPort = GetPortFromPortId(port);

			// Activate the switch to the invert it has now
			ActivateSwitch(port, (targetPort.TargetSpeed == 0) ? true : (targetPort.TargetSpeed < 0));
		}

		public void ActivateSwitch(string port, bool left)
        {
			Port targetPort = GetPortFromPortId(port);

			switch (targetPort.Function)
			{
				case Port.Functions.SWITCH_DOUBLECROSS:
					{
						targetPort.TargetSpeed = (left) ? -100 : 100;
						SetMotorSpeed(port, targetPort.TargetSpeed);

						System.Timers.Timer timer = new System.Timers.Timer(700);
						timer.Elapsed += (object sender, ElapsedEventArgs ev) =>
						{
							// Stop it after 700ms 
							timer.Stop();
							Stop(port);

							timer = new System.Timers.Timer(200);
							timer.Elapsed += (object sender2, ElapsedEventArgs ev2) =>
							{
								timer.Stop();

								SetMotorSpeed(port, targetPort.TargetSpeed);
								timer = new System.Timers.Timer(700);
								timer.Elapsed += (object sender3, ElapsedEventArgs ev3) =>
								{									
									timer.Stop();
									timer.Dispose();
									Stop(port);
								};
								timer.Start();
							};
							timer.Start();
						};
						timer.Start();
						break;
					}
				case Port.Functions.SWITCH_STANDARD:
					{
						targetPort.TargetSpeed = (left) ? -100 : 100;
						SetMotorSpeed(port, targetPort.TargetSpeed);

						System.Timers.Timer timer = new System.Timers.Timer(500);
						timer.Elapsed += (object sender, ElapsedEventArgs ev) =>
						{
							// Stop it after 700ms 
							timer.Stop();
							Stop(port, true);
						};
						timer.Start();
						break;
					}
				case Port.Functions.SWITCH_TRIXBRIX:
					{
						targetPort.TargetSpeed = (left) ? -40 : 40;
						SetMotorSpeed(port, targetPort.TargetSpeed);

						System.Timers.Timer timer = new System.Timers.Timer(500);
						timer.Elapsed += (object sender, ElapsedEventArgs ev) =>
						{
							// Stop it after 700ms 
							timer.Stop();
							Stop(port);
						};
						timer.Start();
						break;
					}
			}
		}

        public void Stop()
        {
			foreach (Port p in RegistredPorts)
				if (p.Speed != 0)
					Stop(p.Id, true);
        }

		public void Stop(string port)
		{
			Stop(port, false);
		}

		public void Stop(string port, bool stopFast)
        {
			Port p = GetPortFromPortId(port);

			if (p == null)
				return;

			if (p.MotorTimer != null)
			{
				p.MotorTimer.Stop();
				p.MotorTimer.Dispose();
			}

			if (!stopFast)
				SetMotorSpeed(port, 0);
			else
			{
				if (Type == Types.POWERED_UP_HUB || Type == Types.BOOST_MOVE_HUB)
					SetMotorSpeed(port, 127);
				else
					SetMotorSpeed(port, 0);

				System.Timers.Timer timer = new System.Timers.Timer(1000);
				timer.Elapsed += (object sender, ElapsedEventArgs ev) =>
				{
				// Stop it after 700ms 
				timer.Stop();
					SetMotorSpeed(port, 0);
				};
				timer.Start();
			}

			IsBusy = false;
		}

		public int GetSpeed(string port)
		{
			Port p = GetPortFromPortId(port);
			return (p == null) ? 0 : p.Speed;
		}

		public int GetDistance(string port)
		{
			Port p = GetPortFromPortId(port);
			return (p == null) ? 0 : p.LatestDistance;
		}

		public Colors GetColor(string port)
		{
			Port p = GetPortFromPortId(port);
			return (p == null) ? 0 : p.LatestColor;
		}

		public void SetSensorCooldown(string port, int ms)
		{
			Port p = GetPortFromPortId(port);
			if (p != null)
				p.DistanceColorCooldownMs = ms;
		}


		public virtual void SetLEDColor(Colors color)
		{
			byte[] data = new byte[] { 0x41, 0x32, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 };
			this.WriteMessage(data);
			WriteMessage(new byte[] { 0x81, 0x32, 0x11, 0x51, 0x00, (byte)color });
		}

		public void RestoreLEDColor()
		{
			if (LEDColor == Colors.BLACK)
				LEDColor = Colors.GREEN;

			SetLEDColor(LEDColor);
		}

		//////////////////////////////////////////
		////
		////
		////
		//// Self-Driving functions
		////
		////
		////
		////
		//////////////////////////////////////////







		public async Task<bool> InitNewAutomatedPath(Sections sections, Path path)
		{
			int totalSections = sections.GetAll().Count;
			bool wasWaiting = IsWaitingSection;
			int resumingIndex = -1;

			// If the train is waiting at a section we wait for its status to be cleared
			while (IsWaitingSection)
			{
				AbortReserve = true;
				MainBoard.WriteLine($"{Name} - Attempting to assign new path, but waiting for lock to finish", Color.Red);
				await Task.Delay(1000);
			}

			AbortReserve = false;

			// In a normal case, we simply check the rail
			if (!wasWaiting)
				resumingIndex = ResumePathPosition(sections, path);
			// If we did wait then section state can be multiple
			else
			{
				// So we take the last know section from this hub
				resumingIndex = IsSectionInPath(path, CurrentPathPositionIndex) ? CurrentPathPositionIndex : -1;
			}

			// We don't exist in the path proposed
			if (resumingIndex == -1)
			{
				MainBoard.WriteLine($"{Name} is not in the current path provided!", Color.Red);
				return false;
			}

			CurrentPathPositionIndex = resumingIndex;
			CurrentPath = path;

			MainBoard.WriteLine($"{Name} self-driving path is set to {CurrentPath.Name}! Resuming from Section " + CurrentPath.Sections[CurrentPathPositionIndex], Color.Green);
			return true;
		}

		public async Task ClearAutomatedPathAndStop(bool clearPath)
		{
			AbortReserve = true;

			// If the train is waiting at a section we wait for its status to be cleared
			while (IsWaitingSection)
			{
				MainBoard.WriteLine($"{Name} - Attempting to clear path and stop train, but waiting for lock to finish", Color.Red);
				await Task.Delay(1000);
			}

			AbortReserve = false;

			if (clearPath)
			{
				IsPathProgramRunning = false;
				CurrentPathPositionIndex = -1;
				CurrentPath = null;
			}

			// Stop the train!
			Stop();

			MainBoard.WriteLine($"{Name} path has been cleared and train is stopped", Color.Red);
		}

		private bool IsSectionInPath(Path path, int sectionId)
		{
			for (int i = 0; i < path.Sections.Length; i++)
				if (path.Sections[i] == sectionId)
					return true;

			return false;
		}

		private int ResumePathPosition(Sections program, Path path)
		{
			int resumingIndex = -1;

			for (int i = 0; i < path.Sections.Length; i++)
			{
				if (program[path.Sections[i]].CurrentHub != null && program[path.Sections[i]].CurrentHub == this)
				{
					resumingIndex = i;
					break;
				}
			}

			return resumingIndex;
		}

		internal int GetNextSectionIndex()
		{
			if (CurrentPath != null && CurrentPathPositionIndex != -1)
			{
				int nextPositionId = (CurrentPathPositionIndex + 1);

				if (nextPositionId >= CurrentPath.Sections.Length)
					nextPositionId = LoopCurrentPath ? 0 : -1;

				// We skip the first position if it is the same as last when we loop
				if (nextPositionId == 0 && CurrentPathPositionIndex == CurrentPath.Sections.Length - 1 && CurrentPath.Sections[nextPositionId] == CurrentPath.Sections[CurrentPathPositionIndex])
				{
					CurrentPathPositionIndex = 0;
					nextPositionId = 1;
				}

				return (nextPositionId >= 0 && nextPositionId < CurrentPath.Sections.Length) ? CurrentPath.Sections[nextPositionId] : -1;
			}

			return -1;
		}

		internal void MoveToNextSectionIndex()
		{
			CurrentPathPositionIndex++;

			if (CurrentPathPositionIndex >= CurrentPath.Sections.Length && LoopCurrentPath)
				CurrentPathPositionIndex = 0;
		}




		//////////////////////////////////////////
		////
		////
		////
		//// Helper functions
		////
		////
		////
		////
		//////////////////////////////////////////


		protected void RegisterDeviceAttachement(Port port, Port.Devices type)
        {
			if (port.Connected)
			{
				port.Device = type;
				MainBoard.WriteLine("Port Connected: " + port.Id + " of type " + Enum.GetName(typeof(Port.Devices), type), Color.Green);

				if (port.Device == Port.Devices.TRAIN_MOTOR || port.Device == Port.Devices.BOOST_MOTOR || port.Device == Port.Devices.BOOST_EXT_MOTOR 
					|| port.Device == Port.Devices.BASIC_MOTOR || port.Device == Devices.EV3_MOTOR 
					|| port.Device == Devices.CONTROL_PLUS_L_MOTOR || port.Device == Devices.CONTROL_PLUS_XL_MOTOR)
				{
					if (port.Device == Port.Devices.TRAIN_MOTOR)
						port.Function = Port.Functions.TRAIN_MOTOR;
					else if (port.Function == Port.Functions.NOT_USED)
						port.Function = Port.Functions.MOTOR;

					if (port.Function == Functions.SWITCH_DOUBLECROSS ||
						port.Function == Functions.SWITCH_STANDARD ||
						port.Function == Functions.SWITCH_TRIXBRIX)
						ActivateSwitchToLeft(port.Id);
				}

				if (port.Device == Port.Devices.BOOST_DISTANCE)
				{
					if (port.Function == Port.Functions.NOT_USED)
						port.Function = Port.Functions.SENSOR;
					ActivatePortDevice((byte)port.Value, (byte)(this.Type == Types.WEDO_2_SMART_HUB ? 0x00 : 0x08), 0, 0);
				}

				if (port.Device == Port.Devices.WEDO2_DISTANCE)
				{
					if (port.Function == Port.Functions.NOT_USED)
						port.Function = Port.Functions.SENSOR;
					ActivatePortDevice((byte)port.Value, 0x00, 0, 0);
				}

				if (port.Device == Port.Devices.POWERED_UP_REMOTE_BUTTON)
				{
					port.Function = Port.Functions.BUTTON;
					ActivatePortDevice((byte)port.Value, 0x00, 0, 0);
				}

				if (port.Device == Devices.EV3_SENSOR || port.Device == Devices.NXT_SENSOR || port.Device == Devices.EV3_COLOR_SENSOR)
				{
					port.Function = Port.Functions.SENSOR;
				}

				if (port.Device == Port.Devices.LED_LIGHTS)
				{
					if (port.Function == Port.Functions.NOT_USED)
						port.Function = Port.Functions.LIGHT;
					SetLightBrightness(port.Id, 100);
				}

				// Let them know the type has changed
				OnPortTypeUpdate();

				// And data has changed!
				OnDataUpdated();
			}
            else
            {
                port.Device = Port.Devices.UNKNOWN;
				port.Function = Port.Functions.NOT_USED;

                MainBoard.WriteLine("Port Disconnected: " + port.Id, Color.Purple);
				OnDataUpdated();
			}

        }

		protected virtual void ActivatePortDevice(byte port, byte type, byte mode, byte format)
		{
			WriteMessage(new byte[] { 0x01, 0x00 }, false); // Continious reading activation
			WriteMessage(new byte[] { 0x41, port, type, 0x01, 0x00, 0x00, 0x00, 0x01 }); // Activate sensore reports
		}

		public Port GetPortFromPortId(string name)
        {
            foreach (Port port in RegistredPorts)
            {
                if (port.Id == name)
                    return port;
            }

            return null;
        }

        protected Port GetPortFromPortNumber(int number)
        {
            foreach (Port port in RegistredPorts)
            {
                if (port.Value == number)
                    return port;
            }

            return null;
        }

        internal static void SaveAll(List<Hub> registeredTrains)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("./trainAll.txt", FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, registeredTrains);
            stream.Close();
        }

        internal static List<Hub> LoadAll(string path)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);
            List<Hub> trains = null;

            if (stream.Length > 0)
                trains = (List<Hub>)formatter.Deserialize(stream);

            stream.Close();
            trains = (trains == null) ? new List<Hub>() : trains;

            return trains;
        }
    }
}
