using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mandlebrot
{
    public partial class MandlebrotDisp : Form
    {
        private static PictureBox pctbx_PaletteGenEditorDisplay;
        private static Slider PaletteLengthSlider;
        private static Slider OffsetSlider;

        private const int PLSliderX = 90;
        private const int PLSliderY = 375;
        private const int PLSliderID = 1;

        private const int OSSliderX = 90;
        private const int OSSliderY = PLSliderY + Slider.TotalHeight + 8;
        private const int OSSliderID = 2;

        private Point PGA_SliderMouseDownLocation;

        public void InitializePaletteGenAdjust()
        {
            generatePaletteGenEditorDisplay();
            generatePaletteGenSliders();
            PaletteGenAdjustExit();
        }
        public void PaletteGenAdjustEnter()
        {
            SetPaletteLengthSliderLoc();
            PaletteLengthSlider.background.Visible = true;
            PaletteLengthSlider.input.Visible = true;
            PaletteLengthSlider.chev.Visible = true;
            PaletteLengthSlider.componentLabel.Visible = true;

            SetOffsetSliderLoc();
            OffsetSlider.background.Visible = true;
            OffsetSlider.input.Visible = true;
            OffsetSlider.chev.Visible = true;
            OffsetSlider.componentLabel.Visible = true;

            renderPaletteEditor();
            pctbx_PaletteGenEditorDisplay.Visible = true;
        }
        public void PaletteGenAdjustExit()
        {
            PaletteLengthSlider.background.Visible = false;
            PaletteLengthSlider.input.Visible = false;
            PaletteLengthSlider.chev.Visible = false;
            PaletteLengthSlider.componentLabel.Visible = false;

            OffsetSlider.background.Visible = false;
            OffsetSlider.input.Visible = false;
            OffsetSlider.chev.Visible = false;
            OffsetSlider.componentLabel.Visible = false;

            pctbx_PaletteGenEditorDisplay.Visible = false;
        }

        public void generatePaletteGenEditorDisplay()
        {
            pctbx_PaletteGenEditorDisplay = new PictureBox();
            // 
            // pctbx_PaletteDisplay
            // 
            pctbx_PaletteGenEditorDisplay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            pctbx_PaletteGenEditorDisplay.Location = new System.Drawing.Point(25, 284);
            pctbx_PaletteGenEditorDisplay.Name = "pctbx_PaletteGenEditorDisplay";
            pctbx_PaletteGenEditorDisplay.Size = new System.Drawing.Size(347, 40);
            pctbx_PaletteGenEditorDisplay.TabIndex = 0;
            pctbx_PaletteGenEditorDisplay.TabStop = false;
            pctbx_PaletteGenEditorDisplay.Image = new Bitmap(347, 40);
            renderPaletteEditor();
            pctbx_PaletteGenEditorDisplay.Visible = false;
            pnl_ColorEditor.Controls.Add(pctbx_PaletteGenEditorDisplay);
        }
        public void generatePaletteGenSliders()
        {
            PaletteLengthSlider = new Slider(
                    PLSliderID,
                    new Point(PLSliderX - Slider.SliderLinesOffset, PLSliderY - Slider.SliderLinesOffset),
                    200,
                    new MouseEventHandler(PGA_SliderLines_MouseUp),
                    new MouseEventHandler(PGA_SliderChev_MouseDown),
                    new MouseEventHandler(PGA_SliderChev_MouseMove),
                    new MouseEventHandler(PGA_SliderChev_MouseUp),
                    new KeyEventHandler(PGA_Input_KeyDown),
                    new EventHandler(PGA_Input_LostFocus)
                    );
            PaletteLengthSlider.componentLabel.Text = "Palette\nLength";
            PaletteLengthSlider.componentLabel.AutoSize = true;
            PaletteLengthSlider.componentLabel.Font = new Font(PaletteLengthSlider.componentLabel.Font.FontFamily, 9);
            PaletteLengthSlider.componentLabel.Location = new Point(PaletteLengthSlider.componentLabel.Location.X + 6, PaletteLengthSlider.componentLabel.Location.Y);
            PaletteLengthSlider.componentLabel.TextAlign = ContentAlignment.MiddleRight;

            pnl_ColorEditor.Controls.Add(PaletteLengthSlider.background);
            pnl_ColorEditor.Controls.Add(PaletteLengthSlider.input);
            pnl_ColorEditor.Controls.Add(PaletteLengthSlider.componentLabel);

            OffsetSlider = new Slider(
                    OSSliderID,
                    new Point(OSSliderX - Slider.SliderLinesOffset, OSSliderY - Slider.SliderLinesOffset),
                    200,
                    new MouseEventHandler(PGA_SliderLines_MouseUp),
                    new MouseEventHandler(PGA_SliderChev_MouseDown),
                    new MouseEventHandler(PGA_SliderChev_MouseMove),
                    new MouseEventHandler(PGA_SliderChev_MouseUp),
                    new KeyEventHandler(PGA_Input_KeyDown),
                    new EventHandler(PGA_Input_LostFocus)
                    );
            OffsetSlider.componentLabel.Text = "Offset\n(%)";
            OffsetSlider.componentLabel.AutoSize = true;
            OffsetSlider.componentLabel.Font = new Font(OffsetSlider.componentLabel.Font.FontFamily, 9);
            OffsetSlider.componentLabel.Location = new Point(OffsetSlider.componentLabel.Location.X + 12, OffsetSlider.componentLabel.Location.Y);
            OffsetSlider.componentLabel.TextAlign = ContentAlignment.MiddleRight;

            pnl_ColorEditor.Controls.Add(OffsetSlider.background);
            pnl_ColorEditor.Controls.Add(OffsetSlider.input);
            pnl_ColorEditor.Controls.Add(OffsetSlider.componentLabel);

            pnl_ColorEditor.Refresh();
        }
        public void renderPaletteEditor()
        {
            PaletteManager.colorSetup();
            double scale = (double)PaletteManager.IterationLim / pctbx_PaletteGenEditorDisplay.Width;
            for (int x = 0; x < pctbx_PaletteGenEditorDisplay.Width; x++)
            {
                for (int y = 0; y < pctbx_PaletteGenEditorDisplay.Height; y++)
                    ((Bitmap)pctbx_PaletteGenEditorDisplay.Image).SetPixel(x, y, PaletteManager.ColorContainer(((int)(scale * x)) % (int)PaletteManager.IterationLim));
            }
            pctbx_PaletteGenEditorDisplay.Refresh();

            RenderPalettePreviewDisplay();
        }

        private void PGA_SliderLines_MouseUp(object sender, MouseEventArgs e)
        {
            int sliderID = Convert.ToInt32(((PictureBox)sender).Name);

            switch(sliderID)
            {
                case PLSliderID:
                    {
                        PaletteLengthSlider.Left = e.X;
                        UpdatePaletteLengthSlider(PaletteLengthSlider.SLoc);
                        break;
                    }
                case OSSliderID:
                    {
                        OffsetSlider.Left = e.X;
                        UpdateOffsetSlider(OffsetSlider.SLoc);
                        break;
                    }
            }
            return;
        }

        private void PGA_SliderChev_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                PGA_SliderMouseDownLocation = e.Location;
            }
        }
        private void PGA_SliderChev_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                int sliderID = Convert.ToInt32(((PictureBox)sender).Name);

                switch (sliderID)
                {
                    case PLSliderID:
                        {
                            PaletteLengthSlider.Left = e.X + ((PictureBox)sender).Left - PGA_SliderMouseDownLocation.X + Slider.SliderChevRadius;
                            UpdatePaletteLengthSlider(PaletteLengthSlider.SLoc);
                            break;
                        }
                    case OSSliderID:
                        {
                            OffsetSlider.Left = e.X + ((PictureBox)sender).Left - PGA_SliderMouseDownLocation.X + Slider.SliderChevRadius;
                            UpdateOffsetSlider(OffsetSlider.SLoc);
                            break;
                        }
                }
            }
        }
        private void PGA_SliderChev_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                int sliderID = Convert.ToInt32(((PictureBox)sender).Name);

                switch (sliderID)
                {
                    case PLSliderID:
                        {
                            PaletteLengthSlider.Left = e.X + ((PictureBox)sender).Left - PGA_SliderMouseDownLocation.X + Slider.SliderChevRadius;
                            UpdatePaletteLengthSlider(PaletteLengthSlider.SLoc);
                            break;
                        }
                    case OSSliderID:
                        {
                            OffsetSlider.Left = e.X + ((PictureBox)sender).Left - PGA_SliderMouseDownLocation.X + Slider.SliderChevRadius;
                            UpdateOffsetSlider(OffsetSlider.SLoc);
                            break;
                        }
                }

                //updateSliderAndPalette(sliderID, Sliders[sliderID].SLoc);
            }
        }
        private void PGA_Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                int sliderID = Convert.ToInt32(((TextBox)sender).Name);

                switch (sliderID)
                {
                    case PLSliderID:
                        {
                            PaletteLengthSlider.setSliderImgLocFromInput();
                            UpdatePaletteLengthSlider(PaletteLengthSlider.SLoc);
                            break;
                        }
                    case OSSliderID:
                        {
                            OffsetSlider.setSliderImgLocFromInput();
                            UpdateOffsetSlider(OffsetSlider.SLoc);
                            break;
                        }
                }

            }
        }
        private void PGA_Input_LostFocus(object sender, EventArgs e)
        {
            int sliderID = Convert.ToInt32(((TextBox)sender).Name);

            switch (sliderID)
            {
                case PLSliderID:
                    {
                        PaletteLengthSlider.setSliderImgLocFromInput();
                        UpdatePaletteLengthSlider(PaletteLengthSlider.SLoc);
                        break;
                    }
                case OSSliderID:
                    {
                        OffsetSlider.setSliderImgLocFromInput();
                        UpdateOffsetSlider(OffsetSlider.SLoc);
                        break;
                    }
            }
        }

        private void UpdatePaletteLengthSlider(decimal SLoc)
        {
            if (SLoc > 0)
            {
                PaletteManager.PaletteLength = (int)(PaletteManager.IterationLim * SLoc);
                renderPaletteEditor();
            }
        }
        private void SetPaletteLengthSliderLoc()
        {
            PaletteLengthSlider.setSliderImgLocFromInput((decimal)PaletteManager.PaletteLength / (PaletteManager.IterationLim));
            renderPaletteEditor();
        }
        private void UpdateOffsetSlider(decimal SLoc)
        {
            PaletteManager.Offset = SLoc;
            renderPaletteEditor();
        }
        private void SetOffsetSliderLoc()
        {
            OffsetSlider.setSliderImgLocFromInput(PaletteManager.Offset);
            renderPaletteEditor();
        }
    }
}