using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegoTrainProject
{
    [Serializable]
    public class Port
    {
		public enum Devices {
            UNKNOWN = 0,
            BASIC_MOTOR = 1,
            TRAIN_MOTOR = 2,
            LED_LIGHTS = 8,
            BOOST_LED = 22,
            WEDO2_TILT = 34,
            WEDO2_DISTANCE = 35,
            BOOST_DISTANCE = 37,
            BOOST_EXT_MOTOR = 38,
            BOOST_MOTOR = 39,
            BOOST_TILT = 40,
            DUPLO_TRAIN_BASE_MOTOR = 41,
            DUPLO_TRAIN_BASE_SPEAKER = 42,
            DUPLO_TRAIN_BASE_COLOR = 43,
            DUPLO_TRAIN_BASE_SPEEDOMETER = 44,
			CONTROL_PLUS_L_MOTOR = 46,
			CONTROL_PLUS_XL_MOTOR = 47,
			POWERED_UP_REMOTE_BUTTON = 55,
			EV3_SENSOR = 400,
			EV3_MOTOR = 401,
			NXT_SENSOR = 402,
			NEXT_MOTOR = 403,
			EV3_COLOR_SENSOR = 404
		};

		public enum Functions
		{
			NOT_USED = 0,
			MOTOR = 1,
			LIGHT = 2,
			SWITCH_STANDARD = 3,
			SENSOR = 4,
			SWITCH_DOUBLECROSS = 5,
			SWITCH_TRIXBRIX = 6,
			TRAIN_MOTOR = 7,
			BUTTON = 8,
			PFX_SPEAKER = 9,
			PFX_LIGHT_CONTROLER = 10
		}

		public enum RemoteButtons
		{
			BUTTON_PLUS = 0,
			BUTTON_MINUS = 1,
			BUTTON_STOP = 2,
			BUTTON_POWER = 3,
			BUTTON_RELEASED = 4
		}


		public string Id;
        public int Value;
		public Devices Device = Devices.UNKNOWN;
		public Functions Function = Functions.NOT_USED;

        [NonSerialized]
        public bool Connected = false;

        [NonSerialized]
        public bool Busy = false;

		[NonSerialized]
		public Label label;

		/// <summary>
		/// Public Speed 
		/// </summary>
		[NonSerialized]
		public int Speed = 0;

		/// <summary>
		/// Timer to accelerate and decelerate a motor
		/// </summary>
		[NonSerialized]
		public System.Timers.Timer MotorTimer;

		/// <summary>
		/// Last Color Traversed
		/// </summary>
		[NonSerialized]
		public Colors LatestColor = Colors.BLACK;

		[NonSerialized]
		public long LastColorTick;

		[NonSerialized]
		public int LatestDistance = 0;

		[NonSerialized]
		public long LastDistanceTick = 0;

		public int MaxDistance = 0;
		public int MinDistance = 0;

		/// <summary>
		/// The 10 colors detectable
		/// </summary>
		public enum Colors
		{
			BLACK = 0,
			PINK = 1,
			PURPLE = 2,
			BLUE = 3,
			LBLUE = 4,
			CYAN = 5,
			GREEN = 6,
			YELLOW = 7,
			ORANGE = 8,
			RED = 9,
			WHITE = 10,
			NONE = 255
		}

		public static Bitmap[] colorBitmaps = new Bitmap[]
		{
			Properties.Resources.connected,
			Properties.Resources.pink,
			Properties.Resources.purple,
			Properties.Resources.blue,
			Properties.Resources.lblue,
			Properties.Resources.cyan,
			Properties.Resources.connected,
			Properties.Resources.yellow,
			Properties.Resources.orange,
			Properties.Resources.red,
			Properties.Resources.white,
			Properties.Resources.connected
		};

		/// <summary>
		/// Public Speed 
		/// </summary>
		public int TargetSpeed = 100;

		/// <summary>
		/// How often can we trigger these sensors?
		/// </summary>
		private int distanceColorCooldownMs = 2000;
		public int DistanceColorCooldownMs { get => (distanceColorCooldownMs == 0) ? 2000 : distanceColorCooldownMs; set => distanceColorCooldownMs = value; }

		public Port(string id, int value)
        {
            this.Id = id;
            this.Value = value;
            this.Device = Devices.UNKNOWN;
            this.Connected = false;
        }

        public Port(string id, int value, bool connected)
        {
            this.Id = id;
            this.Value = value;
            this.Device = Devices.UNKNOWN;
            this.Connected = connected;
        }
    }
}
