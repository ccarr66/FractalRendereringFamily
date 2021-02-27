using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Microsoft.VisualStudio.Modeling;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace Julia
{
    public partial class Julia : Form
    {
        private void renderDisplayImageTRACE()
        {
            int imgWidth = pctBx_Display.Width, imgHeight = pctBx_Display.Height;


            double adjPlotScale = PlotScale * MaxPlotScale;

            PointD firstPixelGCoord = new PointD(
                                                    Plot_CenterCoord.X - 0.5d * adjPlotScale * imgWidth,
                                                    Plot_CenterCoord.Y - 0.5d * adjPlotScale * imgHeight
                                                    );

            int[,] traceContainer = new int[imgWidth, imgHeight];
            
            Complex c = new Complex(0.001, 0);
            Complex z = new Complex(0,0);
            int maxIterations = 1000, numPoints = 2000000;
            double radius, phase;
            int x, y;
            Random v = new Random();
            for (int i = 0; i < numPoints; i++)
            {
                radius = v.NextDouble() * 3;
                phase = v.NextDouble() * 2 * Math.PI;
                z = new Complex(radius * Math.Cos(phase), radius * Math.Sin(phase));
                for (int k = 0; k < maxIterations; k++)
                {
                    //z = z*z*z - 2 * z + 2;
                    //z = z * z;
                    //z = (z * z * z + c)/z;
                    //z = z / (z * z * z * z + 6 * z * z + 1.001);
                    z = (z * z) / (Complex.Pow(z, 9) - z + 0.025);

                    if (double.IsNaN(z.Real) || double.IsNaN(z.Imaginary) || double.IsInfinity(z.Real) || double.IsInfinity(z.Imaginary))
                        break;

                    x = (int)((z.Real - firstPixelGCoord.X) / adjPlotScale);
                    y = (int)((z.Imaginary - firstPixelGCoord.Y) / adjPlotScale);

                    if (x < 0 || y < 0 || x >= imgWidth || y >= imgHeight)
                        break;
                    else
                        traceContainer[x,y]++;
                }

            }

            double maxVal = 0;
            for (int i = 0; i < imgWidth; i++)
                for (int j = 0; j < imgHeight; j++)
                    maxVal = (traceContainer[i, j] > maxVal) ? traceContainer[i, j] : maxVal;

            pctBx_Display.Image = new Bitmap(imgWidth, imgHeight);
            //Color pixelColor;
            //decimal hue, sat, light;
            int component;
            Complex p;
            for (int i = 0; i < imgWidth; i++)
            {
                for (int j = 0; j < imgHeight; j++)
                {

                    //hue = (decimal)traceContainer[i, j] / avgTrace * 360m;
                    //sat = 1m;
                    //sat = (decimal)traceContainer[i,j]/avgTrace;
                    //light = 0.5m;
                    //pixelColor = HSLtoRGBConversion(hue, sat, light);
                    p = new Complex(i * adjPlotScale + firstPixelGCoord.X, j * adjPlotScale + firstPixelGCoord.Y);
                    if (Math.Abs(p.Magnitude - 2) < 0.01)
                        ((Bitmap)pctBx_Display.Image).SetPixel(i, j, Color.Red);
                    else
                    {
                        component = (int)((1 - (double)traceContainer[i, j] / maxVal) * 255d) % 255;
                        ((Bitmap)pctBx_Display.Image).SetPixel(i, j, Color.FromArgb(component, component, component));
                    }
                }
            }

            pctBx_Display.Refresh();
            System.Media.SystemSounds.Beep.Play();
        }
    }
}