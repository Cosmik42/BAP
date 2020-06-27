namespace LegoTrainProject
{
	partial class TrainSectionControl
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
			this.labelName = new System.Windows.Forms.Label();
			this.comboBoxCurrentSection = new System.Windows.Forms.ComboBox();
			this.comboBoxPath = new System.Windows.Forms.ComboBox();
			this.checkBoxLoop = new System.Windows.Forms.CheckBox();
			this.buttonStop = new System.Windows.Forms.Button();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this.buttonConfigure = new System.Windows.Forms.Button();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.startButton = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// labelName
			// 
			this.labelName.AutoSize = true;
			this.labelName.ForeColor = System.Drawing.Color.Green;
			this.labelName.Location = new System.Drawing.Point(23, 7);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(80, 19);
			this.labelName.TabIndex = 0;
			this.labelName.Text = "Yellow Train";
			// 
			// comboBoxCurrentSection
			// 
			this.comboBoxCurrentSection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxCurrentSection.FormattingEnabled = true;
			this.comboBoxCurrentSection.Location = new System.Drawing.Point(129, 7);
			this.comboBoxCurrentSection.Name = "comboBoxCurrentSection";
			this.comboBoxCurrentSection.Size = new System.Drawing.Size(108, 25);
			this.comboBoxCurrentSection.TabIndex = 1;
			this.comboBoxCurrentSection.SelectedIndexChanged += new System.EventHandler(this.ComboBoxCurrentSection_SelectedIndexChanged);
			// 
			// comboBoxPath
			// 
			this.comboBoxPath.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxPath.FormattingEnabled = true;
			this.comboBoxPath.Location = new System.Drawing.Point(31, 36);
			this.comboBoxPath.Name = "comboBoxPath";
			this.comboBoxPath.Size = new System.Drawing.Size(206, 25);
			this.comboBoxPath.TabIndex = 9;
			this.comboBoxPath.SelectedIndexChanged += new System.EventHandler(this.comboBoxPath_SelectedIndexChanged);
			// 
			// checkBoxLoop
			// 
			this.checkBoxLoop.AutoSize = true;
			this.checkBoxLoop.Location = new System.Drawing.Point(168, 70);
			this.checkBoxLoop.Name = "checkBoxLoop";
			this.checkBoxLoop.Size = new System.Drawing.Size(62, 23);
			this.checkBoxLoop.TabIndex = 14;
			this.checkBoxLoop.Text = "Loop";
			this.checkBoxLoop.UseVisualStyleBackColor = true;
			this.checkBoxLoop.CheckedChanged += new System.EventHandler(this.checkBoxLoop_CheckedChanged);
			// 
			// buttonStop
			// 
			this.buttonStop.Image = global::LegoTrainProject.Properties.Resources.icons8_private_48;
			this.buttonStop.Location = new System.Drawing.Point(63, 67);
			this.buttonStop.Name = "buttonStop";
			this.buttonStop.Size = new System.Drawing.Size(45, 30);
			this.buttonStop.TabIndex = 13;
			this.buttonStop.UseVisualStyleBackColor = true;
			this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
			// 
			// pictureBox3
			// 
			this.pictureBox3.Image = global::LegoTrainProject.Properties.Resources.icons8_navigator_48;
			this.pictureBox3.Location = new System.Drawing.Point(3, 36);
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.Size = new System.Drawing.Size(22, 22);
			this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox3.TabIndex = 12;
			this.pictureBox3.TabStop = false;
			// 
			// buttonConfigure
			// 
			this.buttonConfigure.Image = global::LegoTrainProject.Properties.Resources.icons8_services_26;
			this.buttonConfigure.Location = new System.Drawing.Point(6, 67);
			this.buttonConfigure.Name = "buttonConfigure";
			this.buttonConfigure.Size = new System.Drawing.Size(42, 30);
			this.buttonConfigure.TabIndex = 11;
			this.buttonConfigure.UseVisualStyleBackColor = true;
			this.buttonConfigure.Click += new System.EventHandler(this.buttonConfigure_Click);
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = global::LegoTrainProject.Properties.Resources.icons8_place_marker_48__1_;
			this.pictureBox2.Location = new System.Drawing.Point(97, 4);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(26, 26);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox2.TabIndex = 10;
			this.pictureBox2.TabStop = false;
			// 
			// startButton
			// 
			this.startButton.Image = global::LegoTrainProject.Properties.Resources.icons8_next_48;
			this.startButton.Location = new System.Drawing.Point(114, 67);
			this.startButton.Name = "startButton";
			this.startButton.Size = new System.Drawing.Size(45, 30);
			this.startButton.TabIndex = 8;
			this.startButton.UseVisualStyleBackColor = true;
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::LegoTrainProject.Properties.Resources.icons8_train_48;
			this.pictureBox1.Location = new System.Drawing.Point(-1, 4);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(25, 25);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 7;
			this.pictureBox1.TabStop = false;
			// 
			// TrainSectionControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.checkBoxLoop);
			this.Controls.Add(this.buttonStop);
			this.Controls.Add(this.pictureBox3);
			this.Controls.Add(this.buttonConfigure);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.comboBoxPath);
			this.Controls.Add(this.startButton);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.comboBoxCurrentSection);
			this.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "TrainSectionControl";
			this.Size = new System.Drawing.Size(242, 107);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
		public System.Windows.Forms.ComboBox comboBoxCurrentSection;
		public System.Windows.Forms.Button startButton;
		public System.Windows.Forms.ComboBox comboBoxPath;
		public System.Windows.Forms.Button buttonConfigure;
		private System.Windows.Forms.PictureBox pictureBox3;
		public System.Windows.Forms.Button buttonStop;
		private System.Windows.Forms.CheckBox checkBoxLoop;
	}
}
