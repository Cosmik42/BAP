namespace LegoTrainProject.Sections_UI
{
	partial class TrainSectionBehavior
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.checkBox2Ahead = new System.Windows.Forms.CheckBox();
			this.textBoxMaxSpeed = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.buttonEditCode = new System.Windows.Forms.Button();
			this.radioButtonExecute = new System.Windows.Forms.RadioButton();
			this.radioButtonResume = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkBox2Ahead
			// 
			this.checkBox2Ahead.AutoSize = true;
			this.checkBox2Ahead.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox2Ahead.Location = new System.Drawing.Point(6, 33);
			this.checkBox2Ahead.Name = "checkBox2Ahead";
			this.checkBox2Ahead.Size = new System.Drawing.Size(206, 23);
			this.checkBox2Ahead.TabIndex = 13;
			this.checkBox2Ahead.Text = "Need next 2 Sections cleared";
			this.checkBox2Ahead.UseVisualStyleBackColor = true;
			this.checkBox2Ahead.CheckedChanged += new System.EventHandler(this.checkBox2Ahead_CheckedChanged);
			// 
			// textBoxMaxSpeed
			// 
			this.textBoxMaxSpeed.Location = new System.Drawing.Point(29, 49);
			this.textBoxMaxSpeed.Margin = new System.Windows.Forms.Padding(2);
			this.textBoxMaxSpeed.Name = "textBoxMaxSpeed";
			this.textBoxMaxSpeed.Size = new System.Drawing.Size(126, 22);
			this.textBoxMaxSpeed.TabIndex = 12;
			this.textBoxMaxSpeed.TextChanged += new System.EventHandler(this.textBoxMaxSpeed_TextChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkBox2Ahead);
			this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox1.Location = new System.Drawing.Point(17, 176);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(243, 74);
			this.groupBox1.TabIndex = 14;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "When Leaving Section";
			// 
			// buttonEditCode
			// 
			this.buttonEditCode.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonEditCode.Location = new System.Drawing.Point(29, 106);
			this.buttonEditCode.Name = "buttonEditCode";
			this.buttonEditCode.Size = new System.Drawing.Size(133, 32);
			this.buttonEditCode.TabIndex = 16;
			this.buttonEditCode.Text = "Edit Code";
			this.buttonEditCode.UseVisualStyleBackColor = true;
			this.buttonEditCode.Click += new System.EventHandler(this.buttonEditCode_Click);
			// 
			// radioButtonExecute
			// 
			this.radioButtonExecute.AutoSize = true;
			this.radioButtonExecute.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.radioButtonExecute.Location = new System.Drawing.Point(6, 79);
			this.radioButtonExecute.Name = "radioButtonExecute";
			this.radioButtonExecute.Size = new System.Drawing.Size(164, 23);
			this.radioButtonExecute.TabIndex = 15;
			this.radioButtonExecute.TabStop = true;
			this.radioButtonExecute.Text = "Execute Custom Code";
			this.radioButtonExecute.UseVisualStyleBackColor = true;
			this.radioButtonExecute.CheckedChanged += new System.EventHandler(this.radioButtonExecute_CheckedChanged);
			// 
			// radioButtonResume
			// 
			this.radioButtonResume.AutoSize = true;
			this.radioButtonResume.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.radioButtonResume.Location = new System.Drawing.Point(6, 21);
			this.radioButtonResume.Name = "radioButtonResume";
			this.radioButtonResume.Size = new System.Drawing.Size(172, 23);
			this.radioButtonResume.TabIndex = 14;
			this.radioButtonResume.TabStop = true;
			this.radioButtonResume.Text = "Resume At Fixed Speed";
			this.radioButtonResume.UseVisualStyleBackColor = true;
			this.radioButtonResume.CheckedChanged += new System.EventHandler(this.radioButtonResume_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.buttonEditCode);
			this.groupBox2.Controls.Add(this.radioButtonExecute);
			this.groupBox2.Controls.Add(this.radioButtonResume);
			this.groupBox2.Controls.Add(this.textBoxMaxSpeed);
			this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox2.Location = new System.Drawing.Point(17, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(243, 158);
			this.groupBox2.TabIndex = 15;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "When Entering Section";
			// 
			// TrainSectionBehavior
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(272, 264);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "TrainSectionBehavior";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Train Behavior";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBox2Ahead;
		private System.Windows.Forms.TextBox textBoxMaxSpeed;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonExecute;
		private System.Windows.Forms.RadioButton radioButtonResume;
		private System.Windows.Forms.Button buttonEditCode;
	}
}