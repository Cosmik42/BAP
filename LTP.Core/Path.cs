using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoTrainProject
{
	[Serializable]
	public class Path
	{
		public int[] Sections = new int[1];
		public string Name;
		public bool loopPath = false;

		public Path()
		{
			Name = "New Path";
			Sections[0] = 0;
		}

		public override string ToString()
		{
			return Name + $" ({Sections.Length} sections)";
		}

			public string SectionsToString()
		{
			string output = "";

			if (Sections != null)
				for (int i = 0; i < Sections.Length; i++)
				{
					output += Sections[i];

					if (i + 1 < Sections.Length)
						output += ", ";
				}

			return output;
		}

		public bool FromString(string path)
		{
			string[] sections = path.Split(new char[] { ',' });

			if (sections != null && sections.Length > 0)
			{
				int[] finalProgram = new int[sections.Length];

				for (int i = 0; i < sections.Length; i++)
				{
					int currentSection;
					if (Int32.TryParse(sections[i], out currentSection))
						finalProgram[i] = currentSection;
					else
					{
						return false;
					}
				}

				Sections = finalProgram;
				return true;
			}

			return false;
		}
	}
}
