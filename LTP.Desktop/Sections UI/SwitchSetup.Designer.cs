namespace LegoTrainProject
{
	partial class SwitchSetup
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.comboBoxSwitch = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxLeft = new System.Windows.Forms.ComboBox();
			this.comboBoxRight = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// comboBoxSwitch
			// 
			this.comboBoxSwitch.FormattingEnabled = true;
			this.comboBoxSwitch.Location = new System.Drawing.Point(3, 24);
			this.comboBoxSwitch.Margin = new System.Windows.Forms.Padding(2);
			this.comboBoxSwitch.Name = "comboBoxSwitch";
			this.comboBoxSwitch.Size = new System.Drawing.Size(126, 21);
			this.comboBoxSwitch.TabIndex = 0;
			this.comboBoxSwitch.SelectedIndexChanged += new System.EventHandler(this.comboBoxSwitch_SelectedIndexChanged_1);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
			this.label1.Location = new System.Drawing.Point(-2, 69);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(45, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "On Left";
			// 
			// comboBoxLeft
			// 
			this.comboBoxLeft.FormattingEnabled = true;
			this.comboBoxLeft.Location = new System.Drawing.Point(49, 65);
			this.comboBoxLeft.Margin = new System.Windows.Forms.Padding(2);
			this.comboBoxLeft.Name = "comboBoxLeft";
			this.comboBoxLeft.Size = new System.Drawing.Size(80, 21);
			this.comboBoxLeft.TabIndex = 2;
			this.comboBoxLeft.SelectedIndexChanged += new System.EventHandler(this.comboBoxLeft_SelectedIndexChanged);
			// 
			// comboBoxRight
			// 
			this.comboBoxRight.FormattingEnabled = true;
			this.comboBoxRight.Location = new System.Drawing.Point(49, 89);
			this.comboBoxRight.Margin = new System.Windows.Forms.Padding(2);
			this.comboBoxRight.Name = "comboBoxRight";
			this.comboBoxRight.Size = new System.Drawing.Size(80, 21);
			this.comboBoxRight.TabIndex = 4;
			this.comboBoxRight.SelectedIndexChanged += new System.EventHandler(this.comboBoxRight_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F);
			this.label2.Location = new System.Drawing.Point(-3, 91);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "On Right ";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(1, 6);
			this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(137, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Switch at end of Section?";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
			this.label4.Location = new System.Drawing.Point(1, 49);
			this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(86, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Next Section(s)";
			// 
			// SwitchSetup
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboBoxRight);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBoxLeft);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBoxSwitch);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "SwitchSetup";
			this.Size = new System.Drawing.Size(134, 117);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBoxSwitch;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxLeft;
		private System.Windows.Forms.ComboBox comboBoxRight;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
	}
}
