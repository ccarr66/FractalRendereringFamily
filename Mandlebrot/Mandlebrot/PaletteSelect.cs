using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mandlebrot
{
    public partial class MandlebrotDisp : Form
    {
        private const int PaletteSelectFieldHeight = 34;
        private static PictureBox pctbx_PaletteSelectBackground;

        private static ComboBox cmbBx_AvailablePalettes;
        private static Button btn_LoadPalette;
        private static Button btn_Revert;
        private static CheckBox chbx_Cyclic;
        private static CheckBox chbx_Random;

        public void InitializePaletteSelect()
        {
            if (pctbx_PaletteSelectBackground == null)
                generatePaletteSelectBackground();

            //lbl_AvailablePalettes = new System.Windows.Forms.Label();
            cmbBx_AvailablePalettes = new System.Windows.Forms.ComboBox();
            btn_LoadPalette = new System.Windows.Forms.Button();
            btn_Revert = new System.Windows.Forms.Button();
            // 
            // cmbBx_AvailablePalettes
            // 
            cmbBx_AvailablePalettes.BackColor = System.Drawing.Color.Black;
            cmbBx_AvailablePalettes.FlatStyle = System.Windows.Forms.FlatStyle.System;
            cmbBx_AvailablePalettes.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBx_AvailablePalettes.ForeColor = System.Drawing.Color.Gray;
            cmbBx_AvailablePalettes.FormattingEnabled = true;
            cmbBx_AvailablePalettes.Location = new System.Drawing.Point(5, 6);
            cmbBx_AvailablePalettes.Name = "cmbBx_AvailablePalettes";
            cmbBx_AvailablePalettes.Size = new System.Drawing.Size(121, 20);
            cmbBx_AvailablePalettes.TabIndex = 0;
            cmbBx_AvailablePalettes.TabStop = false;
            // 
            // btn_LoadPalette
            // 
            btn_LoadPalette.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            btn_LoadPalette.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn_LoadPalette.ForeColor = System.Drawing.Color.Gray;
            btn_LoadPalette.Location = new System.Drawing.Point(132, 5);
            btn_LoadPalette.Name = "btn_LoadPalette";
            btn_LoadPalette.Size = new System.Drawing.Size(80, 22);
            btn_LoadPalette.TabIndex = 0;
            btn_LoadPalette.TabStop = false;
            btn_LoadPalette.Text = "Load Palette";
            btn_LoadPalette.Click += new EventHandler(btn_LoadPalette_Click);
            btn_LoadPalette.UseVisualStyleBackColor = true;
            // 
            // btn_Revert
            // 
            btn_Revert.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            btn_Revert.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn_Revert.ForeColor = System.Drawing.Color.Gray;
            btn_Revert.Location = new System.Drawing.Point(221, 5);
            btn_Revert.Name = "btn_Revert";
            btn_Revert.Size = new System.Drawing.Size(80, 22);
            btn_Revert.TabIndex = 0;
            btn_Revert.TabStop = false;
            btn_Revert.Text = "Revert";
            btn_Revert.Click += new EventHandler(btn_Revert_Click);
            btn_Revert.UseVisualStyleBackColor = true;
            // 
            // chbx_Cyclic
            // 
            chbx_Cyclic = new CheckBox();
            chbx_Cyclic.Location = new System.Drawing.Point(480, 8);
            chbx_Cyclic.Name = "chbx_Cyclic";
            chbx_Cyclic.TabIndex = 0;
            chbx_Cyclic.TabStop = false;
            chbx_Cyclic.Visible = false;
            chbx_Cyclic.BackColor = Color.Transparent;
            chbx_Cyclic.Text = "Cyclic:";
            chbx_Cyclic.ForeColor = Color.Gray;
            chbx_Cyclic.AutoSize = true;
            chbx_Cyclic.CheckAlign = ContentAlignment.MiddleRight;
            chbx_Cyclic.Click += new EventHandler(chbx_Cyclic_Click);
            chbx_Cyclic.Checked = PaletteManager.Cyclic;
            // 
            // chbx_Random
            // 
            chbx_Random = new CheckBox();
            chbx_Random.Location = new System.Drawing.Point(550, 8);
            chbx_Random.Name = "chbx_Random";
            chbx_Random.TabIndex = 0;
            chbx_Random.TabStop = false;
            chbx_Random.Visible = false;
            chbx_Random.BackColor = Color.Transparent;
            chbx_Random.Text = "Random:";
            chbx_Random.ForeColor = Color.Gray;
            chbx_Random.AutoSize = true;
            chbx_Random.CheckAlign = ContentAlignment.MiddleRight;
            chbx_Random.Click += new EventHandler(chbx_Random_Click);
            chbx_Random.Checked = PaletteManager.RandomColors;

            pctbx_PaletteSelectBackground.Controls.Add(btn_Revert);
            pctbx_PaletteSelectBackground.Controls.Add(btn_LoadPalette);
            pctbx_PaletteSelectBackground.Controls.Add(chbx_Cyclic);
            pctbx_PaletteSelectBackground.Controls.Add(chbx_Random);
            pctbx_PaletteSelectBackground.Controls.Add(cmbBx_AvailablePalettes);


            updatePaletteList();

            //lbl_AvailablePalettes.Visible = false;
            cmbBx_AvailablePalettes.Visible = false;
            pctbx_PaletteSelectBackground.Visible = false;
            btn_LoadPalette.Visible = false;
            chbx_Cyclic.Visible = false;
            chbx_Random.Visible = false;
            btn_Revert.Visible = false;
        }
        public static void PaletteSelectEnter()
        {
            SelectedColorStop = 0;
            cmbBx_AvailablePalettes.Visible = true;
            pctbx_PaletteSelectBackground.Visible = true;
            btn_LoadPalette.Visible = true;
            chbx_Cyclic.Visible = true;
            chbx_Random.Visible = true;
            btn_Revert.Visible = true;
        }
        public static void PaletteSelectExit()
        {
            //lbl_AvailablePalettes.Visible = false;
            cmbBx_AvailablePalettes.Visible = false;
            pctbx_PaletteSelectBackground.Visible = false;
            btn_LoadPalette.Visible = false;
            chbx_Cyclic.Visible = false;
            chbx_Random.Visible = false;
            btn_Revert.Visible = false;
        }

        private void generatePaletteSelectBackground()
        {
            pctbx_PaletteSelectBackground = new PictureBox();

            ((System.ComponentModel.ISupportInitialize)(pctbx_PaletteSelectBackground)).BeginInit();
            // 
            // pctbx_PaletteEditor
            // 
            pctbx_PaletteSelectBackground.BackColor = System.Drawing.Color.Transparent;
            pctbx_PaletteSelectBackground.Location = new System.Drawing.Point(0, pnl_ColorEditor.Height - PaletteSelectFieldHeight);
            pctbx_PaletteSelectBackground.Name = "pctbx_PaletteSelectBackground";
            pctbx_PaletteSelectBackground.Size = new System.Drawing.Size(pnl_ColorEditor.Width, PaletteSelectFieldHeight);
            pctbx_PaletteSelectBackground.TabIndex = 0;
            pctbx_PaletteSelectBackground.TabStop = false;
            ((System.ComponentModel.ISupportInitialize)(pctbx_PaletteSelectBackground)).EndInit();
            pnl_ColorEditor.Controls.Add(pctbx_PaletteSelectBackground);

            pctbx_PaletteSelectBackground.Image = new Bitmap(pnl_ColorEditor.Width, PaletteSelectFieldHeight);

            for (int x = 0; x < pctbx_PaletteSelectBackground.Width; x++)
                for (int y = 0; y < pctbx_PaletteSelectBackground.Height; y++)
                    ((Bitmap)pctbx_PaletteSelectBackground.Image).SetPixel(x, y, Color.FromArgb(DarkTransparency, Color.Black));

            pctbx_PaletteSelectBackground.Refresh();
        }

        private static void updatePaletteList()
        {
            cmbBx_AvailablePalettes.Items.Clear();
            for (int p = 0; p < PaletteManager.NumberOfPalettes; p++)
                cmbBx_AvailablePalettes.Items.Add(PaletteManager.AvailablePalettes(p).Name);
            cmbBx_AvailablePalettes.SelectedIndex = PaletteManager.IndCurrentPalette;
        }
        private void btn_LoadPalette_Click(object sender, EventArgs e)
        {
            SelectedColorStop = 0;
            PaletteManager.IndCurrentPalette = cmbBx_AvailablePalettes.SelectedIndex;
            ResetPaletteColorAdjust();
            InitializePaletteColorAdjust();
            PaletteColorAdjustEnter();
        }
        private void btn_Revert_Click(object sender, EventArgs e)
        {
            SelectedColorStop = 0;
            PaletteManager.setCurrentPaletteCopy();
            ResetPaletteColorAdjust();
            InitializePaletteColorAdjust();
            PaletteColorAdjustEnter();
        }
        private void chbx_Cyclic_Click(object sender, EventArgs e)
        {
            if (chbx_Cyclic.Checked)
                PaletteManager.Cyclic = true;
            else
                PaletteManager.Cyclic = false;

            renderPaletteEditor();
        }
        private void chbx_Random_Click(object sender, EventArgs e)
        {
            if (chbx_Random.Checked)
                PaletteManager.RandomColors = true;
            else
                PaletteManager.RandomColors = false;

            renderPaletteEditor();
        }
    }
}