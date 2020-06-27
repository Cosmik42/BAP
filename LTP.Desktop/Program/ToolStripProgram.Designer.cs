namespace LegoTrainProject
{
	partial class ToolStripProgram
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
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonStart = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonAddSensor = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonAddSequence = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(30, 30);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonStart,
            this.toolStripButtonAddSensor,
            this.toolStripButtonAddSequence,
            this.toolStripButtonDelete});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(694, 37);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip";
			this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
			// 
			// toolStripButtonStart
			// 
			this.toolStripButtonStart.Image = global::LegoTrainProject.Properties.Resources.icons8_start_48;
			this.toolStripButtonStart.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonStart.Name = "toolStripButtonStart";
			this.toolStripButtonStart.Size = new System.Drawing.Size(137, 34);
			this.toolStripButtonStart.Text = "Start Listening";
			// 
			// toolStripButtonAddSensor
			// 
			this.toolStripButtonAddSensor.Image = global::LegoTrainProject.Properties.Resources.icons8_motion_sensor_48;
			this.toolStripButtonAddSensor.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonAddSensor.Name = "toolStripButtonAddSensor";
			this.toolStripButtonAddSensor.Size = new System.Drawing.Size(159, 34);
			this.toolStripButtonAddSensor.Text = "Add Sensor Event";
			// 
			// toolStripButtonAddSequence
			// 
			this.toolStripButtonAddSequence.Image = global::LegoTrainProject.Properties.Resources.icons8_code_48;
			this.toolStripButtonAddSequence.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonAddSequence.Name = "toolStripButtonAddSequence";
			this.toolStripButtonAddSequence.Size = new System.Drawing.Size(139, 34);
			this.toolStripButtonAddSequence.Text = "Add Sequence";
			// 
			// toolStripButtonDelete
			// 
			this.toolStripButtonDelete.Image = global::LegoTrainProject.Properties.Resources.icons8_trash_can_48;
			this.toolStripButtonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonDelete.Name = "toolStripButtonDelete";
			this.toolStripButtonDelete.Size = new System.Drawing.Size(148, 34);
			this.toolStripButtonDelete.Text = "Delete Program";
			// 
			// ToolStripProgram
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.toolStrip1);
			this.Name = "ToolStripProgram";
			this.Size = new System.Drawing.Size(694, 38);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		public System.Windows.Forms.ToolStripButton toolStripButtonStart;
		public System.Windows.Forms.ToolStripButton toolStripButtonAddSensor;
		public System.Windows.Forms.ToolStripButton toolStripButtonAddSequence;
		public System.Windows.Forms.ToolStripButton toolStripButtonDelete;
	}
}
