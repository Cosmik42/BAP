using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using LegoTrainProject;
using Microsoft.CSharp;

namespace LegoTrainProject
{
    public partial class FormCodeEditor : Form
    {
		AutocompleteMenu popupMenu;
		TrainProgramEvent CodeEvent;
		TrainProgramEvent GlobalCode;
		private List<Hub> hubs;
		Sections sections;

		public FormCodeEditor(TrainProgramEvent codeEvent, TrainProject project, bool editGlobalCode)
        {
            // Init code to edit and global code
            CodeEvent = codeEvent;
			GlobalCode = (editGlobalCode) ? null : project.GlobalCode;

			// Attach Hubs without launching sensors.
			this.hubs = project.RegisteredTrains;
			this.sections = project.Sections;
			foreach (Hub h in hubs)
				h.State = h.State ?? (new int[100]);

			// Init Components
			InitializeComponent();

			if (codeEvent.Name != null)
				textBoxName.Text = codeEvent.Name;

			fastColoredTextBox1.Text = codeEvent.CodeToRun;
			fastColoredTextBox1.TextChanged += fastColoredTextBox1_TextChanged;

			fastColoredTextBox1.SelectAll();
			fastColoredTextBox1.Selection.SetStyle(GreenStyle, @"//.*$", RegexOptions.Multiline);
			fastColoredTextBox1.Selection.SetStyle(BrownStyle, @"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'");
			fastColoredTextBox1.Selection.SetStyle(MagentaStyle, @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b");

			fastColoredTextBox1.Selection = new Range(fastColoredTextBox1, 0);

			// if Globa Code is null, it means we are editing the global code itself
			buttonTestCode.Text = (GlobalCode == null) ? "Compile Code" : "Run Code";

			InitLabel();
			InitAutocomplete();

		}

		public void InitAutocomplete()
		{
			//create autocomplete popup menu
			popupMenu = new AutocompleteMenu(fastColoredTextBox1);

			//generate 456976 words
			var keyword = new List<string>()
			{
				".SetMotorSpeed(\"A\", 100);", ".GetMotorSpeed(\"A\");", ".RampMotorSpeed(\"A\", 100, 1000);", ".Stop(\"A\");", ".Wait(1000);", ".ActivateSwitchOnLeft(\"A\");", ".ActivateSwitchOnRight(\"A\");", ".State[]", ".WriteLine(\"\")"
			};

			popupMenu.ForeColor = Color.White;
			popupMenu.BackColor = Color.Gray;
			popupMenu.SelectedColor = Color.Purple;
			popupMenu.SearchPattern = @"\w\.";
			popupMenu.AllowTabKey = true;
			popupMenu.AlwaysShowTooltip = true;
			//set words as autocomplete source
			popupMenu.Items.SetAutocompleteItems(keyword);

		}

		private void InitLabel()
        {
            string labelText = "";
            if (hubs != null)
            {
                for (int i = 0; i < hubs.Count; i++)
                    labelText += $"Hub[{i}] => '" + hubs[i].Name + "' - ";
            }

            labelHelp.Text = labelText;
        }

        private void WriteLine(string text, Color color)
        {
            MainBoard.AppendTextWithColor(richTextBoxConsole, text + Environment.NewLine, color);
            richTextBoxConsole.SelectionStart = richTextBoxConsole.Text.Length;
            // scroll it automatically
            richTextBoxConsole.ScrollToCaret();
        }

        private void buttonTestCode_Click(object sender, EventArgs ev)
        {
            WriteLine($"Compiling code ...", Color.Black);
			string code = "";

			// We are running code in a normal way
			if (GlobalCode != null)
			{
				code = TrainProgramEvent.Code.Replace("%Code%", CodeEvent.CodeToRun.Replace("Wait(", "await Task.Delay("));
				code = code.Replace("/*%GlobalCode%*/", GlobalCode.CodeToRun.Replace("Wait(", "await Task.Delay("));
			}
			// We are editing the global code
			else
			{
				code = TrainProgramEvent.Code.Replace("%Code%", "");
				code = code.Replace("/*%GlobalCode%*/", CodeEvent.CodeToRun.Replace("Wait(", "await Task.Delay("));
			}


			//var options = new Dictionary<string, string> { { "CompilerVersion", "v5.0" } };
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
                WriteLine($"Compiling failed.", Color.Red);
                foreach (CompilerError error in results.Errors)
                {
                    WriteLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText), Color.Red);
                }
            }
            else
            {
                WriteLine($"Compiling successful!", Color.Green);

				// We don't run the code if we are editing the global code itself.
				if (GlobalCode != null)
				{ 
					Assembly assembly = results.CompiledAssembly;
					Type program = assembly.GetType("LegoTrainProject.DynamicCode");
					MethodInfo main = program.GetMethod("ExecuteCode");

					WriteLine($"Executing code ...", Color.Black);
					try
					{
						main.Invoke(null, new object[] { CodeEvent, hubs, sections });
					}
					catch (Exception ex)
					{
						WriteLine("Exception while executing your sequence: " + ex.Message, Color.Red);
						return;
					}

					WriteLine($"Executing completed!", Color.Green);
				}
            }


        }

		TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
		TextStyle GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
		TextStyle BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Italic);

		private void fastColoredTextBox1_TextChanged(object sender, TextChangedEventArgs e)
		{
			//clear style of changed range
			e.ChangedRange.ClearStyle(GreenStyle);
			//comment highlighting
			e.ChangedRange.SetStyle(GreenStyle, @"//.*$", RegexOptions.Multiline);
			e.ChangedRange.SetStyle(BrownStyle, @"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'");
			e.ChangedRange.SetStyle(MagentaStyle, @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b");

			CodeEvent.CodeToRun = fastColoredTextBox1.Text;
		}

		private void fastColoredTextBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control)
			{
				if (e.KeyCode == Keys.Space)
				{
					Timer t = new Timer();
					popupMenu.Show(true);
					e.Handled = true;
				}
			}
		}

		private void textBoxName_TextChanged(object sender, EventArgs e)
		{
			if (textBoxName.Text == "")
				CodeEvent.Name = null;
			else
				CodeEvent.Name = textBoxName.Text;
		}
	}
}
