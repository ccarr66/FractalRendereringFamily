namespace Mandlebrot
{
    partial class MandlebrotDisp
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
            this.btn_DragRegion = new System.Windows.Forms.Button();
            this.txtbx_Center = new System.Windows.Forms.TextBox();
            this.lbl_CenterXY = new System.Windows.Forms.Label();
            this.lbl_Scale = new System.Windows.Forms.Label();
            this.txtbx_Scale = new System.Windows.Forms.TextBox();
            this.btn_Update = new System.Windows.Forms.Button();
            this.btn_ColorSettings = new System.Windows.Forms.Button();
            this.btn_ResetImage = new System.Windows.Forms.Button();
            this.pctbx_PaletteDisplay = new System.Windows.Forms.PictureBox();
            this.btn_Maximize = new System.Windows.Forms.Button();
            this.btn_Close = new System.Windows.Forms.Button();
            this.btn_ZoomIn = new System.Windows.Forms.Button();
            this.btn_ZoomOut = new System.Windows.Forms.Button();
            this.pctBx_Display = new System.Windows.Forms.PictureBox();
            this.pnl_ColorEditor = new System.Windows.Forms.Panel();
            this.lbl_Palette = new System.Windows.Forms.Label();
            this.lbl_IterationDepth = new System.Windows.Forms.Label();
            this.txtbx_IterationDepth = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pctbx_PaletteDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctBx_Display)).BeginInit();
            this.pnl_ColorEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_DragRegion
            // 
            this.btn_DragRegion.FlatAppearance.BorderSize = 0;
            this.btn_DragRegion.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btn_DragRegion.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.btn_DragRegion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_DragRegion.Location = new System.Drawing.Point(0, 0);
            this.btn_DragRegion.Name = "btn_DragRegion";
            this.btn_DragRegion.Size = new System.Drawing.Size(783, 20);
            this.btn_DragRegion.TabIndex = 0;
            this.btn_DragRegion.TabStop = false;
            this.btn_DragRegion.UseVisualStyleBackColor = true;
            this.btn_DragRegion.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_DragRegion_MouseDown);
            this.btn_DragRegion.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btn_DragRegion_MouseMove);
            this.btn_DragRegion.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_DragRegion_MouseUp);
            // 
            // txtbx_Center
            // 
            this.txtbx_Center.BackColor = System.Drawing.Color.Black;
            this.txtbx_Center.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtbx_Center.ForeColor = System.Drawing.Color.Gray;
            this.txtbx_Center.Location = new System.Drawing.Point(79, 632);
            this.txtbx_Center.Name = "txtbx_Center";
            this.txtbx_Center.Size = new System.Drawing.Size(120, 20);
            this.txtbx_Center.TabIndex = 0;
            this.txtbx_Center.TabStop = false;
            // 
            // lbl_CenterXY
            // 
            this.lbl_CenterXY.AutoSize = true;
            this.lbl_CenterXY.ForeColor = System.Drawing.Color.Gray;
            this.lbl_CenterXY.Location = new System.Drawing.Point(10, 634);
            this.lbl_CenterXY.Name = "lbl_CenterXY";
            this.lbl_CenterXY.Size = new System.Drawing.Size(63, 13);
            this.lbl_CenterXY.TabIndex = 0;
            this.lbl_CenterXY.Text = "Center (x,y):";
            // 
            // lbl_Scale
            // 
            this.lbl_Scale.AutoSize = true;
            this.lbl_Scale.ForeColor = System.Drawing.Color.Gray;
            this.lbl_Scale.Location = new System.Drawing.Point(218, 634);
            this.lbl_Scale.Name = "lbl_Scale";
            this.lbl_Scale.Size = new System.Drawing.Size(37, 13);
            this.lbl_Scale.TabIndex = 0;
            this.lbl_Scale.Text = "Scale:";
            // 
            // txtbx_Scale
            // 
            this.txtbx_Scale.BackColor = System.Drawing.Color.Black;
            this.txtbx_Scale.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtbx_Scale.ForeColor = System.Drawing.Color.Gray;
            this.txtbx_Scale.Location = new System.Drawing.Point(262, 632);
            this.txtbx_Scale.Name = "txtbx_Scale";
            this.txtbx_Scale.Size = new System.Drawing.Size(57, 20);
            this.txtbx_Scale.TabIndex = 0;
            this.txtbx_Scale.TabStop = false;
            // 
            // btn_Update
            // 
            this.btn_Update.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btn_Update.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Update.ForeColor = System.Drawing.Color.Gray;
            this.btn_Update.Location = new System.Drawing.Point(552, 631);
            this.btn_Update.Name = "btn_Update";
            this.btn_Update.Size = new System.Drawing.Size(54, 22);
            this.btn_Update.TabIndex = 0;
            this.btn_Update.TabStop = false;
            this.btn_Update.Text = "Update";
            this.btn_Update.UseVisualStyleBackColor = true;
            this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
            // 
            // btn_ColorSettings
            // 
            this.btn_ColorSettings.AutoSize = true;
            this.btn_ColorSettings.BackgroundImage = global::Mandlebrot.Properties.Resources.SettingsH;
            this.btn_ColorSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_ColorSettings.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btn_ColorSettings.FlatAppearance.BorderSize = 0;
            this.btn_ColorSettings.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btn_ColorSettings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.btn_ColorSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ColorSettings.Location = new System.Drawing.Point(792, 632);
            this.btn_ColorSettings.Name = "btn_ColorSettings";
            this.btn_ColorSettings.Size = new System.Drawing.Size(20, 20);
            this.btn_ColorSettings.TabIndex = 0;
            this.btn_ColorSettings.TabStop = false;
            this.btn_ColorSettings.UseVisualStyleBackColor = false;
            this.btn_ColorSettings.Click += new System.EventHandler(this.btn_ColorSettings_Click);
            // 
            // btn_ResetImage
            // 
            this.btn_ResetImage.BackgroundImage = global::Mandlebrot.Properties.Resources.reset;
            this.btn_ResetImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_ResetImage.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btn_ResetImage.FlatAppearance.BorderSize = 0;
            this.btn_ResetImage.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btn_ResetImage.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.btn_ResetImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ResetImage.ForeColor = System.Drawing.Color.Gray;
            this.btn_ResetImage.Location = new System.Drawing.Point(612, 632);
            this.btn_ResetImage.Name = "btn_ResetImage";
            this.btn_ResetImage.Size = new System.Drawing.Size(20, 20);
            this.btn_ResetImage.TabIndex = 0;
            this.btn_ResetImage.TabStop = false;
            this.btn_ResetImage.UseVisualStyleBackColor = false;
            this.btn_ResetImage.Click += new System.EventHandler(this.btn_ResetImage_Click);
            // 
            // pctbx_PaletteDisplay
            // 
            this.pctbx_PaletteDisplay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pctbx_PaletteDisplay.Location = new System.Drawing.Point(654, 632);
            this.pctbx_PaletteDisplay.Name = "pctbx_PaletteDisplay";
            this.pctbx_PaletteDisplay.Size = new System.Drawing.Size(132, 20);
            this.pctbx_PaletteDisplay.TabIndex = 0;
            this.pctbx_PaletteDisplay.TabStop = false;
            // 
            // btn_Maximize
            // 
            this.btn_Maximize.FlatAppearance.BorderSize = 0;
            this.btn_Maximize.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btn_Maximize.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.btn_Maximize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Maximize.Image = global::Mandlebrot.Properties.Resources.MaxButton;
            this.btn_Maximize.Location = new System.Drawing.Point(783, 0);
            this.btn_Maximize.Name = "btn_Maximize";
            this.btn_Maximize.Size = new System.Drawing.Size(20, 20);
            this.btn_Maximize.TabIndex = 0;
            this.btn_Maximize.TabStop = false;
            this.btn_Maximize.UseVisualStyleBackColor = true;
            this.btn_Maximize.Click += new System.EventHandler(this.btn_Maximize_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.FlatAppearance.BorderSize = 0;
            this.btn_Close.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btn_Close.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.btn_Close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Close.Image = global::Mandlebrot.Properties.Resources.X_button;
            this.btn_Close.Location = new System.Drawing.Point(803, 0);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(20, 20);
            this.btn_Close.TabIndex = 0;
            this.btn_Close.TabStop = false;
            this.btn_Close.UseVisualStyleBackColor = false;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // btn_ZoomIn
            // 
            this.btn_ZoomIn.AutoSize = true;
            this.btn_ZoomIn.BackgroundImage = global::Mandlebrot.Properties.Resources.ZoomIn;
            this.btn_ZoomIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_ZoomIn.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btn_ZoomIn.FlatAppearance.BorderSize = 0;
            this.btn_ZoomIn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btn_ZoomIn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.btn_ZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ZoomIn.Location = new System.Drawing.Point(325, 632);
            this.btn_ZoomIn.Name = "btn_ZoomIn";
            this.btn_ZoomIn.Size = new System.Drawing.Size(20, 20);
            this.btn_ZoomIn.TabIndex = 0;
            this.btn_ZoomIn.TabStop = false;
            this.btn_ZoomIn.UseVisualStyleBackColor = true;
            this.btn_ZoomIn.Click += new System.EventHandler(this.btn_ZoomIn_Click);
            // 
            // btn_ZoomOut
            // 
            this.btn_ZoomOut.BackgroundImage = global::Mandlebrot.Properties.Resources.ZoomOut;
            this.btn_ZoomOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btn_ZoomOut.FlatAppearance.BorderSize = 0;
            this.btn_ZoomOut.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Black;
            this.btn_ZoomOut.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.btn_ZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ZoomOut.Location = new System.Drawing.Point(351, 632);
            this.btn_ZoomOut.Name = "btn_ZoomOut";
            this.btn_ZoomOut.Size = new System.Drawing.Size(20, 20);
            this.btn_ZoomOut.TabIndex = 0;
            this.btn_ZoomOut.TabStop = false;
            this.btn_ZoomOut.UseVisualStyleBackColor = true;
            this.btn_ZoomOut.Click += new System.EventHandler(this.btn_ZoomOut_Click);
            // 
            // pctBx_Display
            // 
            this.pctBx_Display.BackColor = System.Drawing.Color.White;
            this.pctBx_Display.Location = new System.Drawing.Point(12, 26);
            this.pctBx_Display.Name = "pctBx_Display";
            this.pctBx_Display.Size = new System.Drawing.Size(800, 600);
            this.pctBx_Display.TabIndex = 0;
            this.pctBx_Display.TabStop = false;
            this.pctBx_Display.DoubleClick += new System.EventHandler(this.pctBx_Display_DoubleClick);
            // 
            // pnl_ColorEditor
            // 
            this.pnl_ColorEditor.Controls.Add(this.lbl_Palette);
            this.pnl_ColorEditor.Location = new System.Drawing.Point(100, 100);
            this.pnl_ColorEditor.Name = "pnl_ColorEditor";
            this.pnl_ColorEditor.Size = new System.Drawing.Size(633, 510);
            this.pnl_ColorEditor.TabIndex = 0;
            this.pnl_ColorEditor.Click += new System.EventHandler(this.pnl_ColorEditor_Click);
            // 
            // lbl_Palette
            // 
            this.lbl_Palette.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbl_Palette.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lbl_Palette.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Palette.ForeColor = System.Drawing.Color.Gray;
            this.lbl_Palette.Location = new System.Drawing.Point(0, 0);
            this.lbl_Palette.Name = "lbl_Palette";
            this.lbl_Palette.Size = new System.Drawing.Size(633, 40);
            this.lbl_Palette.TabIndex = 0;
            this.lbl_Palette.Text = "Palette Editor";
            this.lbl_Palette.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbl_Palette.Click += new System.EventHandler(this.lbl_Palette_Click);
            // 
            // lbl_IterationDepth
            // 
            this.lbl_IterationDepth.AutoSize = true;
            this.lbl_IterationDepth.ForeColor = System.Drawing.Color.Gray;
            this.lbl_IterationDepth.Location = new System.Drawing.Point(387, 634);
            this.lbl_IterationDepth.Name = "lbl_IterationDepth";
            this.lbl_IterationDepth.Size = new System.Drawing.Size(80, 13);
            this.lbl_IterationDepth.TabIndex = 0;
            this.lbl_IterationDepth.Text = "Iteration Depth:";
            // 
            // txtbx_IterationDepth
            // 
            this.txtbx_IterationDepth.BackColor = System.Drawing.Color.Black;
            this.txtbx_IterationDepth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtbx_IterationDepth.ForeColor = System.Drawing.Color.Gray;
            this.txtbx_IterationDepth.Location = new System.Drawing.Point(473, 632);
            this.txtbx_IterationDepth.Name = "txtbx_IterationDepth";
            this.txtbx_IterationDepth.Size = new System.Drawing.Size(57, 20);
            this.txtbx_IterationDepth.TabIndex = 2;
            this.txtbx_IterationDepth.TabStop = false;
            // 
            // MandlebrotDisp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(824, 671);
            this.Controls.Add(this.lbl_IterationDepth);
            this.Controls.Add(this.txtbx_IterationDepth);
            this.Controls.Add(this.pnl_ColorEditor);
            this.Controls.Add(this.btn_ZoomIn);
            this.Controls.Add(this.btn_ZoomOut);
            this.Controls.Add(this.btn_ColorSettings);
            this.Controls.Add(this.btn_ResetImage);
            this.Controls.Add(this.pctbx_PaletteDisplay);
            this.Controls.Add(this.btn_Update);
            this.Controls.Add(this.lbl_Scale);
            this.Controls.Add(this.txtbx_Scale);
            this.Controls.Add(this.lbl_CenterXY);
            this.Controls.Add(this.txtbx_Center);
            this.Controls.Add(this.btn_Maximize);
            this.Controls.Add(this.btn_DragRegion);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.pctBx_Display);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(824, 660);
            this.Name = "MandlebrotDisp";
            this.ResizeEnd += new System.EventHandler(this.MandlebrotDisp_ResizeEnd);
            ((System.ComponentModel.ISupportInitialize)(this.pctbx_PaletteDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctBx_Display)).EndInit();
            this.pnl_ColorEditor.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pctBx_Display;
        private System.Windows.Forms.Button btn_ZoomOut;
        private System.Windows.Forms.Button btn_ZoomIn;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.Button btn_DragRegion;
        private System.Windows.Forms.Button btn_Maximize;
        private System.Windows.Forms.TextBox txtbx_Center;
        private System.Windows.Forms.Label lbl_CenterXY;
        private System.Windows.Forms.Label lbl_Scale;
        private System.Windows.Forms.TextBox txtbx_Scale;
        private System.Windows.Forms.Button btn_Update;
        private System.Windows.Forms.PictureBox pctbx_PaletteDisplay;
        private System.Windows.Forms.Button btn_ResetImage;
        private System.Windows.Forms.Button btn_ColorSettings;
        public System.Windows.Forms.Panel pnl_ColorEditor;
        private System.Windows.Forms.Label lbl_Palette;
        private System.Windows.Forms.Label lbl_IterationDepth;
        private System.Windows.Forms.TextBox txtbx_IterationDepth;
    }
}

