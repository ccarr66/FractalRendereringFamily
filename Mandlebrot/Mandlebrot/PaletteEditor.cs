using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mandlebrot
{
    public partial class MandlebrotDisp : Form
    {
        public Bitmap PanelImage;
        public const int DarkTransparency = 180;
        public const int HorizontalLineLoc = 255;
        public const int VerticalLineLoc = 400;
        public const int LineRadius = 3;

        private void pnlEnter()
        {
            lbl_Palette.Visible = true;

            Point pixelOffset = new Point(pnl_ColorEditor.Location.X - pctBx_Display.Location.X, pnl_ColorEditor.Location.Y - pctBx_Display.Location.Y);
            pnl_ColorEditor.BackgroundImage = generatePnlImage(pnl_ColorEditor.Width, pnl_ColorEditor.Height, pixelOffset, (Bitmap)pctBx_Display.Image);
            pnl_ColorEditor.BackgroundImage = applyAreaLines((Bitmap)pnl_ColorEditor.BackgroundImage);

            PaletteGenAdjustEnter();
            PaletteColorAdjustEnter();
            PaletteSelectEnter();
            PalettePreviewEnter();

            pnl_ColorEditor.Invalidate();
        }
        private static Bitmap generatePnlImage(int Width, int Height, Point pixelOffset, Bitmap Image)
        {
            Bitmap PanelImage = new Bitmap(Width, Height);
            //Point pixelOffset = new Point(pnl_ColorEditor.Location.X - pctBx_Display.Location.X, pnl_ColorEditor.Location.Y - pctBx_Display.Location.Y);

            double colorSum = 0;
            for (int x = 0; x < PanelImage.Width; x++)
                for (int y = 0; y < PanelImage.Height; y++)
                {
                    colorSum += Image.GetPixel(x + pixelOffset.X, y + pixelOffset.Y).R;
                    colorSum += Image.GetPixel(x + pixelOffset.X, y + pixelOffset.Y).G;
                    colorSum += Image.GetPixel(x + pixelOffset.X, y + pixelOffset.Y).B;
                }
            int avgColor = (int)(colorSum / (PanelImage.Width * PanelImage.Height * 3));

            //Color pixelUnderPnl, InvColor, avgInvColor;
            //int avgComponent;
            int invAvgColor = (255 - avgColor);
            Color Overlay = Color.FromArgb(180, invAvgColor, invAvgColor, invAvgColor);

            Color composite, pixelUnderPnl;
            double alpha = (double)Overlay.A / 255;
            int compRed, compGreen, compBlue;
            for (int x = 0; x < PanelImage.Width; x++)
                for (int y = 0; y < PanelImage.Height; y++)
                {
                    /*
                    pixelUnderPnl = ((Bitmap)(pctBx_Display.Image)).GetPixel(x + pixelOffset.X, y + pixelOffset.Y);
                    InvColor = Color.FromArgb((int)((255 - pixelUnderPnl.R) * 0.5), (int)((255 - pixelUnderPnl.G) * 0.5), (int)((255 - pixelUnderPnl.B) * 0.5));
                    avgComponent = (InvColor.R + InvColor.G + InvColor.B) / 3;
                    avgInvColor = Color.FromArgb(avgComponent, avgComponent, avgComponent);
                    */
                    pixelUnderPnl = Image.GetPixel(x + pixelOffset.X, y + pixelOffset.Y);
                    compRed = ((int)(alpha * Overlay.R + (1 - alpha) * pixelUnderPnl.R));
                    compGreen = ((int)(alpha * Overlay.G + (1 - alpha) * pixelUnderPnl.G));
                    compBlue = ((int)(alpha * Overlay.B + (1 - alpha) * pixelUnderPnl.B));

                    composite = Color.FromArgb(255, compRed, compGreen, compBlue);

                    PanelImage.SetPixel(x, y, composite);
                }

            return PanelImage;
        }
        private static Bitmap applyAreaLines(Bitmap Image)
        {
            
            for (int x = 0; x < Image.Width; x++)
                for (int y = 0; y < Image.Height; y++)
                {
                    if (y <= HorizontalLineLoc + LineRadius && y >= HorizontalLineLoc - LineRadius)
                    {
                        int red, green, blue;
                        Color overlay = Color.FromArgb(DarkTransparency, Color.Black);
                        decimal alpha = (decimal)DarkTransparency / 255;
                        red = ((int)(alpha * overlay.R + (1 - alpha) * Image.GetPixel(x, y).R));
                        green = ((int)(alpha * overlay.G + (1 - alpha) * Image.GetPixel(x, y).G));
                        blue = ((int)(alpha * overlay.B + (1 - alpha) * Image.GetPixel(x, y).B));
                        Image.SetPixel(x, y, Color.FromArgb(red, green, blue));
                    }
                    else if((y > HorizontalLineLoc + LineRadius && y < Image.Height - PaletteSelectFieldHeight) && (x <= VerticalLineLoc + LineRadius && x >= VerticalLineLoc - LineRadius))
                    {
                        int red, green, blue;
                        Color overlay = Color.FromArgb(DarkTransparency, Color.Black);
                        decimal alpha = (decimal)DarkTransparency / 255;
                        red = ((int)(alpha * overlay.R + (1 - alpha) * Image.GetPixel(x, y).R));
                        green = ((int)(alpha * overlay.G + (1 - alpha) * Image.GetPixel(x, y).G));
                        blue = ((int)(alpha * overlay.B + (1 - alpha) * Image.GetPixel(x, y).B));
                        Image.SetPixel(x, y, Color.FromArgb(red, green, blue));
                    }
                }
            return Image;
        }
        private void pnlExit()
        {
            PaletteManager.setCurrentPaletteCopy();
            PaletteColorAdjustExit();
            PaletteGenAdjustExit();
            PaletteSelectExit();
            PalettePreviewExit();

            lbl_Palette.Visible = false;
        }
    }
}
