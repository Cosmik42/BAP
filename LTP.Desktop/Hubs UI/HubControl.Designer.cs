namespace LegoTrainProject
{
	partial class HubControl
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
			this.richTextBoxLabelHub = new System.Windows.Forms.RichTextBox();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.ButtonConfigure = new System.Windows.Forms.Button();
			this.pictureBoxStatus = new System.Windows.Forms.PictureBox();
			this.buttonDisconnect = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).BeginInit();
			this.SuspendLayout();
			// 
			// richTextBoxLabelHub
			// 
			this.richTextBoxLabelHub.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.richTextBoxLabelHub.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.richTextBoxLabelHub.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBoxLabelHub.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richTextBoxLabelHub.Location = new System.Drawing.Point(37, 8);
			this.richTextBoxLabelHub.Name = "richTextBoxLabelHub";
			this.richTextBoxLabelHub.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.richTextBoxLabelHub.Size = new System.Drawing.Size(108, 19);
			this.richTextBoxLabelHub.TabIndex = 0;
			this.richTextBoxLabelHub.Text = "Red Train - 87%\n";
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(4, 71);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(139, 233);
			this.flowLayoutPanel1.TabIndex = 1;
			// 
			// ButtonConfigure
			// 
			this.ButtonConfigure.Location = new System.Drawing.Point(7, 34);
			this.ButtonConfigure.Name = "ButtonConfigure";
			this.ButtonConfigure.Size = new System.Drawing.Size(98, 27);
			this.ButtonConfigure.TabIndex = 2;
			this.ButtonConfigure.Text = "Configure";
			this.ButtonConfigure.UseVisualStyleBackColor = true;
			this.ButtonConfigure.Click += new System.EventHandler(this.ButtonConfigure_Click);
			// 
			// pictureBoxStatus
			// 
			this.pictureBoxStatus.Image = global::LegoTrainProject.Properties.Resources.disconnected;
			this.pictureBoxStatus.Location = new System.Drawing.Point(5, 3);
			this.pictureBoxStatus.Name = "pictureBoxStatus";
			this.pictureBoxStatus.Size = new System.Drawing.Size(26, 26);
			this.pictureBoxStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxStatus.TabIndex = 3;
			this.pictureBoxStatus.TabStop = false;
			// 
			// buttonDisconnect
			// 
			this.buttonDisconnect.Image = global::LegoTrainProject.Properties.Resources.icons8_close_window_48;
			this.buttonDisconnect.Location = new System.Drawing.Point(109, 34);
			this.buttonDisconnect.Name = "buttonDisconnect";
			this.buttonDisconnect.Size = new System.Drawing.Size(32, 27);
			this.buttonDisconnect.TabIndex = 4;
			this.buttonDisconnect.UseVisualStyleBackColor = true;
			this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
			// 
			// HubControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.buttonDisconnect);
			this.Controls.Add(this.pictureBoxStatus);
			this.Controls.Add(this.ButtonConfigure);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Controls.Add(this.richTextBoxLabelHub);
			this.Name = "HubControl";
			this.Size = new System.Drawing.Size(146, 310);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox richTextBoxLabelHub;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button ButtonConfigure;
		private System.Windows.Forms.PictureBox pictureBoxStatus;
		private System.Windows.Forms.Button buttonDisconnect;
	}
}
