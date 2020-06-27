using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace LegoTrainProject
{

    [Serializable]
    public class TrainProgramEvent
    {
        public enum EventType
        {
            Sensor_Triggered = 0,
            User_Triggerd = 1,
			Global_Code = 2
		}

		public enum TriggerType2
		{
			No_Event = 0,
			Color_Change_To = 1,
			Distance_is_below = 2,
			Distance_is_above = 3,
			Button_Plus_is_pressed = 4,
			Button_Minus_is_pressed = 5,
			Button_Stop_is_pressed = 6,
			Button_Power_is_pressed = 7,
			Buttons_are_released = 8,
			Raw_Value_is_below = 9,
			Raw_Value_is_above = 10
		}

		public enum TriggerType1
        {
            No_Event = 0,
            Change_Color_To_Yellow = 1,
            Change_Color_To_Red = 2,
            Change_Color_To_White = 3,
            Distance_is_below_3 = 4,
            Distance_is_below_7 = 5,
			Distance_is_below_30 = 6,
			Distance_is_above_30 = 7,
			Distance_is_Any = 8,
        }

        public enum ActionType
        {
            Do_Nothing = 0,
            Stop_Motor = 1,
            Set_Speed = 2,
            Accelerate_Motor_Over_X_Ms = 3,
            Decelerate_Motor_Over_X_Ms = 4,
            Invert_Speed = 5,
            Wait_X_Ms_And_Set_Speed = 6,
            Set_Speed_For_X_Ms_And_Stop = 7,
            Activate_Switch_Right = 8,
            Activate_Switch_Left = 9,
            Execute_Code = 10,
			PFx_Play_Sound_With_File_Id = 11,
			PFx_Light_Fx = 12,
			Play_Sound = 13
		}

		public enum TrainParamType
        {
            Not_Used = 0,
            Speed = 1,
            Time_In_Ms = 2,
            Port = 3,
            Code = 4,
			PfxFile = 5,
			PfxLight = 6,
			Lights = 7,
			Path = 8
		}

        [NonSerialized]
        public static TrainParamType[][] ParamTypes = new TrainParamType[14][]
        {
            new TrainParamType[] { },
            new TrainParamType[] { TrainParamType.Port },
            new TrainParamType[] { TrainParamType.Port, TrainParamType.Speed },
            new TrainParamType[] { TrainParamType.Port, TrainParamType.Speed, TrainParamType.Time_In_Ms },
            new TrainParamType[] { TrainParamType.Port, TrainParamType.Time_In_Ms },
            new TrainParamType[] { TrainParamType.Port},
            new TrainParamType[] { TrainParamType.Port, TrainParamType.Time_In_Ms, TrainParamType.Speed },
            new TrainParamType[] { TrainParamType.Port, TrainParamType.Time_In_Ms, TrainParamType.Speed },
            new TrainParamType[] { TrainParamType.Port },
            new TrainParamType[] { TrainParamType.Port }, 
            new TrainParamType[] { TrainParamType.Code },
			new TrainParamType[] { TrainParamType.PfxFile },
			new TrainParamType[] { TrainParamType.PfxLight, TrainParamType.Lights },
			new TrainParamType[] { TrainParamType.Path }
		};
		
		[NonSerialized]
        public const string Code = @" 
                using System; 
                using System.Windows.Forms;
                using System.Collections.Generic;
				using System.Threading.Tasks;
				using System.Drawing;
				using System.IO.Ports;

                namespace LegoTrainProject
                {
                    public class DynamicCode
                    {

/*%GlobalCode%*/

                        public static async void ExecuteCode(TrainProgramEvent e, List<Hub> Hub, Sections Sections)
                        {
try
{
							if (TrainProgram.Global == null)
								TrainProgram.Global = new int[100];

                           %Code%
}
catch( Exception ex)
{
MainBoard.WriteLine(""FATAL ERROR In Program: "" + ex.Message);
}
                        }
                    }	
				}";

		public string Name;
		internal int TriggerDistanceParam;
		internal Port.Colors TriggerColorParam;
		internal PFxHub.PFxLightFx PFxLightParam;
		internal string Lights;

		public EventType Type { get; internal set; }
        public string TrainDeviceID { get; internal set; }
		public string TrainPort { get; internal set; }
		public string TargetDeviceID { get; internal set; }
        public string TargetPort { get; internal set; }
        public string CodeToRun { get; internal set; }
        public TriggerType2 Trigger { get; internal set; }
        public ActionType Action { get; internal set; }
        public int[] Param { get; internal set; }
		public string Path { get; internal set; }

		public delegate void EventTriggeredDelegate();

		[field: NonSerialized]
		public event EventTriggeredDelegate EventIsTriggered;

		public bool OnEventIsTriggered()
		{
			if (EventIsTriggered == null)
				return false;

			EventIsTriggered?.Invoke();
			return true;
		}

		public TrainProgramEvent(EventType type)
        {
			
			Type = type;
            TargetPort = "A";
            Param = new int[2];
            CodeToRun = (Type != TrainProgramEvent.EventType.Global_Code) ?
				@"
// Start a motor attached to port B to run at 75% indefinitely
Hub[0].SetMotorSpeed(""A"", 75); 

// Wait 1000ms (1 second)
Wait(1000); 

// Stop the motor attached to port B
Hub[0].Stop(""A""); 

            " :
			@"
// The code in this section will be available in all other programs! //
///////////////////////////////////////////////////////////////////////


// Create your own enums
public enum MyEnum
{
   MyYellowTrain = 0,
   MyRedTrain = 1
}

// Create your own helper functions
public void MyGlobalFunction()
{
   // Execute your code here
}

// Create your own constants!
public const int RED_TRAIN = 1;
public const int YELLOW_TRAIN = 2;

            "
			;
        }

        internal string GetParamText(int paramId)
        {
			if (paramId == 0 && Action == ActionType.PFx_Light_Fx)
				return Lights;
			else if (paramId == 0 && Action == ActionType.PFx_Play_Sound_With_File_Id)
				return Param[paramId].ToString();
			else if (paramId == 0 && Action == ActionType.Play_Sound)
                return Path;
			else if (paramId + 1 < ParamTypes[(int)Action].Length)
				return Param[paramId].ToString();
			else
				return "Not Used";
        }

        internal bool HasParam(int paramId)
        {
			if (paramId == 0 && Action == ActionType.PFx_Light_Fx)
				return true;
			else if (paramId == 0 && Action == ActionType.PFx_Play_Sound_With_File_Id)
				return true;
			else if (paramId == 0 && Action == ActionType.Play_Sound)
				return true;
			return (paramId + 1 < ParamTypes[(int)Action].Length);
        }

        internal bool HasPortSelection()
        {
            return (ParamTypes[(int)Action].Length > 0 && ParamTypes[(int)Action][0] == TrainParamType.Port);
        }

        internal bool HasCodeEdition()
        {
            return (ParamTypes[(int)Action].Length > 0 && ParamTypes[(int)Action][0] == TrainParamType.Code);
        }

		internal bool HasPfxLightParam()
		{
			return (ParamTypes[(int)Action].Length > 0 && ParamTypes[(int)Action][0] == TrainParamType.PfxLight);
		}
	}

    [Serializable]
    public class TrainProgram
    {
        [NonSerialized]
        public List<Hub> hubs;


		[NonSerialized]
		public Sections sections;

		/// <summary>
		/// All Events of the program
		/// </summary>
		public List<TrainProgramEvent> Events = new List<TrainProgramEvent>();

        /// <summary>
        ///  Name of the program
        /// </summary>
        public string Name = "New Program";

		/// <summary>
		/// 
		/// </summary>
		[NonSerialized]
		public bool IsRunning = false;

		/// <summary>
		/// 
		/// </summary>
		public static int[] Global = new int[100];

		/// <summary>
		/// 
		/// </summary>
		private TrainProgramEvent GlobalCode;

		public bool Init(TrainProject project)
		{
			sections = project.Sections;
			hubs = project.RegisteredTrains;
			if (hubs == null || hubs.Count == 0)
				return false;

			foreach (Hub t in hubs)
			{
				t.State = (t.State == null) ? new int[100] : t.State;
			}

			// Make sure GlobalStates exist
			Global = (Global == null) ? new int[100] : Global;
			
			// Make sure GlobalCode exist
			GlobalCode = (project.GlobalCode == null) ? new TrainProgramEvent(TrainProgramEvent.EventType.Global_Code) : project.GlobalCode;

			return true;
		}


		public bool Start(TrainProject project)
        {
			if (IsRunning)
				return true;

			// We first initialize the program
            if (!Init(project))
                return false;

			// Then we plug all event handlers
            foreach (Hub t in hubs)
            {
                t.ColorTriggered += ColorTriggeredHandler;
                t.DistanceTriggered += DistanceTriggeredHandler;
				t.RemoteTriggered += RemoteTriggeredHandler;
            }

            MainBoard.WriteLine($"All Sensor Events for {Name} are active.", Color.DarkMagenta);
			IsRunning = true;

			return true;
        }

		public void Stop()
        {
            // If we never started this program, nothing to stop
            if (hubs == null)
                return;

			// Otherwise let's clear all events
			foreach (Hub t in hubs)
			{
				t.ColorTriggered -= ColorTriggeredHandler;
				t.DistanceTriggered -= DistanceTriggeredHandler;
				t.RemoteTriggered -= RemoteTriggeredHandler;
			}

			IsRunning = false;
			MainBoard.WriteLine($"Sensor Events for Program '{Name}' are stopped.");
        }

		private void RemoteTriggeredHandler(Hub train, Port p, Port.RemoteButtons button)
		{
			if (hubs == null || hubs.Count == 0)
			{
				MainBoard.WriteLine("Remote event received, but program is not started. Start program first");
				return;
			}

			foreach (TrainProgramEvent e in Events)
			{
				if (e.TrainDeviceID == train.DeviceId && (e.TrainPort == null || p == null || e.TrainPort == p.Id))
				{
					if (e.Trigger == TrainProgramEvent.TriggerType2.Button_Plus_is_pressed && button == Port.RemoteButtons.BUTTON_PLUS)
					{
						MainBoard.WriteLine("Event #" + (Events.IndexOf(e) + 1) + " triggered - " + train.Name + " PLUS button pressed on port " + p.Id, Color.Purple);
						ActivateAction(e);
					}
					else if (e.Trigger == TrainProgramEvent.TriggerType2.Button_Minus_is_pressed && button == Port.RemoteButtons.BUTTON_MINUS)
					{
						MainBoard.WriteLine("Event #" + (Events.IndexOf(e) + 1) + " triggered - " + train.Name + " MINUS button pressed on port " + p.Id, Color.Purple);
						ActivateAction(e);
					}
					else if (e.Trigger == TrainProgramEvent.TriggerType2.Button_Stop_is_pressed && button == Port.RemoteButtons.BUTTON_STOP)
					{
						MainBoard.WriteLine("Event #" + (Events.IndexOf(e) + 1) + " triggered - " + train.Name + " STOP button pressed on port " + p.Id, Color.Purple);
						ActivateAction(e);
					}
					else if (e.Trigger == TrainProgramEvent.TriggerType2.Button_Power_is_pressed && button == Port.RemoteButtons.BUTTON_POWER)
					{
						MainBoard.WriteLine("Event #" + (Events.IndexOf(e) + 1) + " triggered - " + train.Name + " POWER button pressed", Color.Purple);
						ActivateAction(e);
					}
					else if (e.Trigger == TrainProgramEvent.TriggerType2.Buttons_are_released && button == Port.RemoteButtons.BUTTON_RELEASED)
					{
						MainBoard.WriteLine("Event #" + (Events.IndexOf(e) + 1) + " triggered - " + train.Name + " had a button released", Color.Purple);
						ActivateAction(e);
					}
				}
			}
		}

		private void ColorTriggeredHandler(Hub train, Port port, Port.Colors color)
        {
            if (hubs == null || hubs.Count == 0)
            {
                MainBoard.WriteLine("Color event received, but program is not started. Start program first");
                return;
            }

            foreach (TrainProgramEvent e in Events)
            {
                if (e.TrainDeviceID == train.DeviceId)
                {
                    if (e.Trigger == TrainProgramEvent.TriggerType2.Color_Change_To && color == e.TriggerColorParam && (e.TrainPort == null || e.TrainPort == port.Id))
                    {
						port.LastColorTick = Environment.TickCount;
						MainBoard.WriteLine("Event #" + (Events.IndexOf(e) + 1) + " triggered - " + train.Name + " sensor detected " + color.ToString(), Color.Purple);
                        ActivateAction(e);
                    }
                }
            }
        }

        private void DistanceTriggeredHandler(Hub train, Port port, int distance)
        {
            if (hubs == null || hubs.Count == 0)
            {
                MainBoard.WriteLine("Distance event received, but program is not started. Start program first");
                return;
            }

            foreach (TrainProgramEvent e in Events)
            {
                if (e.TrainDeviceID == train.DeviceId && (e.TrainPort == null || e.TrainPort == port.Id))
                {
                    if ((e.Trigger == TrainProgramEvent.TriggerType2.Distance_is_above || e.Trigger == TrainProgramEvent.TriggerType2.Raw_Value_is_above) && distance > e.TriggerDistanceParam)
                    {
						port.LastDistanceTick = Environment.TickCount;
						MainBoard.WriteLine("Event #" + (Events.IndexOf(e) + 1) + " triggered - " + train.Name + " on port " + port.Id + ((train.Type == Hub.Types.EV3) ? " has a raw value " : " distance ") + "above " + distance, Color.Purple);
                        ActivateAction(e);
                    }
					else if ((e.Trigger == TrainProgramEvent.TriggerType2.Distance_is_below || e.Trigger == TrainProgramEvent.TriggerType2.Raw_Value_is_below) && distance < e.TriggerDistanceParam)
					{
						port.LastDistanceTick = Environment.TickCount;
						MainBoard.WriteLine("Event #" + (Events.IndexOf(e) + 1) + " triggered - " + train.Name + " on port " + port.Id + ((train.Type == Hub.Types.EV3) ? " has raw value " : " has a distance ") + "below " + distance, Color.Purple);
						ActivateAction(e);
					}
                }
            }
        }

        internal void StartSequence(TrainProgramEvent trainEvent)
        {
			if (hubs == null)
			{
				MainBoard.WriteLine($"You need at least 1 registered hub to start a program. '{Name}' cannot be started.", Color.Red);
				return;
			}

			// Make sure to execute code and nothing else
			trainEvent.Action = TrainProgramEvent.ActionType.Execute_Code;

            // Let's roll!
            ActivateAction(trainEvent);
        }

        public void ActivateAction(TrainProgramEvent e)
        {
			Hub target = GetTrain(e.TargetDeviceID);

            if (target == null && e.Action != TrainProgramEvent.ActionType.Execute_Code && e.Action != TrainProgramEvent.ActionType.Play_Sound)
            {
                MainBoard.WriteLine($"Target train '{ e.TargetDeviceID}' is not connected");
                return;
            }

            switch (e.Action)
            {
                case TrainProgramEvent.ActionType.Stop_Motor:
                    {
                        MainBoard.WriteLine("Action Tiggered => Stop Port " + e.TargetPort + " of " + target.Name);
                        target.Stop(e.TargetPort, true);
                        break;
                    }
                case TrainProgramEvent.ActionType.Set_Speed:
                    {
                        MainBoard.WriteLine($"{target.Name} Action Tiggered => Set Motor Speed of Port {e.TargetPort} to {e.Param[0]} for {target.Name}");
                        target.SetMotorSpeed(e.TargetPort, e.Param[0]);
                        break;
                    }
                case TrainProgramEvent.ActionType.Invert_Speed:
                    {
						Port targetPort = target.GetPortFromPortId(e.TargetPort);

						MainBoard.WriteLine($"{target.Name} Action Tiggered => Invert Speed of Port {e.TargetPort} to {-targetPort.Speed} for {target.Name}");
                        target.SetMotorSpeed(e.TargetPort, -targetPort.Speed);
                        break;
                    }
                case TrainProgramEvent.ActionType.Accelerate_Motor_Over_X_Ms:
                    {
						Port targetPort = target.GetPortFromPortId(e.TargetPort);

						MainBoard.WriteLine($"{target.Name} Action Tiggered => Accelerate Port {e.TargetPort} to {e.Param[0]} over {e.Param[1]}ms");
                        target.RampMotorSpeed(e.TargetPort, e.Param[0], e.Param[1]);
                        break;
                    }
                case TrainProgramEvent.ActionType.Decelerate_Motor_Over_X_Ms:
                    {
						Port targetPort = target.GetPortFromPortId(e.TargetPort);

						MainBoard.WriteLine($"{target.Name} Action Tiggered => Decelerate Port {e.TargetPort} to 0 over {e.Param[0]}ms");
                        target.RampMotorSpeed(e.TargetPort, 0, e.Param[0]);
                        break;
                    }
                case TrainProgramEvent.ActionType.Wait_X_Ms_And_Set_Speed:
                    {
                        MainBoard.WriteLine($"{target.Name} Action Tiggered => Wait {e.Param[0]}ms and Set Speed of Port {e.TargetPort} to {e.Param[1]}");
                        target.Stop();
                        System.Timers.Timer t = new System.Timers.Timer();
                        t.Interval = e.Param[0];
                        t.Elapsed += (object sender, ElapsedEventArgs ev) =>
                        {
                            target.SetMotorSpeed(e.TargetPort, e.Param[1]);
                            t.Stop();
                            t.Dispose();
                        };
                        t.Start();
                        break;
                    }

                case TrainProgramEvent.ActionType.Set_Speed_For_X_Ms_And_Stop:
                    {
                        MainBoard.WriteLine($"{target.Name} Action Tiggered => Set Speed on Port {e.TargetPort} to {e.Param[0]} And Stop after {e.Param[1]}ms");
                        target.SetMotorSpeed(e.TargetPort, e.Param[1]);
                        System.Timers.Timer t = new System.Timers.Timer(e.Param[0]);
                        t.Elapsed += (object sender, ElapsedEventArgs ev) =>
                        {
                            target.Stop(e.TargetPort);
                            t.Stop();
                            t.Dispose();
                        };
                        t.Start();
                        break;
                    }
                case TrainProgramEvent.ActionType.Activate_Switch_Left:
                case TrainProgramEvent.ActionType.Activate_Switch_Right:
                    {
						Port targetPort = target.GetPortFromPortId(e.TargetPort);

						targetPort.TargetSpeed = (e.Action == TrainProgramEvent.ActionType.Activate_Switch_Left) ? -100 : 100;
                        MainBoard.WriteLine($"{target.Name} Action Tiggered => Activate Switch to the " + ((targetPort.TargetSpeed == -100) ? "Left" : "Right"));

						// Then trigger the motor
						if (e.Action == TrainProgramEvent.ActionType.Activate_Switch_Left)
							target.ActivateSwitchToLeft(e.TargetPort);
						else
							target.ActivateSwitchToRight(e.TargetPort);
                        break;
                    }
				case TrainProgramEvent.ActionType.PFx_Light_Fx:
					{
						PFxHub hub = (PFxHub)target;
						MainBoard.WriteLine($"{target.Name} Action Tiggered => Activate PFx Light FX " + e.PFxLightParam.ToString() + " on lights " + e.Lights);

						hub.LightFx(e.Lights, (byte)e.PFxLightParam, new byte[0]);
						break;
					}
				case TrainProgramEvent.ActionType.PFx_Play_Sound_With_File_Id:
					{
						PFxHub hub = (PFxHub)target;
						MainBoard.WriteLine($"{target.Name} Action Tiggered => Play PFx Sound from file Id " + e.Param[0]);

						hub.PlayAudioFile((byte)e.Param[0]);
						break;
					}
				case TrainProgramEvent.ActionType.Play_Sound:
					{
						MainBoard.WriteLine($"{target.Name} Action Tiggered => Play Sound " + e.Path);
						try
						{
							System.Media.SoundPlayer player = new System.Media.SoundPlayer(e.Path);
							player.Play();
						}
						catch(Exception ex)
						{
							MainBoard.WriteLine("Error while trying to play a sound: " + ex.Message, Color.Red);
						}

						break;
					}
				case TrainProgramEvent.ActionType.Execute_Code:
                    {
						// If we have an internal event code, we execute and stop
						if (e.OnEventIsTriggered())
							return;

						Hub source = GetTrain(e.TrainDeviceID);
						int currentTrainIndex = hubs.IndexOf(source);

						string code = TrainProgramEvent.Code.Replace("%Code%", e.CodeToRun.Replace("Wait(", "await Task.Delay("));
						code = code.Replace("/*%GlobalCode%*/", GlobalCode.CodeToRun.Replace("Wait(", "await Task.Delay("));
						code = code.Replace("%CurrentTrainIndex%", currentTrainIndex.ToString());

						CSharpCodeProvider provider = new CSharpCodeProvider();
                        CompilerParameters parameters = new CompilerParameters();
                        
                        // Reference to System.Drawing library
                        parameters.ReferencedAssemblies.Add("System.dll");
                        parameters.ReferencedAssemblies.Add("LegoTrainProject.exe");
                        parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
						parameters.ReferencedAssemblies.Add("System.Drawing.dll");
						// True - memory generation, false - external file generation
						parameters.GenerateInMemory = true;
                        // True - exe file generation, false - dll file generation
                        parameters.GenerateExecutable = false;

                        // Run the code!
                        CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);

                        if (results.Errors.HasErrors)
                        {
                            StringBuilder sb = new StringBuilder();

                            foreach (CompilerError error in results.Errors)
                            {
                                MainBoard.WriteLine(String.Format("Compilation Error ({0}): {1}", error.ErrorNumber, error.ErrorText), Color.Red);
                            }
                        }
                        else
                        {
							Assembly assembly = results.CompiledAssembly;
                            Type program = assembly.GetType("LegoTrainProject.DynamicCode");
                            MethodInfo main = program.GetMethod("ExecuteCode");

							try
                            {
								ThreadStart threadMain = delegate () { main.Invoke(null, new object[] { e, this.hubs, sections }); };
								new System.Threading.Thread(threadMain).Start();
                            }
                            catch (Exception ex)
                            {
                                MainBoard.WriteLine("Exception while executing your sequence: " + ex.InnerException);
                            }
                            
                        }

                        break;
                    }

            }
        }

		private Hub GetTrain(string trainDeviceID)
        {
            if (hubs == null)
                return null;

            foreach (Hub t in hubs)
                if (t.DeviceId == trainDeviceID)
                    return t;

            return null;
        }
    }
}
