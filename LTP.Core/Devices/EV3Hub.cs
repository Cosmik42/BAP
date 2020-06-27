using Lego.Ev3.Core;
using Lego.Ev3.Desktop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Windows.Devices.Bluetooth;
using static LegoTrainProject.Hub;

namespace LegoTrainProject
{
	[Serializable]
	class EV3Hub : Hub
	{
		private enum EV3Color
		{
			No_Color = 0,
			Black = 1,
			Blue = 2,
			Green = 3,
			Yellow = 4,
			Red = 5,
			White = 6,
			Brown = 7
		}

		[NonSerialized]
		Brick brick;

		public EV3Hub(BluetoothLEDevice device, Types type, string comAddress) : base(device, type)
		{
			Name = "EV3";
			DeviceId = comAddress;

			Type = type;
			IsConnected = false;

			InitPorts();
		}

		public override void InitPorts()
		{
			// Clear any previous port
			RegistredPorts.Clear();

			Port portA = new Port("A", 0, true);
			Port portB = new Port("B", 1, true);
			Port portC = new Port("C", 2, true);
			Port portD = new Port("D", 3, true);

			Port port1 = new Port("One", 4, true);
			Port port2 = new Port("Two", 5, true);
			Port port3 = new Port("Three", 6, true);
			Port port4 = new Port("Four", 7, true);

			RegistredPorts.Add(portA);
			RegistredPorts.Add(portB);
			RegistredPorts.Add(portC);
			RegistredPorts.Add(portD);

			RegistredPorts.Add(port1);
			RegistredPorts.Add(port2);
			RegistredPorts.Add(port3);
			RegistredPorts.Add(port4);
		}

		public void TryToConnect()
		{
			MainBoard.WriteLine("Connecting to EV3 on port " + DeviceId);

			var conType = CreateConnection();

			Dispatcher.CurrentDispatcher.Invoke(new Action(async () =>
			{
				if (conType != null)
				{
					brick = new Brick(conType, true);
					brick.BrickChanged += Brick_BrickChanged;
					try
					{
						await brick.ConnectAsync();
					}
					catch (Exception)
					{
						MessageBox.Show("Could not connect", "Error", MessageBoxButtons.OK);
					}
				}
				else
				{
					MessageBox.Show("Invalid connection type for this device", "Error", MessageBoxButtons.OK);
				}
			}));
		}

		private void Brick_BrickChanged(object sender, BrickChangedEventArgs e)
		{
			IsConnected = true;
			bool somethingChanged = false;

			foreach (InputPort type in (InputPort[])Enum.GetValues(typeof(InputPort)))
			{
				// Get the port
				Port p = GetPortFromPortId(type.ToString());

				// Check device type
				Port.Devices device = ConvertTypeToDevice(brick.Ports[type].Type);

				// Are we set? If not register the device type
				if (p.Device != device)
				{
					p.Connected = (device != Port.Devices.UNKNOWN);
					RegisterDeviceAttachement(p, device);
				}

				// We need to force Color mode if we have a color detector
				if (brick.Ports[type].Type == DeviceType.Color && brick.Ports[type].Mode != (byte)ColorMode.Color)
					brick.Ports[type].SetMode(ColorMode.Color);

				// If we do have a color mode then let's update it!
				if (brick.Ports[type].Mode == (byte)ColorMode.Color)
				{
					p.LatestColor = ConvertEV3ColorToPortColor((EV3Color)brick.Ports[type].RawValue);

					if (MainBoard.showColorDebug)
						MainBoard.WriteLine($"{Name} - Color Received: " + p.LatestColor + " last triggers was " + ((Environment.TickCount - p.LastColorTick) / 1000) + "s ago.");

					if (Environment.TickCount - p.LastColorTick > p.DistanceColorCooldownMs)
					{
						OnColorTriggered(this, p, p.LatestColor);
						OnDataUpdated();
					}
				}

				// If we have an active sensor on, check for event trigger
				else if (p.Value != brick.Ports[type].RawValue && device != Port.Devices.UNKNOWN)
				{
					p.LatestDistance = brick.Ports[type].RawValue;
					somethingChanged = true;

					if (MainBoard.showColorDebug)
						MainBoard.WriteLine($"{Name} - Raw Value Received: " + brick.Ports[type].RawValue  + " last triggers was " + ((Environment.TickCount - p.LastDistanceTick) / 1000) + "s ago.");

					if (Environment.TickCount - p.LastDistanceTick > p.DistanceColorCooldownMs)
					{
						OnDistanceTriggered(this, p, p.LatestDistance);
						OnDataUpdated();
					}
				}
			}

			if (somethingChanged)
				OnDataUpdated();
		}


		private Port.Colors ConvertEV3ColorToPortColor(EV3Color color)
		{
			switch (color)
			{
				case EV3Color.Black:
					return Port.Colors.BLACK;

				case EV3Color.Blue:
					return Port.Colors.BLUE;

				case EV3Color.Brown:
					return Port.Colors.ORANGE;

				case EV3Color.Green:
					return Port.Colors.GREEN;

				case EV3Color.Red:
					return Port.Colors.RED;

				case EV3Color.White:
					return Port.Colors.WHITE;

				case EV3Color.Yellow:
					return Port.Colors.YELLOW;
			}

			return Port.Colors.NONE;
		}

		private Port.Devices ConvertTypeToDevice(DeviceType type)
		{
			switch (type)
			{
				case DeviceType.LMotor:
				case DeviceType.MMotor:
					return Port.Devices.EV3_MOTOR;

				case DeviceType.Touch:
				case DeviceType.Ultrasonic:
				case DeviceType.Infrared:
					return Port.Devices.EV3_SENSOR;

				case DeviceType.Color:
				case DeviceType.NxtColor:
					return Port.Devices.EV3_COLOR_SENSOR;

				case DeviceType.NxtLight:
				case DeviceType.NxtTemperature:
				case DeviceType.NxtTouch:
				case DeviceType.NxtUltrasonic:
					return Port.Devices.NXT_SENSOR;
			}

			return Port.Devices.UNKNOWN;
		}

		protected override void ActivatePortDevice(byte port, byte type, byte mode, byte format)
		{
			// Do nothing
		}

		private ICommunication CreateConnection()
		{
			ICommunication returnType = null;

			returnType = new BluetoothCommunication(DeviceId);
			//returnType = new UsbCommunication();
		    //returnType = new NetworkCommunication(ConnControl.GetIpAddress());

			return returnType;
		}

		public override void SetLEDColor(Port.Colors color)
		{
			LedPattern ledPattern = (color == Port.Colors.GREEN) ? LedPattern.Green : (color == Port.Colors.RED) ? LedPattern.Red : (color == Port.Colors.ORANGE) ? LedPattern.Orange : LedPattern.Black;
			brick.DirectCommand.SetLedPatternAsync(ledPattern);
		}

		public override void SetMotorSpeed(string port, int speed)
		{
			Port portObj = GetPortFromPortId(port);

			// If we can't find the port, we can't do anything!
			if (portObj == null)
			{
				MainBoard.WriteLine("Could not set Motor Speed to " + speed + " for " + Name, Color.Red);
				return;
			}

			portObj.Speed = speed;
			IsBusy = (speed != 0);

			OnDataUpdated();

			OutputPort outputPort = (port == "A") ? OutputPort.A : (port == "B") ? OutputPort.B : (port == "C") ? OutputPort.C : OutputPort.D;
			brick.DirectCommand.TurnMotorAtPowerAsync(outputPort, speed);
		}

		public override void SetLightBrightness(string port, int brightness)
		{
			Port portObj = GetPortFromPortId(port);

			// If we can't find the port, we can't do anything!
			if (portObj == null)
			{
				MainBoard.WriteLine("Could not set light brightness for " + Name, Color.Red);
				return;
			}

			portObj.Speed = brightness;
			OnDataUpdated();
			//brick.DirectCommand.(OutputPort.A, speed);
		}

		public override void Disconnect()
		{
			if (IsConnected)
				brick.Disconnect();
		}
	}
}
