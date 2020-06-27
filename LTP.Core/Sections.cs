using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoTrainProject
{
	[Serializable]
	public class Section
	{
		public enum ActionOnRelease
		{
			Resume_Speed = 0,
			Execute_Code = 1,
			Do_Nothing = 2
		};

		/// <summary>
		/// Carries the custom code to execute
		/// </summary>
		public TrainProgramEvent CustomCodeEvent;

		/// <summary>
		/// What do we do when the next Section is free
		/// </summary>
		public ActionOnRelease Action = ActionOnRelease.Resume_Speed;

		/// <summary>
		/// Name of the section
		/// </summary>
		public string Name { get; internal set; }

		/// <summary>
		/// Detector at the end of this section
		/// </summary>
		public string Detector;

		/// <summary>
		/// ID + Port of the switch of this section
		/// </summary>
		public string Switch;

		/// <summary>
		/// All Switch Related stuff
		/// </summary>
		[NonSerialized]
		public Hub Switch1Hub;

		[NonSerialized]
		public string Switch1Port;

		/// <summary>
		/// Left or Straight section forward
		/// </summary>
		public Section LeftSection;

		/// <summary>
		/// Right Sections
		/// </summary>
		public Section RightSection;

		/// <summary>
		/// Train currently on this section
		/// </summary>
		public Hub CurrentHub;

		/// <summary>
		/// Train currently on this section
		/// </summary>
		public Hub ReservedBy;

		[NonSerialized]
		public bool IsBeingCleared = false;

		/// <summary>
		/// Speed of the trains on this section
		/// </summary>
		internal int MaxSpeed = 100;

		/// <summary>
		/// Need not one, but the next 2 sections released before moving forward
		/// </summary>
		internal bool NeedsTwoSectionReleased = false;

		internal bool IsDetectorPresent(Hub h, Port p)
		{
			if (Detector == null)
				return false;

			string[] parts = Detector.Split(new char[] { '*' });
			return (parts.Length > 1 && parts[0] == p.Id && parts[1] == h.DeviceId);
		}

		internal string GetDetectorDeviceId()
		{
			if (Detector == null)
				return null;

			string[] parts = Detector.Split(new char[] { '*' });
			return parts[1];
		}

		internal string GetDetectorPort()
		{
			if (Detector == null)
				return null;

			string[] parts = Detector.Split(new char[] { '*' });
			return parts[0];
		}

		internal bool IsSwitchPresent(Hub h, Port p)
		{
			if (Switch == null)
				return false;

			string[] parts = Switch.Split(new char[] { '*' });
			return (parts.Length > 1 && parts[0] == p.Id && parts[1] == h.DeviceId);
		}

		internal string GetSwitchDeviceId()
		{
			if (Switch == null)
				return null;

			string[] parts = Switch.Split(new char[] { '*' });
			return parts[1];
		}

		internal string GetSwitchPort()
		{
			if (Switch == null)
				return null;

			string[] parts = Switch.Split(new char[] { '*' });
			return parts[0];
		}


		internal bool IsConnectedForward(Section destinationSection)
		{
			return (LeftSection == destinationSection || RightSection == destinationSection);
		}

		public int CountForwardSections()
		{
			return (Switch1Hub != null || RightSection != null) ? 2 : 1;
		}

		public override string ToString()
		{
			return Name;
		}
	}

	[Serializable]
	public class Sections : IEnumerable<Section>
	{
		/// <summary>
		/// Allow to let UI know when things have changed!
		/// </summary>
		public delegate void RefreshUIThreadSafeDelegate();

		[field: NonSerialized]
		public event RefreshUIThreadSafeDelegate DataUpdated;

		public void OnDataUpdated()
		{
			DataUpdated?.Invoke();
		}

		/// <summary>
		/// All the sections!
		/// </summary>
		public List<Section> sections = new List<Section>();

		/// <summary>
		/// Is the Section Program running?
		/// </summary>
		[NonSerialized]
		private TrainProgram program = new TrainProgram();

		/// <summary>
		/// Is the program running
		/// </summary>
		public bool IsRunning { get => (program != null) ? program.IsRunning : false; }

		/// <summary>
		/// List of all the paths a train can take
		/// </summary>
		public List<Path> Paths = new List<Path>();

		/// <summary>
		/// Accessor for the Sections
		/// </summary>
		/// <returns></returns>
		public List<Section> GetAll()
		{
			return sections;
		}

		/// <summary>
		/// Look up a section by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Section FindSection(string name)
		{
			foreach (Section s in sections)
				if (s.Name == name)
					return s;

			return null;
		}

		/// <summary>
		/// Clear a hub from all the network of section
		/// </summary>
		/// <param name="hub"></param>
		/// <returns></returns>
		public Section ClearHub(Hub hub)
		{
			foreach (Section s in sections)
				if (s.CurrentHub == hub)
					s.CurrentHub = null;

			return null;
		}

		/// <summary>
		/// Try to reserve a section. If occupied, will wait for it to relax
		/// </summary>
		/// <param name="sectionName"></param>
		/// <param name="hub"></param>
		/// <param name="portName"></param>
		/// <returns></returns>
		public async Task Reserve(Section nextSection, Section currentSection, Hub hub)
		{
			Section nextNextSection = null;

			if (nextSection == null || hub == null || hub.IsWaitingSection || nextSection.CurrentHub == hub)
			{
				return;
			}

			Hub dectector = program.hubs.Find(s => s.DeviceId == currentSection.GetDetectorDeviceId());

			while (!hub.AbortReserve && IsRunning)
			{
				// Should we exit the loop?
				if (nextSection.CurrentHub == null && (nextSection.ReservedBy == null || nextSection.ReservedBy == hub))
					break;

				hub.IsWaitingSection = true;
				MainBoard.WriteLine($"{hub.Name} - stopped because " + nextSection.Name + " is occupied", Color.Red);
				hub.Stop(hub.TrainMotorPort, true);
				
				// We cannot go. Turn light to Red
				if (dectector != null)
					dectector.SetLEDColor(Port.Colors.RED);

				await Task.Delay(1000).ConfigureAwait(false);
			}

			// Do we need to leave early?
			if (!IsRunning || hub.AbortReserve)
			{
				MainBoard.WriteLine($"{hub.Name} - wait is aborted!", Color.Red);
				hub.AbortReserve = false;
				hub.IsWaitingSection = false;
				return;
			}

			// We clear the section but keep reserving it
			currentSection.IsBeingCleared = true;
			currentSection.ReservedBy = hub;
			currentSection.CurrentHub = null;

			// We step in the new section
			nextSection.CurrentHub = hub;

			// move forward with the expected next section
			hub.MoveToNextSectionIndex();

			// We get the next next section
			int nextNextSectionId = hub.GetNextSectionIndex();

			if (nextNextSectionId != -1)
				nextNextSection = sections[nextNextSectionId];

			int resumingSpeed = nextSection.MaxSpeed;

			// Is this section linked with another one?
			if (nextSection.NeedsTwoSectionReleased && nextNextSection != null)
			{
				// We wait for the next next section too
				while (!hub.AbortReserve && IsRunning)
				{
					if (nextNextSection.CurrentHub == null && nextNextSection.ReservedBy == null)
						break;

					MainBoard.WriteLine($"{hub.Name} - stopped because " + sections[nextNextSectionId].Name + " is occupied and we need 2 sections cleared.", Color.Red);
					hub.Stop(hub.TrainMotorPort, true);
					await Task.Delay(1000).ConfigureAwait(false);
				}

				nextNextSection.ReservedBy = hub;
			}

			if (currentSection.CountForwardSections() > 1 && currentSection.Switch1Hub != null)
			{
				if (!nextSection.NeedsTwoSectionReleased && 
					((currentSection.Switch1Hub.IsSwitchOnTheLeft(currentSection.Switch1Port) && (currentSection.LeftSection == nextSection)) ||
					(!currentSection.Switch1Hub.IsSwitchOnTheLeft(currentSection.Switch1Port) && (currentSection.RightSection == nextSection))))
					MainBoard.WriteLine($"{hub.Name} - Switch already in position!", Color.Magenta);
				else
				{
					MainBoard.WriteLine($"{hub.Name} - We stop to move switch!", Color.Red);
					hub.Stop(hub.TrainMotorPort, true);
					currentSection.Switch1Hub.ActivateSwitch(currentSection.Switch1Port, (currentSection.LeftSection == nextSection));

					// We can go - turn back to Green
					if (dectector != null)
						dectector.SetLEDColor(Port.Colors.RED);

					// We update the visual
					OnDataUpdated();

					// Wait 2 seconds to allow switch to move
					await Task.Delay(2000).ConfigureAwait(false);

					if (!IsRunning)
					{
						// We clean up our tail earlier because of program abort
						hub.AbortReserve = false;
						hub.IsWaitingSection = false;
						return;
					}
				}
			}

			// We update the visual
			OnDataUpdated();

			// Will we have to slow downn ahead of us?
			if (nextSection.CountForwardSections() > 1 && nextNextSectionId != -1 && !nextSection.NeedsTwoSectionReleased)
			{
				if ((!nextSection.Switch1Hub.IsSwitchOnTheLeft(nextSection.Switch1Port) && (nextSection.LeftSection == sections[nextNextSectionId])) ||
					(nextSection.Switch1Hub.IsSwitchOnTheLeft(nextSection.Switch1Port) && (nextSection.RightSection == sections[nextNextSectionId])))
				{
					MainBoard.WriteLine($"{hub.Name} - Switch Ahead! We slow down", Color.Red);
					resumingSpeed = hub.SpeedWhenAboutToStop;
				}
			}
			else if (nextSection.CountForwardSections() == 1 && nextNextSectionId != -1)
			{
				if (sections[nextNextSectionId].CurrentHub != null && sections[nextNextSectionId].CurrentHub != hub)
				{
					MainBoard.WriteLine($"{hub.Name} - Next section is not cleared! We slow down", Color.Red);
					resumingSpeed = hub.SpeedWhenAboutToStop;
				}
			}
			else if (nextNextSectionId == -1)
			{
				MainBoard.WriteLine($"{hub.Name} - End of the path is ahead! We slow down", Color.Red);
				resumingSpeed = hub.SpeedWhenAboutToStop;
			}

			// We can go - turn back to Green
			if (dectector != null)
				dectector.SetLEDColor(Port.Colors.GREEN);

			// Resume train activity
			if (IsRunning && nextSection.Action == Section.ActionOnRelease.Resume_Speed)
			{
				hub.SetMotorSpeed(hub.TrainMotorPort, (int)(resumingSpeed * hub.SpeedCoefficient));
				MainBoard.WriteLine($"{hub.Name} - Allowed to move to " + nextSection.Name, System.Drawing.Color.Green);
			}
			else if (IsRunning && nextSection.Action == Section.ActionOnRelease.Execute_Code)
			{
				if (nextSection.CustomCodeEvent != null)
				{
					MainBoard.WriteLine($"{hub.Name} - Allowed to move to " + nextSection.Name + " via custom code", System.Drawing.Color.Green);
					nextSection.CustomCodeEvent.TrainDeviceID = hub.DeviceId;
					nextSection.CustomCodeEvent.TrainPort = hub.TrainMotorPort;
					program.StartSequence(nextSection.CustomCodeEvent);
				}
			}

			// We can be stopped again
			hub.IsWaitingSection = false;

			int waitingCount = 0;
			while (waitingCount < 10 && IsRunning && !hub.AbortReserve)
			{
				// We wait 3s to make sure the train has left
				await Task.Delay(hub.ClearingTimeInMs / 10).ConfigureAwait(false);
				waitingCount++;
			}

			// Then we release all section
			currentSection.ReservedBy = null;

			// We are officially gone
			currentSection.IsBeingCleared = false;
			
			MainBoard.WriteLine(hub.Name + " has cleared section " + currentSection.Name, System.Drawing.Color.Green);

			// Let the world know something new happened
			OnDataUpdated();
		}

		/// <summary>
		/// Release a section from its current hub
		/// </summary>
		/// <param name="sectionName"></param>
		public void Release(string sectionName)
		{
			Section currentSection = FindSection(sectionName);
			if (currentSection != null)
				currentSection.CurrentHub = null;
		}

		/// <summary>
		/// Look for the index of a section by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		internal int FindSectionIndexByName(string name)
		{
			for (int i = 0; i < sections.Count; i++)
				if (sections[i].Name == name)
					return i;

			return -1;
		}

		/// <summary>
		/// Is the current train anywhere in the network?
		/// </summary>
		/// <param name="train"></param>
		/// <returns></returns>
		internal bool IsTrainInNetwork(Hub train)
		{
			foreach (Section s in sections)
				if (s.CurrentHub == train)
					return true;

			return false;
		}


		//////////////////////////
		///
		///  Helpers
		///  
		//////////////////////////
		


		public IEnumerator<Section> GetEnumerator()
		{
			return sections.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Section this[int index]
		{
			get
			{
				return sections[index];
			}
		}

		internal void Start(TrainProject project)
		{
			// We make sure to have a program
			program = program ?? new TrainProgram();
			program.Name = "Self-Driving";

			// We make sure to clear any previous events
			program.Events.Clear();

			for (int j = 0; j < sections.Count; j++)
			{
				Section s = sections[j];
				s.IsBeingCleared = false;
				s.ReservedBy = null;

				s.Switch1Hub = project.GetHubIndexByDeviceId(s.GetSwitchDeviceId());
				s.Switch1Port = s.GetSwitchPort();
				
				Hub detector = project.GetHubIndexByDeviceId(s.GetDetectorDeviceId());
				if (detector != null)
					detector.SetLEDColor(Port.Colors.GREEN);

				TrainProgramEvent newEvent = new TrainProgramEvent(TrainProgramEvent.EventType.Sensor_Triggered);
				newEvent.TrainDeviceID = s.GetDetectorDeviceId();
				newEvent.TrainPort = s.GetDetectorPort();
				newEvent.Trigger = TrainProgramEvent.TriggerType2.Distance_is_below;
				newEvent.TriggerDistanceParam = 3;

				newEvent.Action = TrainProgramEvent.ActionType.Execute_Code;
				newEvent.EventIsTriggered += async () =>
				{
					//MainBoard.WriteLine($"{newEvent.TrainDeviceID} on port {newEvent.TrainPort} was triggered on Section {s.Name}", Color.Blue);
					Section currentSection = s;
					Hub currentTrain = currentSection.CurrentHub;

					if (currentTrain == null)
					{
						MainBoard.WriteLine("Event discarded - Unknown train on " + currentSection.Name + ", nothing done");
						return;
					}

					if (!currentTrain.IsPathProgramRunning)
					{
						MainBoard.WriteLine($"{currentTrain.Name} - Event discarded - Train is not running");
						return;
					}

					// If we are still waiting to move forward, we ignore the event
					if (currentTrain.IsWaitingSection)
					{
						MainBoard.WriteLine($"{currentTrain.Name} - Event discarded - Waiting to for next section.");
						return;
					}

					if (currentSection.IsBeingCleared)
					{
						MainBoard.WriteLine($"{currentSection.Name} - Event discarded - Leaving the section.");
						return;
					}

					await ReserveNextTrainSection(currentSection, currentTrain);

				};

				// And then we start it!
				program.Events.Add(newEvent);
			}

			if (!program.Start(project))
				MainBoard.WriteLine("You need at least one hub connected to launch this program", Color.Red);
			else
				MainBoard.WriteLine("Anti-Collision is active over " + sections.Count + " sections! ");
		}

		public async Task ReserveNextTrainSection(Section currentSection, Hub currentTrain)
		{
			// Get next section to go to
			int nextSectionId = currentTrain.GetNextSectionIndex();

			// We check that the next section is valid per the current network
			if (nextSectionId != -1 && nextSectionId < sections.Count && currentSection.IsConnectedForward(sections[nextSectionId]))
			{
				// Then await to be authorized!
				await Reserve(sections[nextSectionId], currentSection, currentTrain);
			}
			else
			{
				if (nextSectionId == -1)
				{
					currentTrain.Stop();
					currentTrain.IsPathProgramRunning = false;
					MainBoard.WriteLine($"{currentSection.Name} has to stop because the path has been completed.", Color.Red);
				}
				else
				{
					currentTrain.Stop();
					MainBoard.WriteLine($"{currentSection.Name} has been stopped because Section " + nextSectionId + " is not a valid section forward. Check your path definition.");
				}
			}
		}

		internal void ClearAllTrains()
		{
			foreach (Section s in sections)
			{
				s.CurrentHub = null;
			}
		}

		internal void Stop()
		{
			program.Stop();
			program.Events.Clear();
		}
	}
}
