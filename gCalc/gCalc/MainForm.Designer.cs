namespace gCalc
{
    partial class MainForm
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
            this.Canvas = new System.Windows.Forms.PictureBox();
            this.AddButton = new System.Windows.Forms.Button();
            this.FunctionsPanel = new System.Windows.Forms.Panel();
            this.CenterButton = new System.Windows.Forms.Button();
            this.ZoomInButton = new System.Windows.Forms.Button();
            this.ZoomOutButton = new System.Windows.Forms.Button();
            this.ToggleAllCheckbox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.Canvas)).BeginInit();
            this.FunctionsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // Canvas
            // 
            this.Canvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Canvas.BackColor = System.Drawing.Color.White;
            this.Canvas.Location = new System.Drawing.Point(145, 0);
            this.Canvas.Margin = new System.Windows.Forms.Padding(0);
            this.Canvas.Name = "Canvas";
            this.Canvas.Size = new System.Drawing.Size(706, 573);
            this.Canvas.TabIndex = 0;
            this.Canvas.TabStop = false;
            this.Canvas.SizeChanged += new System.EventHandler(this.Canvas_SizeChanged);
            this.Canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.Canvas_Paint);
            this.Canvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Canvas_MouseDown);
            this.Canvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Canvas_MouseMove);
            this.Canvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Canvas_MouseUp);
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(0, 25);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(109, 23);
            this.AddButton.TabIndex = 2;
            this.AddButton.Text = "+";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // FunctionsPanel
            // 
            this.FunctionsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.FunctionsPanel.AutoScroll = true;
            this.FunctionsPanel.Controls.Add(this.AddButton);
            this.FunctionsPanel.Location = new System.Drawing.Point(12, 34);
            this.FunctionsPanel.Name = "FunctionsPanel";
            this.FunctionsPanel.Size = new System.Drawing.Size(133, 539);
            this.FunctionsPanel.TabIndex = 4;
            // 
            // CenterButton
            // 
            this.CenterButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CenterButton.Location = new System.Drawing.Point(775, 506);
            this.CenterButton.Name = "CenterButton";
            this.CenterButton.Size = new System.Drawing.Size(48, 39);
            this.CenterButton.TabIndex = 5;
            this.CenterButton.Text = "Center";
            this.CenterButton.UseVisualStyleBackColor = true;
            this.CenterButton.Click += new System.EventHandler(this.CenterButton_Click);
            // 
            // ZoomInButton
            // 
            this.ZoomInButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ZoomInButton.Location = new System.Drawing.Point(775, 423);
            this.ZoomInButton.Name = "ZoomInButton";
            this.ZoomInButton.Size = new System.Drawing.Size(48, 39);
            this.ZoomInButton.TabIndex = 6;
            this.ZoomInButton.Text = "+";
            this.ZoomInButton.UseVisualStyleBackColor = true;
            this.ZoomInButton.Click += new System.EventHandler(this.ZoomInButton_Click);
            // 
            // ZoomOutButton
            // 
            this.ZoomOutButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ZoomOutButton.Location = new System.Drawing.Point(775, 461);
            this.ZoomOutButton.Name = "ZoomOutButton";
            this.ZoomOutButton.Size = new System.Drawing.Size(48, 39);
            this.ZoomOutButton.TabIndex = 7;
            this.ZoomOutButton.Text = "-";
            this.ZoomOutButton.UseVisualStyleBackColor = true;
            this.ZoomOutButton.Click += new System.EventHandler(this.ZoomOutButton_Click);
            // 
            // ToggleAllCheckbox
            // 
            this.ToggleAllCheckbox.AutoSize = true;
            this.ToggleAllCheckbox.Checked = true;
            this.ToggleAllCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ToggleAllCheckbox.Location = new System.Drawing.Point(12, 14);
            this.ToggleAllCheckbox.Name = "ToggleAllCheckbox";
            this.ToggleAllCheckbox.Size = new System.Drawing.Size(15, 14);
            this.ToggleAllCheckbox.TabIndex = 8;
            this.ToggleAllCheckbox.UseVisualStyleBackColor = true;
            this.ToggleAllCheckbox.Click += new System.EventHandler(this.ToggleAllCheckbox_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(851, 573);
            this.Controls.Add(this.ToggleAllCheckbox);
            this.Controls.Add(this.ZoomInButton);
            this.Controls.Add(this.ZoomOutButton);
            this.Controls.Add(this.CenterButton);
            this.Controls.Add(this.FunctionsPanel);
            this.Controls.Add(this.Canvas);
            this.MinimumSize = new System.Drawing.Size(243, 206);
            this.Name = "MainForm";
            this.Text = "gCalc";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Canvas)).EndInit();
            this.FunctionsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox Canvas;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Panel FunctionsPanel;
        private System.Windows.Forms.Button CenterButton;
        private System.Windows.Forms.Button ZoomInButton;
        private System.Windows.Forms.Button ZoomOutButton;
        private System.Windows.Forms.CheckBox ToggleAllCheckbox;
    }
}

