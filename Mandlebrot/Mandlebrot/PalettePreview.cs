using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Mandlebrot
{
    public partial class MandlebrotDisp : Form
    {
        private int previewIterationDepth = 1000;
        PictureBox pctbx_PalettePreviewDisplay;
        private void InitializePalettePreview()
        {
            GeneratePalettePreviewDisplay();
            pctbx_PalettePreviewDisplay.Visible = false;
        }
        private void PalettePreviewEnter()
        {
            RenderPalettePreviewDisplay();
            pctbx_PalettePreviewDisplay.Visible = true;
        }
        private void PalettePreviewExit()
        {
            pctbx_PalettePreviewDisplay.Visible = false;
        }
        private void GeneratePalettePreviewDisplay()
        {
            // 
            // PalettePreviewDisplay
            // 
            pctbx_PalettePreviewDisplay = new PictureBox();
            this.pctbx_PalettePreviewDisplay.BackColor = System.Drawing.Color.White;
            this.pctbx_PalettePreviewDisplay.Location = new System.Drawing.Point(429, 284);
            this.pctbx_PalettePreviewDisplay.Name = "PalettePreviewDisplay";
            this.pctbx_PalettePreviewDisplay.Size = new System.Drawing.Size(179, 167);
            this.pctbx_PalettePreviewDisplay.TabIndex = 0;
            this.pctbx_PalettePreviewDisplay.TabStop = false;
            this.pctbx_PalettePreviewDisplay.Image = new Bitmap(pctbx_PalettePreviewDisplay.Width, pctbx_PalettePreviewDisplay.Height);
            RenderPalettePreviewDisplay();
            this.pnl_ColorEditor.Controls.Add(pctbx_PalettePreviewDisplay);
        }
        private void RenderPalettePreviewDisplay()
        {
            if (pctbx_PalettePreviewDisplay != null)
            {
                double adjPlotScale = PlotScale * MaxPlotScale * pctBx_Display.Width / pctbx_PalettePreviewDisplay.Size.Width;
                PointD firstPixelGCoord = new PointD(
                                                        Plot_CenterCoord.X - 0.5d * adjPlotScale * pctbx_PalettePreviewDisplay.Size.Width,
                                                        Plot_CenterCoord.Y - 0.5d * adjPlotScale * pctbx_PalettePreviewDisplay.Size.Height
                                                        );

                int color;
                Color inMandSet = Color.Black;
                for (int y = 0; y < pctbx_PalettePreviewDisplay.Height; y++)
                {
                    for (int x = 0; x < pctbx_PalettePreviewDisplay.Width; x++)
                    {
                        color = mandlebrotPixel(x * adjPlotScale + firstPixelGCoord.X, y * adjPlotScale + firstPixelGCoord.Y, previewIterationDepth);
                        ((Bitmap)pctbx_PalettePreviewDisplay.Image).SetPixel(x, y, color >= 0 ? PaletteManager.ColorContainer(color) : inMandSet);
                    }
                }

                pctbx_PalettePreviewDisplay.Refresh();
            }
        }
    }
}