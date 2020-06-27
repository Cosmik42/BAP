namespace LegoTrainProject.Sections_UI
{
	partial class PathEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PathEditor));
			this.listBoxPaths = new System.Windows.Forms.ListBox();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonAddPaths = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDeletePath = new System.Windows.Forms.ToolStripButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.buttonSave = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxPath = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.toolStrip1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// listBoxPaths
			// 
			this.listBoxPaths.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listBoxPaths.FormattingEnabled = true;
			this.listBoxPaths.ItemHeight = 17;
			this.listBoxPaths.Location = new System.Drawing.Point(13, 39);
			this.listBoxPaths.Name = "listBoxPaths";
			this.listBoxPaths.Size = new System.Drawing.Size(481, 157);
			this.listBoxPaths.TabIndex = 0;
			this.listBoxPaths.SelectedIndexChanged += new System.EventHandler(this.listBoxPaths_SelectedIndexChanged);
			// 
			// toolStrip1
			// 
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonAddPaths,
            this.toolStripButtonDeletePath});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(506, 27);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButtonAddPaths
			// 
			this.toolStripButtonAddPaths.Image = global::LegoTrainProject.Properties.Resources.icons8_navigator_plus_48;
			this.toolStripButtonAddPaths.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonAddPaths.Name = "toolStripButtonAddPaths";
			this.toolStripButtonAddPaths.Size = new System.Drawing.Size(93, 24);
			this.toolStripButtonAddPaths.Text = "Add Path";
			this.toolStripButtonAddPaths.Click += new System.EventHandler(this.toolStripButtonAddPaths_Click);
			// 
			// toolStripButtonDeletePath
			// 
			this.toolStripButtonDeletePath.Image = global::LegoTrainProject.Properties.Resources.icons8_navigator_del_;
			this.toolStripButtonDeletePath.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonDeletePath.Name = "toolStripButtonDeletePath";
			this.toolStripButtonDeletePath.Size = new System.Drawing.Size(170, 24);
			this.toolStripButtonDeletePath.Text = "Delete Selected Path";
			this.toolStripButtonDeletePath.Click += new System.EventHandler(this.toolStripButtonDeletePath_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.buttonSave);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.textBoxPath);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.textBoxName);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox1.Location = new System.Drawing.Point(13, 202);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(481, 268);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Selected Path";
			// 
			// buttonSave
			// 
			this.buttonSave.Enabled = false;
			this.buttonSave.Location = new System.Drawing.Point(190, 226);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(107, 33);
			this.buttonSave.TabIndex = 3;
			this.buttonSave.Text = "Save";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(59, 84);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(413, 133);
			this.label3.TabIndex = 4;
			this.label3.Text = resources.GetString("label3.Text");
			// 
			// textBoxPath
			// 
			this.textBoxPath.Enabled = false;
			this.textBoxPath.Location = new System.Drawing.Point(59, 57);
			this.textBoxPath.Name = "textBoxPath";
			this.textBoxPath.Size = new System.Drawing.Size(416, 25);
			this.textBoxPath.TabIndex = 3;
			this.textBoxPath.TextChanged += new System.EventHandler(this.textBoxPath_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(7, 57);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(37, 19);
			this.label2.TabIndex = 2;
			this.label2.Text = "Path";
			// 
			// textBoxName
			// 
			this.textBoxName.Enabled = false;
			this.textBoxName.Location = new System.Drawing.Point(59, 24);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(416, 25);
			this.textBoxName.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(7, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(45, 19);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name";
			// 
			// PathEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(506, 482);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.listBoxPaths);
			this.Name = "PathEditor";
			this.Text = "Path Editor";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox listBoxPaths;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButtonAddPaths;
		private System.Windows.Forms.ToolStripButton toolStripButtonDeletePath;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxPath;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonSave;
	}
}