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
	public class PFxAction
	{
		const byte PFX_CMD_GET_EVENT_ACTION = 0x11;
		const byte PFX_CMD_SET_EVENT_ACTION = 0x12;
		const byte PFX_CMD_TEST_ACTION = 0x13;
		const byte PFX_CMD_FILE_DIR = 0x45;

		const byte PFX_DIR_REQ_GET_FILE_COUNT = 0x00;
		const byte PFX_DIR_REQ_GET_FREE_SPACE = 0x01;
		const byte PFX_DIR_REQ_GET_DIR_ENTRY_IDX = 0x02;

		public byte command = 0;
		public byte motorActionId = 0;
		public byte motorParam1 = 0;
		public byte motorParam2 = 0;
		public byte lightFxId = 0;
		public byte lightOutputMask = 0;
		public byte lightPFOutputMask = 0;
		public byte lightParam1 = 0;
		public byte lightParam2 = 0;
		public byte lightParam3 = 0;
		public byte lightParam4 = 0;
		public byte lightParam5 = 0;
		public byte soundFxId = 0;
		public byte soundFileId = 0;
		public byte soundParam1 = 0;
		public byte soundParam2 = 0;

		public byte[] GetTestActionData()
		{
			byte[] msg = new byte[17];
			msg[0] = PFX_CMD_TEST_ACTION;

			Array.Copy(ToBytes(), 0, msg, 1, 16);
			return msg;
		}

		public byte[] GetCmdGetDirEntry(byte idx)
		{
			byte[] msg = new byte[3];
			msg[0] = PFX_CMD_FILE_DIR;
			msg[1] = PFX_DIR_REQ_GET_DIR_ENTRY_IDX;
			msg[2] = (byte)(idx & 0xFF);
			return msg;
		}

		public byte[] GetCmdNumFiles()
		{
			byte[] msg = new byte[2];
			msg[0] = PFX_CMD_FILE_DIR;
			msg[1] = PFX_DIR_REQ_GET_FILE_COUNT;
			return msg;
		}

		public byte[] GetCmdFreeSpace()
		{
			byte[] msg = new byte[2];
			msg[0] = PFX_CMD_FILE_DIR;
			msg[1] = PFX_DIR_REQ_GET_FREE_SPACE;
			return msg;
		}

		public byte[] ToBytes()
		{
			byte[] msg = new byte[16];

			msg[0] = command;
			msg[1] = motorActionId;
			msg[2] = motorParam1;
			msg[3] = motorParam2;
			msg[4] = lightFxId;
			msg[5] = lightOutputMask;
			msg[6] = lightPFOutputMask;
			msg[7] = lightParam1;
			msg[8] = lightParam2;
			msg[9] = lightParam3;
			msg[10] = lightParam4;
			msg[11] = lightParam5;
			msg[12] = soundFxId;
			msg[13] = soundFileId;
			msg[14] = soundParam1;
			msg[15] = soundParam2;

			return msg;
		}

		public void FromBytes(byte[] msg)
		{
			command = msg[0];
			motorActionId = msg[1];
			motorParam1 = msg[2];
			motorParam2 = msg[3];
			lightFxId = msg[4];
			lightOutputMask = msg[5];
			lightPFOutputMask = msg[6];
			lightParam1 = msg[7];
			lightParam2 = msg[8];
			lightParam3 = msg[9];
			lightParam4 = msg[10];
			lightParam5 = msg[11];
			soundFxId = msg[12];
			soundFileId = msg[13];
			soundParam1 = msg[14];
			soundParam2 = msg[15];
		}
	}


	[Serializable]
	public class PFxHub : Hub
	{

		const byte EVT_MOTOR_SPEED_HIRES_MASK = 0x3F;
		const byte EVT_MOTOR_SPEED_HIRES = 0x80;
		const byte EVT_MOTOR_SPEED_HIRES_REV = 0x40;
		const byte EVT_MOTOR_SPEED_HIRES_FWD = 0x80;
		const byte EVT_MOTOR_OUTPUT_MASK = 0x0F;
		const byte EVT_MOTOR_SET_SPD = 0x70;
		const byte EVT_LIGHTFX_ON_OFF_TOGGLE = 0x01;
		const byte EVT_LIGHTFX_SET_BRIGHT = 0x04;
		const byte EVT_TRANSITION_ON = 0x01;
		const byte EVT_TRANSITION_OFF = 0x02;
		const byte EVT_LIGHT_COMBO_MASK = 0x80;
		const byte EVT_SOUND_TOGGLE = 0x0;
		const byte EVT_SOUND_RESTART = 0x1;
		const byte EVT_SOUND_NONE = 0x0;
		const byte EVT_SOUND_INC_VOL = 0x1;
		const byte EVT_SOUND_DEC_VOL = 0x2;
		const byte EVT_SOUND_SET_VOL = 0x3;
		const byte EVT_SOUND_PLAY_ONCE = 0x4;
		const byte EVT_SOUND_PLAY_CONT = 0x5;
		const byte EVT_SOUND_PLAY_NTIMES = 0x6;
		const byte EVT_SOUND_PLAY_DUR = 0x7;
		const byte EVT_SOUND_PLAY_PITCH = 0x8;
		const byte EVT_SOUND_PLAY_GATED = 0x9;
		const byte EVT_SOUND_PLAY_AM = 0xA;
		const byte EVT_SOUND_STOP = 0xB;

		public enum PFxLightFx
		{
			TURN_OFF = 0x00,
			ON_OFF_TOGGLE = 0x01,
			INC_BRIGHT = 0x02,
			DEC_BRIGHT = 0x03,
			SET_BRIGHT = 0x04,
			FLASH_50 = 0x05,
			STROBE = 0x07,
			GYRALITE = 0x09,
			FLICKER = 0x0B,
			RAND_BLINK = 0x0C,
			PHOTON_TORP = 0x0D,
			LASER_PULSE = 0x0E,
			ENGINE_GLOW = 0x0F,
			LIGHTHOUSE = 0x10,
			BROKEN_LIGHT = 0x11
		}

		public enum PFxLightFactor
		{
			FADE_FACTOR_NONE = 0x0,
			FADE_FACTOR_1 = 0x1,
			FADE_FACTOR_5 = 0x2,
			FADE_FACTOR_10 = 0x3,
			FADE_FACTOR_15 = 0x4,
			FADE_FACTOR_20 = 0x5,
			FADE_FACTOR_25 = 0x6,
			FADE_FACTOR_30 = 0x7,
			FADE_FACTOR_40 = 0x8,
			FADE_FACTOR_50 = 0x9,
			FADE_FACTOR_75 = 0xA,
			FADE_FACTOR_90 = 0xB,
			FADE_FACTOR_100 = 0xC,
			FADE_FACTOR_150 = 0xD,
			FADE_FACTOR_200 = 0xE,
			FADE_FACTOR_400 = 0xF,
			FADE_FACTOR_MAX = 0x10,
			FADE_FACTOR_MIN = 0x1F
		}

		public enum PFxLightPeriod
		{
			PERIOD_100MS = 0x0,
			PERIOD_250MS = 0x1,
			PERIOD_500MS = 0x2,
			PERIOD_750MS = 0x3,
			PERIOD_1S = 0x4,
			PERIOD_1_25S = 0x5,
			PERIOD_1_5S = 0x6,
			PERIOD_1_75S = 0x7,
			PERIOD_2S = 0x8,
			PERIOD_2_5S = 0x9,
			PERIOD_3S = 0xA,
			PERIOD_4S = 0xB,
			PERIOD_5S = 0xC,
			PERIOD_8S = 0xD,
			PERIOD_10S = 0xE,
			PERIOD_20S = 0xF,
			PERIOD_MAX = 0x10
		}

		[NonSerialized]
		GattCharacteristic TxCharacteristic;

		[NonSerialized]
		List<string> fileNames = new List<string>();

		[NonSerialized]
		List<int> fileId = new List<int>();

		[NonSerialized]
		bool RxRead = true;

		[NonSerialized]
		byte[] Rx;

		public PFxHub(BluetoothLEDevice device, Types type) : base(device, type)
		{

		}

		internal override async Task RenewCharacteristic()
		{
			if (Device != null)
			{
				Device = await BluetoothLEDevice.FromBluetoothAddressAsync(Device.BluetoothAddress);
				Gatt = await Device.GetGattServicesAsync(BluetoothCacheMode.Uncached);
				AllCharacteristic = await Gatt.Services.Single(s => s.Uuid == Guid.Parse("49535343-FE7D-4AE5-8FA9-9FAFD205E455")).GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
				Characteristic = AllCharacteristic.Characteristics.Single(c => c.Uuid == Guid.Parse("49535343-1E4D-4BD9-BA61-23C647249616"));
				TxCharacteristic = AllCharacteristic.Characteristics.Single(c => c.Uuid == Guid.Parse("49535343-8841-43F4-A8D4-ECBE34729BB3"));

				MainBoard.WriteLine("New Hub Found of type " + Enum.GetName(typeof(Hub.Types), Type), Color.Green);
			}
		}

		public override void InitPorts()
		{
			// Clear any previous port
			RegistredPorts.Clear();

			Port portA = new Port("A", 0, true);
			Port portB = new Port("B", 1, true);
			Port portC = new Port("C", 2, true);
			Port portD = new Port("D", 3, true);

			RegistredPorts.Add(portA);
			RegistredPorts.Add(portB);
			RegistredPorts.Add(portC);
			RegistredPorts.Add(portD);

			portA.Function = Port.Functions.MOTOR;
			portB.Function = Port.Functions.MOTOR;
			portC.Function = Port.Functions.PFX_SPEAKER;
			portD.Function = Port.Functions.PFX_LIGHT_CONTROLER;
		}

		internal override void InitializeNotifications()
		{
			// Make sure the light is ready
			//SetLightFXBrightness("1,2,3,4,5,6,7,8", 255);

			// We do nothing
			RefreshFileDir();
		}

		/// <summary>
		/// Treat Incoming Data from the train
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		internal override void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
		{			
			// An Indicate or Notify reported that the value has changed.
			var reader = DataReader.FromBuffer(args.CharacteristicValue);

			Rx = new byte[reader.UnconsumedBufferLength];
			reader.ReadBytes(Rx);

			RxRead = false;
		}

		internal async Task<byte[]> WaitForRx()
		{
			while (RxRead)
			{
				await Task.Delay(10);
			}

			byte[] tempRx = Rx.ToArray();

			RxRead = true;
			return tempRx;
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

		private async Task<byte[]> SendData(byte[] data)
		{
			byte[] nsData = new byte[data.Length + 6];

			nsData[0] = nsData[1] = nsData[2] = 0x5B;
			data.CopyTo(nsData, 3);
			nsData[nsData.Length - 3] = nsData[nsData.Length - 2] = nsData[nsData.Length - 1] = 0x5D;

			int lastChunkIndex = 0;
			do
			{
				int chuckSize = Math.Min(nsData.Length - lastChunkIndex, 20);
				byte[] chunkData = new byte[chuckSize];

				Array.Copy(nsData, lastChunkIndex, chunkData, 0, chuckSize);
				WriteMessage(chunkData, TxCharacteristic);
				lastChunkIndex += 20;
			}
			while (lastChunkIndex < nsData.Length);

			return await WaitForRx();
		}


		public override void SetLEDColor(Colors color)
		{
			// Nothing to do
		}

		public async override void SetMotorSpeed(string port, int speed)
		{
			PFxAction action = new PFxAction();
			Port portObj = GetPortFromPortId(port);

			// If we can't find the port, we can't do anything!
			if (portObj == null)
			{
				MainBoard.WriteLine("Could not set Motor Speed to " + speed + " for " + Name + " because no default port are setup", Color.Red);
				return;
			}

			double sf = speed;
			if (sf > 100.0) sf = 100.0;
			if (sf < -100.0) sf = -100.0;
			sf = (sf / 100.0) * 63.0;

			portObj.Speed = speed;

			int si = (int)(Math.Abs(sf)) & EVT_MOTOR_SPEED_HIRES_MASK;
			si |= EVT_MOTOR_SPEED_HIRES;
			if (sf < 0.0) si |= EVT_MOTOR_SPEED_HIRES_REV;
			action.motorParam1 = (byte)si;

			int m = PortToMask(port) & EVT_MOTOR_OUTPUT_MASK;

			m |= EVT_MOTOR_SET_SPD;
			action.motorActionId = (byte)m;

			OnDataUpdated();
			await SendData(action.GetTestActionData());
		}

		public override void SetLightBrightness(string port, int brightness)
		{
			SetMotorSpeed(port, brightness);
		}

		public async void SetLightFXBrightness(string port, int brightness)
		{
			PFxAction action = new PFxAction();

			action.lightOutputMask = PortToMask(port);
			action.lightFxId = EVT_LIGHTFX_ON_OFF_TOGGLE;
			action.lightParam4 = brightness > 0 ? EVT_TRANSITION_ON : EVT_TRANSITION_OFF;

			/*int x = brightness;
			if (x > 255) x = 255;
			if (x < 0) x = 0;*/

			//action.lightFxId = EVT_LIGHTFX_SET_BRIGHT;
			//action.lightParam1 = (byte)x;

			await SendData(action.GetTestActionData());
		}

		private async void RefreshFileDir()
		{
			try
			{

				PFxAction action = new PFxAction();
				byte[] rx = await SendData(action.GetCmdNumFiles());

				fileId.Clear();
				fileNames.Clear();

				int file_count = rx[4];
				for (int i = 0; i < file_count; i++)
				{
					byte[] rxDir = await SendData(action.GetCmdGetDirEntry((byte)(i + 1)));

					int id = rxDir[3];
					fileId.Add(id);

					string name = "";
					for (int j = 0; j < 32 && rxDir[24 + j] != 0; j++) name += (char)rxDir[24 + j];
					fileNames.Add(name);
					MainBoard.WriteLine("PFx: new file found - " + name + " (id " + id + ")");
				}

				MainBoard.WriteLine(fileId.Count + " files found on PFx");
			}
			catch(Exception ex)
			{
				MainBoard.WriteLine("Exception while parsing files: " + ex.Message, Color.Red);
			}
		}

		void ComboLightFx(int fx, byte[] param)
		{
			LightFx("", (byte)(fx | EVT_LIGHT_COMBO_MASK), param);
		}

		public async void LightFx(string ch, byte fx, byte[] param)
		{
			if (fx != (byte)PFxLightFx.TURN_OFF)
				SetLightFXBrightness(ch, 255);

			PFxAction action = new PFxAction();
			action.lightOutputMask = PortToMask(ch);
			action.lightFxId = fx;

			param = new byte[2];
			param[0] = (byte)PFxLightPeriod.PERIOD_1S;
			param[1] = (byte)PFxLightFactor.FADE_FACTOR_NONE; //FADE_FACTOR_150

			for (int i = 0; i < param.Length; i++)
			{
				switch (i)
				{
					case 0: action.lightParam1 = param[0]; break;
					case 1: action.lightParam2 = param[1]; break;
					case 2: action.lightParam3 = param[2]; break;
					case 3: action.lightParam4 = param[3]; break;
					case 4: action.lightParam5 = param[4]; break;
				}
			}

			byte[] res = await SendData(action.GetTestActionData());
		}

		async void SoundFx(byte fx, byte[] param, byte fileID)
		{
			PFxAction action = new PFxAction();
			action.soundFxId = fx;

			if ((fileID >= 0) && (fileID < 0xFF))
				action.soundFileId = fileID;

			for (int i = 0; i < param.Length; i++)
			{
				switch (i)
				{
					case 0: action.soundParam1 = param[0]; break;
					case 1: action.soundParam2 = param[1]; break;
				}
			}

			byte[] res = await SendData(action.GetTestActionData());
		}

		public void PlayAudioFile(byte fileID)
		{
			byte[] p = new byte[2];
			p[0] = EVT_SOUND_TOGGLE;
			p[1] = 0;

			SoundFx(EVT_SOUND_PLAY_ONCE, p, fileID);
		}

		byte PortToMask(string channel)
		{
			int mask = 0;
			int len = channel.Length;
			for (int i = 0; i < len; i++)
			{
				string cs = channel.Substring(i, 1);
				if ((cs == "1") || (cs == "A") || (cs == "a")) mask |= 0x01;
				else if ((cs == "2") || (cs == "B") || (cs == "b")) mask |= 0x02;
				else if ((cs == "3") || (cs == "C") || (cs == "c")) mask |= 0x04;
				else if ((cs == "4") || (cs == "D") || (cs == "d")) mask |= 0x08;
				else if (cs == "5") mask |= 0x10;
				else if (cs == "6") mask |= 0x20;
				else if (cs == "7") mask |= 0x40;
				else if (cs == "8") mask |= 0x80;
			}
			return (byte)mask;
		}

		string MaskToPort(int mask)
		{
			string s = "";
			if ((mask & 0x01) > 0) s += "1 ";
			if ((mask & 0x02) > 0) s += "2 ";
			if ((mask & 0x04) > 0) s += "3 ";
			if ((mask & 0x08) > 0) s += "4 ";
			if ((mask & 0x10) > 0) s += "5 ";
			if ((mask & 0x20) > 0) s += "6 ";
			if ((mask & 0x40) > 0) s += "7 ";
			if ((mask & 0x80) > 0) s += "8 ";

			return s;
		}

	}
}
