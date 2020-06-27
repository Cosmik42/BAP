namespace LegoTrainProject.Sections_UI
{
	partial class TrainOptions
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
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxSpeed = new System.Windows.Forms.TextBox();
			this.textBoxClearing = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxCoefficient = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 45);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(238, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Speed when danger ahead (30-100)";
			// 
			// textBoxSpeed
			// 
			this.textBoxSpeed.Location = new System.Drawing.Point(264, 42);
			this.textBoxSpeed.Name = "textBoxSpeed";
			this.textBoxSpeed.Size = new System.Drawing.Size(100, 22);
			this.textBoxSpeed.TabIndex = 1;
			this.textBoxSpeed.TextChanged += new System.EventHandler(this.textBoxSpeed_TextChanged);
			// 
			// textBoxClearing
			// 
			this.textBoxClearing.Location = new System.Drawing.Point(264, 8);
			this.textBoxClearing.Name = "textBoxClearing";
			this.textBoxClearing.Size = new System.Drawing.Size(100, 22);
			this.textBoxClearing.TabIndex = 4;
			this.textBoxClearing.TextChanged += new System.EventHandler(this.textBoxClearing_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 11);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(238, 17);
			this.label2.TabIndex = 3;
			this.label2.Text = "Time needed to leave a section (ms)";
			// 
			// textBoxCoefficient
			// 
			this.textBoxCoefficient.Location = new System.Drawing.Point(264, 76);
			this.textBoxCoefficient.Name = "textBoxCoefficient";
			this.textBoxCoefficient.Size = new System.Drawing.Size(100, 22);
			this.textBoxCoefficient.TabIndex = 7;
			this.textBoxCoefficient.TextChanged += new System.EventHandler(this.textBoxCoefficient_TextChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 79);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(176, 17);
			this.label3.TabIndex = 6;
			this.label3.Text = "Section\'s Speed Coeficient";
			// 
			// TrainOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(376, 114);
			this.Controls.Add(this.textBoxCoefficient);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBoxClearing);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxSpeed);
			this.Controls.Add(this.label1);
			this.Name = "TrainOptions";
			this.Text = "Train Options";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxSpeed;
		private System.Windows.Forms.TextBox textBoxClearing;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxCoefficient;
		private System.Windows.Forms.Label label3;
	}
}