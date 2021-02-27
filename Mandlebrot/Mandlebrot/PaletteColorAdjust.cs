using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Mandlebrot
{
    public partial class MandlebrotDisp : Form
    {
        private static PictureBox pctbx_PaletteEditor;
        private static List<ColorStop> PaletteColorStops;
        private static List<Slider> Sliders;

        public const int ColorStopTabHeight = 40;
        public const int ColorStopWidth = 11, ColorStopHeight = ColorStopTabHeight + PaletteEditorPaletteHeight;
        public static Size ColorStopSize = new Size(ColorStopWidth, ColorStopHeight);
        public const int PaletteEditorPaletteWidth = 603, PaletteEditorPaletteHeight = 50;
        public const int PaletteEditorPaletteX = 15, PaletteEditorPaletteY = 1;
        public const int ColorStopY = PaletteEditorPaletteY + PaletteEditorPaletteHeight;
        public const int PaletteEditorDisplayColorWidthInPixels = 1;
        public const int NumOfDivisionsFromZero = PaletteEditorPaletteWidth / PaletteEditorDisplayColorWidthInPixels - 1;
        public const int ColorStopOffset = (int)((ColorStopWidth) / 2) - (int)((PaletteEditorDisplayColorWidthInPixels) / 2);

        public static int SelectedColorStop = 0;

        private static Point ColorStopMouseDownLocation;
        private static bool CSCanMove;
        private static int LowerCSBoundX, UpperCSBoundX;

        private static Point SliderChevMouseDownLocation;
        private const int NumOfSliders = 3;
        private const int SliderLinesXMargin = 100;
        private const int SliderLineWidth = PaletteEditorPaletteWidth - 2 * SliderLinesXMargin;
        private const int SliderLinesX = SliderLinesXMargin + PaletteEditorPaletteX;
        private const int SliderLinesY = 165;

        public void InitializePaletteColorAdjust()
        {
            drawPalette();
            generateColorStops();
            generateComponentSliders();
            PaletteColorAdjustExit();
        }

        private void pctbx_PaletteEditor_Click(object sender, EventArgs e)
        {
            pctbx_PaletteEditor.Focus();
        }
        private void pctbx_PaletteEditor_DoubleClick(object sender, EventArgs e)
        {
            Point formScreenCoordLoc = pctbx_PaletteEditor.PointToScreen(new Point(0, 0));
            Point convertedMouseCoords = new Point(Control.MousePosition.X - formScreenCoordLoc.X, Control.MousePosition.Y - formScreenCoordLoc.Y);
            int newCSX = convertedMouseCoords.X - PaletteEditorPaletteX;

            int newCSLowerBound, newCSUpperBound;
            int LowerBoundCS = 0, UpperBoundCS = PaletteColorStops.Count - 1;
            if (newCSX >= 0 && newCSX < pctbx_PaletteEditor.Width)
            {
                int LowerBound = 0;
                for (int clrStp = 0; clrStp < PaletteColorStops.Count; clrStp++)
                {
                    if (PaletteColorStops[clrStp].CSLocation < newCSX && PaletteColorStops[clrStp].CSLocation > LowerBound)
                    {
                        LowerBound = PaletteColorStops[clrStp].CSLocation;
                        LowerBoundCS = clrStp;
                    }
                }
                newCSLowerBound = LowerBound + (int)(PaletteEditorPaletteWidth * 0.02);


                int UpperBound = pctbx_PaletteEditor.Width;
                for (int clrStp = 0; clrStp < PaletteColorStops.Count; clrStp++)
                {
                    if (PaletteColorStops[clrStp].CSLocation > newCSX && PaletteColorStops[clrStp].CSLocation < UpperBound)
                    {
                        UpperBound = PaletteColorStops[clrStp].CSLocation;
                        UpperBoundCS = clrStp;
                    }
                }
                newCSUpperBound = UpperBound - (int)(PaletteEditorPaletteWidth * 0.02);

                if (newCSLowerBound < newCSX && newCSUpperBound > newCSX && UpperBoundCS > LowerBoundCS)
                {
                    decimal newColorStopLocation = (decimal)newCSX / NumOfDivisionsFromZero;
                    Color newCSColor = ((Bitmap)pctbx_PaletteEditor.Image).GetPixel(newCSX, (int)(pctbx_PaletteEditor.Image.Height / 2));
                    int ColorStopLocation = ((int)(newColorStopLocation * NumOfDivisionsFromZero) * PaletteEditorDisplayColorWidthInPixels) + pctbx_PaletteEditor.Location.X - ColorStopOffset + PaletteEditorPaletteX;
                    ColorStop newCS = new ColorStop(100, ColorStopLocation, newCSColor, false, 
                        new EventHandler(ColorStop_Click), new EventHandler(ColorStop_DoubleClick), 
                        new MouseEventHandler(ColorStop_MouseDown), new MouseEventHandler(ColorStop_MouseMove));

                    PaletteColorStops.Insert(UpperBoundCS, newCS);
                    if (PaletteManager.CurrentPalette.HSL)
                    {
                        PaletteColor pc = new PaletteColor(newColorStopLocation, (decimal)newCSColor.R, (decimal)newCSColor.G, (decimal)newCSColor.B);
                        pc.convertToHSL();
                        PaletteManager.CurrentPalette.Insert(UpperBoundCS, pc);
                    }
                    else
                    {
                        PaletteColor pc = new PaletteColor(newColorStopLocation, (decimal)newCSColor.R, (decimal)newCSColor.G, (decimal)newCSColor.B);
                        PaletteManager.CurrentPalette.Insert(UpperBoundCS, pc);
                    }

                    int id = 0;
                    foreach (ColorStop cs in PaletteColorStops)
                    {
                        cs.ID = id;
                        cs.Image.Name = id.ToString();
                        id++;
                    }

                    pctbx_PaletteEditor.Controls.Add(PaletteColorStops[UpperBoundCS].Image);
                    SelectedColorStop = UpperBoundCS;
                    selectColorStop();

                }
            }
        }
        public void PaletteColorAdjustEnter()
        {
            pctbx_PaletteEditor.Visible = true;
            for (int pb = 0; pb < PaletteColorStops.Count; pb++)
            {
                PaletteColorStops[pb].Image.Visible = true;
            }

            for (int sc = 0; sc < Sliders.Count; sc++)
            {
                Sliders[sc].background.Visible = true;
                Sliders[sc].input.Visible = true;
                Sliders[sc].chev.Visible = true;
                Sliders[sc].componentLabel.Visible = true;
            }
            pctbx_PaletteEditor.Visible = true;
        }
        public void PaletteColorAdjustExit()
        {
            pctbx_PaletteEditor.Visible = false;
            for (int pb = 0; pb < PaletteColorStops.Count; pb++)
            {
                PaletteColorStops[pb].Image.Visible = false;
            }

            for (int sc = 0; sc < Sliders.Count; sc++)
            {
                Sliders[sc].background.Visible = false;
                Sliders[sc].input.Visible = false;
                Sliders[sc].chev.Visible = false;
                Sliders[sc].componentLabel.Visible = false;
            }
        }
        public void ResetPaletteColorAdjust()
        {
            for (int pb = 0; pb < PaletteColorStops.Count; pb++)
            {
                if (pnl_ColorEditor.Controls.Contains(PaletteColorStops[pb].Image))
                    pnl_ColorEditor.Controls.Remove(PaletteColorStops[pb].Image);
            }
            PaletteColorStops.Clear();

            for (int sc = 0; sc < Sliders.Count; sc++)
            {
                if (pnl_ColorEditor.Controls.Contains(Sliders[sc].background))
                    pnl_ColorEditor.Controls.Remove(Sliders[sc].background);
                if (pnl_ColorEditor.Controls.Contains(Sliders[sc].input))
                    pnl_ColorEditor.Controls.Remove(Sliders[sc].input);
                if (pnl_ColorEditor.Controls.Contains(Sliders[sc].chev))
                    pnl_ColorEditor.Controls.Remove(Sliders[sc].chev);
                if (pnl_ColorEditor.Controls.Contains(Sliders[sc].componentLabel))
                    pnl_ColorEditor.Controls.Remove(Sliders[sc].componentLabel);
            }
            Sliders.Clear();

            if(pnl_ColorEditor.Controls.Contains(pctbx_PaletteEditor))
                pnl_ColorEditor.Controls.Remove(pctbx_PaletteEditor);
            pctbx_PaletteEditor.Dispose();
        }


        private void drawPalette()
        {
            pctbx_PaletteEditor = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(pctbx_PaletteEditor)).BeginInit();
            // 
            // pctbx_PaletteEditor
            // 
            pctbx_PaletteEditor.BackColor = System.Drawing.Color.Transparent;
            pctbx_PaletteEditor.Location = new System.Drawing.Point(0, 55);
            pctbx_PaletteEditor.Name = "pctbx_PaletteEditor";
            pctbx_PaletteEditor.Size = new System.Drawing.Size(633, 90);
            pctbx_PaletteEditor.TabIndex = 0;
            pctbx_PaletteEditor.TabStop = false;
            pctbx_PaletteEditor.Click += new System.EventHandler(pctbx_PaletteEditor_Click);
            pctbx_PaletteEditor.DoubleClick += new System.EventHandler(pctbx_PaletteEditor_DoubleClick);
            ((System.ComponentModel.ISupportInitialize)(pctbx_PaletteEditor)).EndInit();
            pnl_ColorEditor.Controls.Add(pctbx_PaletteEditor);

            pctbx_PaletteEditor.Image = new Bitmap(pctbx_PaletteEditor.Width, pctbx_PaletteEditor.Height);

            int iteration = 0;
            for (int x = 0; x < pctbx_PaletteEditor.Width; x++)
                for (int y = 0; y < pctbx_PaletteEditor.Height; y++)
                    ((Bitmap)pctbx_PaletteEditor.Image).SetPixel(x, y, Color.Transparent);

            for (int x = PaletteEditorPaletteX - 1; x < PaletteEditorPaletteWidth + PaletteEditorPaletteX + 1; x++)
            {
                for (int y = PaletteEditorPaletteY - 1; y < PaletteEditorPaletteHeight + PaletteEditorPaletteY + 1; y++)
                {
                    ((Bitmap)pctbx_PaletteEditor.Image).SetPixel(x, y, Color.DarkGray);
                }
            }

            for (int x = PaletteEditorPaletteX; x < PaletteEditorPaletteWidth + PaletteEditorPaletteX; x++)
            {
                for (int y = PaletteEditorPaletteY; y < PaletteEditorPaletteHeight + PaletteEditorPaletteY; y++)
                {
                    ((Bitmap)pctbx_PaletteEditor.Image).SetPixel(x, y, PaletteManager.CurrentPalette.getColor((decimal)iteration / NumOfDivisionsFromZero));
                }
                iteration = (((x + 1) % PaletteEditorDisplayColorWidthInPixels == 0 && iteration < NumOfDivisionsFromZero) ? iteration + 1 : iteration);
            }
            pctbx_PaletteEditor.Refresh();
            renderPaletteEditor();
        }
        public void redrawPalette()
        {
            for (int x = PaletteEditorPaletteX; x < PaletteEditorPaletteWidth + PaletteEditorPaletteX; x++)
            {
                for (int y = PaletteEditorPaletteY; y < PaletteEditorPaletteHeight + PaletteEditorPaletteY; y++)
                {
                    ((Bitmap)pctbx_PaletteEditor.Image).SetPixel(x, y, PaletteManager.CurrentPalette.getColor((decimal)(x - PaletteEditorPaletteX) / NumOfDivisionsFromZero));
                }
                //iteration = (((x + 1) % PaletteEditorDisplayColorWidthInPixels == 0 && iteration < NumOfDivisionsFromZero) ? iteration + 1 : iteration);
            }
            pctbx_PaletteEditor.Refresh();
            renderPaletteEditor();
            RenderPalettePreviewDisplay();
        }


        private void generateColorStops()
        {
            int numColorStops = PaletteManager.CurrentPalette.Count;

            PaletteColorStops = new List<ColorStop>();

            //pnl_ColorEditor.SuspendLayout();
            for (int i = 0; i < PaletteManager.CurrentPalette.Count; i++)
            {
                PaletteColor pc = PaletteManager.CurrentPalette[i];
                Color currentColorStopColor;
                if (PaletteManager.CurrentPalette.HSL)
                    currentColorStopColor = Palette.HSLtoRGBConversion(pc.Component1, pc.Component2, pc.Component3);
                else
                    currentColorStopColor = Color.FromArgb((int)pc.Component1, (int)pc.Component2, (int)pc.Component3);

                int ColorStopLocation = ((int)(pc.Location * NumOfDivisionsFromZero) * PaletteEditorDisplayColorWidthInPixels) + pctbx_PaletteEditor.Location.X - ColorStopOffset + PaletteEditorPaletteX;
                ColorStop newCS = new ColorStop(i, ColorStopLocation, currentColorStopColor, i == SelectedColorStop,
                        new EventHandler(ColorStop_Click), new EventHandler(ColorStop_DoubleClick),
                        new MouseEventHandler(ColorStop_MouseDown), new MouseEventHandler(ColorStop_MouseMove));



                PaletteColorStops.Add(newCS);

                pctbx_PaletteEditor.Controls.Add(PaletteColorStops[i].Image);
            }
            //pnl_ColorEditor.ResumeLayout(false);
        }
        public static void ColorStop_Click(object sender, EventArgs e)
        {
            SelectedColorStop = int.Parse(((PictureBox)sender).Name);
            selectColorStop();
        }
        public void ColorStop_DoubleClick(object sender, EventArgs e)
        {
            int DoubleClickedCS = int.Parse(((PictureBox)sender).Name);
            bool canDeleteCS = (DoubleClickedCS > 0 && DoubleClickedCS < PaletteColorStops.Count - 1);
            if (canDeleteCS)
            {
                if (pctbx_PaletteEditor.Controls.Contains(PaletteColorStops[DoubleClickedCS].Image))
                    pctbx_PaletteEditor.Controls.Remove(PaletteColorStops[DoubleClickedCS].Image);
                PaletteColorStops.Remove(PaletteColorStops[DoubleClickedCS]);

                for (int i = 0; i < PaletteColorStops.Count; i++)
                {
                    PaletteColorStops[i].ID = i;
                    PaletteColorStops[i].Image.Name = i.ToString();
                }

                PaletteManager.CurrentPalette.removeColor(DoubleClickedCS);
                redrawPalette();
            }
        }
        private static void selectColorStop()
        {
            ColorStop clickedCS = PaletteColorStops[SelectedColorStop];

            if (!clickedCS.Selected)
            {
                for (int clrStp = 0; clrStp < PaletteColorStops.Count; clrStp++)
                {
                    if (PaletteColorStops[clrStp].Selected)
                    {
                        PaletteColorStops[clrStp].Selected = false;
                        PaletteColorStops[clrStp].Image = PaletteColorStops[clrStp].Image;
                        PaletteColorStops[clrStp].Image.Refresh();
                    }

                }
                clickedCS.Selected = true;
                PaletteColorStops[SelectedColorStop].Image.Refresh();
            }
            setColorStopSliderChevLocs();
        }
        public static void ColorStop_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                SelectedColorStop = int.Parse(((PictureBox)sender).Name);
                selectColorStop();
                ColorStopMouseDownLocation = e.Location;
                CSCanMove = SelectedColorStop > 0 && SelectedColorStop < PaletteColorStops.Count - 1;
                establishCSBounds();
            }
        }
        private static void establishCSBounds()
        {
            int CSX = PaletteColorStops[SelectedColorStop].CSLocation;
            int LowerBound = pctbx_PaletteEditor.Location.X - ColorStopOffset + PaletteEditorPaletteX;
            for (int clrStp = 0; clrStp < PaletteColorStops.Count; clrStp++)
            {
                if (clrStp != SelectedColorStop && PaletteColorStops[clrStp].CSLocation < CSX && PaletteColorStops[clrStp].CSLocation > LowerBound)
                {
                    LowerBound = PaletteColorStops[clrStp].CSLocation;
                }
            }
            LowerCSBoundX = LowerBound + (int)(PaletteEditorPaletteWidth * 0.02);


            int UpperBound = NumOfDivisionsFromZero * PaletteEditorDisplayColorWidthInPixels + pctbx_PaletteEditor.Location.X - ColorStopOffset + PaletteEditorPaletteX;
            for (int clrStp = 0; clrStp < PaletteColorStops.Count; clrStp++)
            {
                if (clrStp != SelectedColorStop && PaletteColorStops[clrStp].CSLocation > CSX && PaletteColorStops[clrStp].CSLocation < UpperBound)
                {
                    UpperBound = PaletteColorStops[clrStp].CSLocation;
                }
            }
            UpperCSBoundX = UpperBound - (int)(PaletteEditorPaletteWidth * 0.02);
        }
        public void ColorStop_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && CSCanMove)
            {
                int newX = e.X + ((PictureBox)sender).Left - ColorStopMouseDownLocation.X;
                if (
                    newX > pctbx_PaletteEditor.Location.X - ColorStopOffset + PaletteEditorPaletteX
                    && newX < NumOfDivisionsFromZero * PaletteEditorDisplayColorWidthInPixels + pctbx_PaletteEditor.Location.X - ColorStopOffset + PaletteEditorPaletteX
                    )
                {
                    if (newX < LowerCSBoundX)
                        newX = LowerCSBoundX;
                    else if (newX > UpperCSBoundX)
                        newX = UpperCSBoundX;

                    ((PictureBox)sender).Left = newX;
                    int clickedColorStop = int.Parse(((PictureBox)sender).Name);
                    PaletteColorStops[clickedColorStop].CSLocation = PaletteColorStops[clickedColorStop].Image.Location.X;
                    decimal colorStopPaletteLocation = ((decimal)(PaletteColorStops[clickedColorStop].CSLocation - (pctbx_PaletteEditor.Location.X - ColorStopOffset + PaletteEditorPaletteX))) / ((decimal)PaletteEditorPaletteWidth);
                    PaletteManager.CurrentPalette[clickedColorStop].Location = Math.Round(colorStopPaletteLocation, 3);
                    redrawPalette();
                }
                //((PictureBox)sender).Top = e.Y + ((PictureBox)sender).Top - MouseDownLocation.Y;
            }
                    //redrawPalette();
        }
        public void updateSelectedColorStop()
        {
            PaletteColor pc = PaletteManager.CurrentPalette[SelectedColorStop];
            Color currentColorStopColor;
            if (PaletteManager.CurrentPalette.HSL)
                currentColorStopColor = Palette.HSLtoRGBConversion(pc.Component1, pc.Component2, pc.Component3);
            else
                currentColorStopColor = Color.FromArgb((int)pc.Component1, (int)pc.Component2, (int)pc.Component3);

            PaletteColorStops[SelectedColorStop].Color = currentColorStopColor;
            PaletteColorStops[SelectedColorStop].Image.Refresh();
        }


        private void generateComponentSliders()
        {
            Sliders = new List<Slider>();
            for (int i = 0; i < NumOfSliders; i++)
            {
                Slider newSlider = new Slider(
                        i,
                        new Point(SliderLinesX - Slider.SliderLinesOffset, SliderLinesY - Slider.SliderLinesOffset + i * Slider.TotalHeight),
                        SliderLineWidth,
                        new MouseEventHandler(ComponentSliders_SliderLines_MouseUp),
                        new MouseEventHandler(ComponentSliders_SliderChev_MouseDown),
                        new MouseEventHandler(ComponentSliders_SliderChev_MouseMove),
                        new MouseEventHandler(ComponentSliders_SliderChev_MouseUp),
                        new KeyEventHandler(ComponentSliders_Input_KeyDown),
                        new EventHandler(ComponentSliders_Input_LostFocus)
                        );

                Sliders.Add(newSlider);
                pnl_ColorEditor.Controls.Add(Sliders[i].background);
                pnl_ColorEditor.Controls.Add(Sliders[i].input);
                pnl_ColorEditor.Controls.Add(Sliders[i].componentLabel);
            }
            pnl_ColorEditor.Refresh();
            setColorStopSliderChevLocs();
            setComponentLabels();
        }
        private void setComponentLabels()
        {
            if (PaletteManager.CurrentPalette.HSL)
            {
                Sliders[0].componentLabel.Text = "Hue";
                Sliders[1].componentLabel.Text = "Sat";
                Sliders[2].componentLabel.Text = "Light";
            }
            else
            {
                Sliders[0].componentLabel.Text = "Red";
                Sliders[1].componentLabel.Text = "Green";
                Sliders[2].componentLabel.Text = "Blue";
            }
        }
        private static void setColorStopSliderChevLocs()
        {
            decimal[] components = {
                                PaletteManager.CurrentPalette[SelectedColorStop].Component1,
                                PaletteManager.CurrentPalette[SelectedColorStop].Component2,
                                PaletteManager.CurrentPalette[SelectedColorStop].Component3
                            };

            if (PaletteManager.CurrentPalette.HSL)
                components[0] = components[0] / 360m;
            else
            {
                components[0] = components[0] / 255m;
                components[1] = components[1] / 255m;
                components[2] = components[2] / 255m;
            }

            for (int i = 0; i < NumOfSliders; i++)
            {
                Sliders[i].setSliderImgLocFromInput(components[i]);
            }
        }
        private void ComponentSliders_SliderLines_MouseUp(object sender, MouseEventArgs e)
        {
            int sliderID = Convert.ToInt32(((PictureBox)sender).Name);

            Sliders[sliderID].Left = e.X;

            updateSliderAndPalette(sliderID, Sliders[sliderID].SLoc);
            return;
        }
        private void ComponentSliders_SliderChev_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                SliderChevMouseDownLocation = e.Location;
            }
        }
        private void ComponentSliders_SliderChev_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                int sliderID = Convert.ToInt32(((PictureBox)sender).Name);

                Sliders[sliderID].Left = e.X + ((PictureBox)sender).Left - SliderChevMouseDownLocation.X + Slider.SliderChevRadius;
            }
        }
        private void ComponentSliders_SliderChev_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                int sliderID = Convert.ToInt32(((PictureBox)sender).Name);

                Sliders[sliderID].Left = e.X + ((PictureBox)sender).Left - SliderChevMouseDownLocation.X + Slider.SliderChevRadius;

                updateSliderAndPalette(sliderID, Sliders[sliderID].SLoc);
            }
        }
        private void ComponentSliders_Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                int sliderID = Convert.ToInt32(((TextBox)sender).Name);

                Sliders[sliderID].setSliderImgLocFromInput();

                updateSliderAndPalette(sliderID, Sliders[sliderID].SLoc);
            }
        }
        private void ComponentSliders_Input_LostFocus(object sender, EventArgs e)
        {
            int sliderID = Convert.ToInt32(((TextBox)sender).Name);

            Sliders[sliderID].setSliderImgLocFromInput();

            updateSliderAndPalette(sliderID, Sliders[sliderID].SLoc);
        }
        private void updateSliderAndPalette(int slider, decimal newLoc)
        {
            if (PaletteManager.CurrentPalette.HSL)
            {
                if (slider == 0)
                    newLoc = newLoc * 360m;
                else
                    newLoc = newLoc * 1m;
            }
            else
                newLoc = newLoc * 255m;


            if (slider == 0)
                PaletteManager.CurrentPalette[SelectedColorStop].Component1 = Math.Round(newLoc, 3);
            else if (slider == 1)
                PaletteManager.CurrentPalette[SelectedColorStop].Component2 = Math.Round(newLoc, 3);
            else if (slider == 2)
                PaletteManager.CurrentPalette[SelectedColorStop].Component3 = Math.Round(newLoc, 3);

            redrawPalette();
            updateSelectedColorStop();
        }


        public class ColorStop
        {
            public int ID { get; set; }
            public int CSLocation { get; set; }
            public PictureBox Image { get; set; }
            private bool selected;
            public bool Selected
            {
                set { selected = value; Image.Image = redraw(); }
                get { return this.selected; }
            }
            private Color colorStopColor;
            public Color Color
            {
                set
                {
                    colorStopColor = value;
                    Image.Image = redraw();
                }
            }

            public ColorStop(int id, int loc, Color clr, bool selected, EventHandler ColorStop_Click, EventHandler ColorStop_DoubleClick, MouseEventHandler ColorStop_MouseDown, MouseEventHandler ColorStop_MouseMove)
            {
                this.ID = id;
                this.CSLocation = loc;
                this.selected = selected;
                this.colorStopColor = clr;

                this.Image = new PictureBox();

                this.Image.Location = new Point(this.CSLocation, 0);
                this.Image.Name = id.ToString();
                this.Image.Size = ColorStopSize;
                this.Image.TabIndex = 0;
                this.Image.TabStop = false;
                this.Image.Visible = true;
                this.Image.Enabled = true;
                this.Image.BackColor = Color.Transparent;
                this.Image.Image = redraw();
                this.Image.Click += ColorStop_Click;
                this.Image.DoubleClick += ColorStop_DoubleClick;
                this.Image.MouseDown += ColorStop_MouseDown;
                this.Image.MouseMove += ColorStop_MouseMove;
            }
            private Bitmap redraw()
            {
                Bitmap clrStpImg = new Bitmap(ColorStopSize.Width, ColorStopSize.Height);
                for (int i = 0; i < clrStpImg.Width; i++)
                    for (int j = 0; j < clrStpImg.Height; j++)
                    {
                        if (j >= MandlebrotDisp.ColorStopY + 5)
                        {
                            if (i == 0 || i == clrStpImg.Width - 1 || j == MandlebrotDisp.ColorStopY + 5 || j == clrStpImg.Height - 1)
                                clrStpImg.SetPixel(i, j, Color.DarkGray);
                            else if ((i > 0 && i < 2) || (i > clrStpImg.Width - 3 && i < clrStpImg.Width - 1)
                                    || (j > MandlebrotDisp.ColorStopY + 5 && j < MandlebrotDisp.ColorStopY + 7) || (j > clrStpImg.Height - 3 && j < clrStpImg.Height - 1))
                                clrStpImg.SetPixel(i, j, Color.Black);
                            else if ((i == 2 || i == clrStpImg.Width - 3) && (j <= clrStpImg.Height - 3 && j >= MandlebrotDisp.ColorStopY + 7))
                                clrStpImg.SetPixel(i, j, Color.DarkGray);
                            else if ((j == MandlebrotDisp.ColorStopY + 7 || j == clrStpImg.Height - 3) && (i <= clrStpImg.Width - 3 && i >= 2))
                                clrStpImg.SetPixel(i, j, Color.DarkGray);
                            else
                                clrStpImg.SetPixel(i, j, colorStopColor);
                        }
                        else if (j > MandlebrotDisp.ColorStopY && j < MandlebrotDisp.ColorStopY + 5 && (i >= 4 && i <= 6))
                            clrStpImg.SetPixel(i, j, Color.DarkGray);
                        else if (j >= 0 && j <= MandlebrotDisp.ColorStopY)
                        {
                            if (this.selected)
                            {
                                if (
                                    i == 0 || i == clrStpImg.Width - 1 || j == 0 || j == MandlebrotDisp.ColorStopY ||
                                    i == 1 || i == clrStpImg.Width - 2 || j == 1 || j == MandlebrotDisp.ColorStopY - 1
                                    )
                                    clrStpImg.SetPixel(i, j, Color.DarkGray);
                                else
                                    clrStpImg.SetPixel(i, j, colorStopColor);
                            }
                        }
                        else
                            clrStpImg.SetPixel(i, j, Color.Transparent);
                    }
                return clrStpImg;
            }

        }

    }
}