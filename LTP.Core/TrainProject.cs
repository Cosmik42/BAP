using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace LegoTrainProject
{
    [Serializable]
    public class TrainProject
    {
        public List<Hub> RegisteredTrains = new List<Hub>();
        public List<TrainProgram> Programs = new List<TrainProgram>();
		public TrainProgramEvent GlobalCode = new TrainProgramEvent(TrainProgramEvent.EventType.Global_Code);
		public Sections Sections = new Sections();
		public bool ShowSectionProgram = false;

        [NonSerialized]
        public string Path = null;

        /// <summary>
        /// Click on the "Open All"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static TrainProject Load(string path)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    TrainProject project = (TrainProject)formatter.Deserialize(stream);
                    project.Path = path;
                    stream.Close();

                    // Make sure we clear any event serialized
                    foreach (Hub train in project.RegisteredTrains)
                    {
                        train.CleanAllEvents();
                    }

                    return project;
                }
            }
            catch(Exception ex)
            {
                MainBoard.WriteLine("ERROR - Could not open file: " + ex.Message, System.Drawing.Color.Red);
                return null;
            }

        }

		public Hub GetHubIndexByDeviceId(string deviceId)
		{
			for (int i = 0; i < RegisteredTrains.Count; i++)
				if (RegisteredTrains[i].DeviceId == deviceId)
					return RegisteredTrains[i];

			return null;
		}

        public bool Save()
        {
            if (Path != null)
            {
                SaveAs(Path);
                return true;
            }

            return false;
        }

        public void SaveAs(string path)
        {            
            try
            {
                IFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    formatter.Serialize(stream, this);
                    stream.Close();
                    Path = path;
                }
            }
            catch (Exception ex)
            {
                MainBoard.WriteLine("ERROR - Could not save file: " + ex.Message, System.Drawing.Color.Red);
            }
        }
    }
}
