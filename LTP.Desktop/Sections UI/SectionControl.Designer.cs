namespace LegoTrainProject
{
	partial class SectionControl
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
			this.labelSectionName = new System.Windows.Forms.Label();
			this.comboBoxDetector = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.pictureBoxDelete = new System.Windows.Forms.PictureBox();
			this.buttonConfigure = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.switchSetup1 = new LegoTrainProject.SwitchSetup();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxDelete)).BeginInit();
			this.SuspendLayout();
			// 
			// labelSectionName
			// 
			this.labelSectionName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelSectionName.ForeColor = System.Drawing.Color.Maroon;
			this.labelSectionName.Location = new System.Drawing.Point(2, 3);
			this.labelSectionName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelSectionName.Name = "labelSectionName";
			this.labelSectionName.Size = new System.Drawing.Size(136, 20);
			this.labelSectionName.TabIndex = 0;
			this.labelSectionName.Text = "Section 0";
			this.labelSectionName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// comboBoxDetector
			// 
			this.comboBoxDetector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxDetector.FormattingEnabled = true;
			this.comboBoxDetector.Location = new System.Drawing.Point(8, 162);
			this.comboBoxDetector.Margin = new System.Windows.Forms.Padding(2);
			this.comboBoxDetector.Name = "comboBoxDetector";
			this.comboBoxDetector.Size = new System.Drawing.Size(126, 27);
			this.comboBoxDetector.TabIndex = 1;
			this.comboBoxDetector.SelectedIndexChanged += new System.EventHandler(this.comboBoxDetector_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
			this.label1.Location = new System.Drawing.Point(6, 144);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(148, 19);
			this.label1.TabIndex = 2;
			this.label1.Text = "End Section Detector";
			// 
			// pictureBox2
			// 
			this.pictureBox2.Image = global::LegoTrainProject.Properties.Resources.icons8_tracks_48;
			this.pictureBox2.Location = new System.Drawing.Point(16, 3);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(21, 21);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox2.TabIndex = 9;
			this.pictureBox2.TabStop = false;
			// 
			// pictureBoxDelete
			// 
			this.pictureBoxDelete.Image = global::LegoTrainProject.Properties.Resources.icons8_trash_can_48;
			this.pictureBoxDelete.Location = new System.Drawing.Point(113, 3);
			this.pictureBoxDelete.Name = "pictureBoxDelete";
			this.pictureBoxDelete.Size = new System.Drawing.Size(21, 21);
			this.pictureBoxDelete.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxDelete.TabIndex = 8;
			this.pictureBoxDelete.TabStop = false;
			this.pictureBoxDelete.Visible = false;
			this.pictureBoxDelete.Click += new System.EventHandler(this.pictureBoxDelete_Click);
			// 
			// buttonConfigure
			// 
			this.buttonConfigure.Image = global::LegoTrainProject.Properties.Resources.icons8_services_26;
			this.buttonConfigure.Location = new System.Drawing.Point(9, 212);
			this.buttonConfigure.Name = "buttonConfigure";
			this.buttonConfigure.Size = new System.Drawing.Size(125, 38);
			this.buttonConfigure.TabIndex = 12;
			this.buttonConfigure.Text = "Configure";
			this.buttonConfigure.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.buttonConfigure.UseVisualStyleBackColor = true;
			this.buttonConfigure.Click += new System.EventHandler(this.buttonConfigure_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
			this.label2.Location = new System.Drawing.Point(6, 191);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(106, 19);
			this.label2.TabIndex = 13;
			this.label2.Text = "Train Behavior";
			// 
			// switchSetup1
			// 
			this.switchSetup1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.switchSetup1.Location = new System.Drawing.Point(3, 24);
			this.switchSetup1.Margin = new System.Windows.Forms.Padding(2);
			this.switchSetup1.Name = "switchSetup1";
			this.switchSetup1.Size = new System.Drawing.Size(137, 117);
			this.switchSetup1.TabIndex = 3;
			// 
			// SectionControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.buttonConfigure);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.pictureBoxDelete);
			this.Controls.Add(this.switchSetup1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBoxDetector);
			this.Controls.Add(this.labelSectionName);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "SectionControl";
			this.Size = new System.Drawing.Size(140, 264);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxDelete)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelSectionName;
		private System.Windows.Forms.ComboBox comboBoxDetector;
		private System.Windows.Forms.Label label1;
		private SwitchSetup switchSetup1;
		private System.Windows.Forms.PictureBox pictureBoxDelete;
		private System.Windows.Forms.PictureBox pictureBox2;
		public System.Windows.Forms.Button buttonConfigure;
		private System.Windows.Forms.Label label2;
	}
}
