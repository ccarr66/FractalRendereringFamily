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

        private void renderDisplayImageBasinFinder()
        {
            int imgWidth = pctBx_Display.Width, imgHeight = pctBx_Display.Height;

            double adjPlotScale = PlotScale * MaxPlotScale;

            PointD firstPixelGCoord = new PointD(
                                                    Plot_CenterCoord.X - 0.5d * adjPlotScale * imgWidth,
                                                    Plot_CenterCoord.Y + 0.5d * adjPlotScale * imgHeight
                                                    );

            Complex[,] lattice = new Complex[imgWidth, imgHeight];
            for (int i = 0; i < imgWidth; i++)
                for (int j = 0; j < imgHeight; j++)
                    lattice[i, j] = new Complex(i * adjPlotScale + firstPixelGCoord.X, firstPixelGCoord.Y - j * adjPlotScale);

            int workers = 32;
            int maxIterations = 300;
            double convergenceThreshold = 0.999;
            double infinityThreshold = 1e250;

            Color[,] pixelBuffer = new Color[imgWidth, imgHeight];
            Parallel.For(0, workers, index =>
                {
                    for (int i = (int)(((double)index / workers) * imgWidth); i < (int)(((double)(index + 1) / workers) * imgWidth); i++)
                        for (int j = 0; j < imgHeight; j++)
                            pixelBuffer[i, j] = juliaPixelBasinFinder(lattice[i, j], maxIterations, convergenceThreshold, infinityThreshold);
                }
            );

            pctBx_Display.Image = new Bitmap(imgWidth, imgHeight);
            //double xCoord, yCoord;
            //double alphaDottedLine = (double)180 / 255;
            for (int i = 0; i < imgWidth; i++)
            {
                for (int j = 0; j < imgHeight; j++)
                {

                    /*xCoord = i * adjPlotScale + firstPixelGCoord.X;
                    yCoord = firstPixelGCoord.Y - j * adjPlotScale;

                    if(Math.Abs(xCoord % 0.5) < adjPlotScale || Math.Abs(yCoord % 0.5) < adjPlotScale)
                    {
                        int r = ((int)(alphaDottedLine * (255 - pixelBuffer[i, j].R) + (1 - alphaDottedLine) * pixelBuffer[i, j].R));
                        int g = ((int)(alphaDottedLine * (255 - pixelBuffer[i, j].G) + (1 - alphaDottedLine) * pixelBuffer[i, j].G));
                        int b = ((int)(alphaDottedLine * (255 - pixelBuffer[i, j].B) + (1 - alphaDottedLine) * pixelBuffer[i, j].B));
                        ((Bitmap)pctBx_Display.Image).SetPixel(i, j, Color.FromArgb(r, g, b));
                    }
                    else
                        ((Bitmap)pctBx_Display.Image).SetPixel(i, j, pixelBuffer[i, j]);*/
                    ((Bitmap)pctBx_Display.Image).SetPixel(i, j, pixelBuffer[i, j]);

                }
            }

            pctBx_Display.Refresh();
            System.Media.SystemSounds.Beep.Play();
        }
        private Color juliaPixelBasinFinder(Complex z, int maxIterations, double convergenceThreshold, double infinityThreshold)
        {

            //Complex c = new Complex(-0.2, 0);
            //Complex pow = new Complex(1.5, 0);
            //Complex c = z;
            //Complex c = new Complex(0.5, 0.5);
            //Complex c = new Complex(0.5, 0.78);
            //Complex c1 = new Complex(0.005, -0.005); 
            //Complex c2 = new Complex(0, -4);
            //Complex c3 = new Complex(0, 0.001);
            //Complex d = new Complex(-0.003, 0.999);
            Complex c = new Complex(-0.2, 0.7);
            //Complex b = new Complex(0.917, 0); 
            //Complex a = new Complex(1.005, 0.002);
            //Complex b = new Complex(0.1, 0);

            Complex[] Fn = new Complex[maxIterations];

            for (int k = 0; k < maxIterations; k++)
            {
                //z -= z * z * z * z;
                //z = z * z + 0.7885 * Complex.Exp(Complex.ImaginaryOne * 0.5 * 2 * Math.PI);
                //z = z * z + c;
                //z -= (z * z * z - 1) / (3 * z * z);
                //z = (z * z * z * z * z + c) / (z * z * z);
                //z = z * z;
                //z -= (z * z * z + 0.5 * z - 1.5) * (3 * z * z + 0.5) / ((3 * z * z + 0.5) * (3 * z * z + 0.5) - (3 * z * (z * z * z + 0.5 * z - 1.5)));
                //z = Complex.Pow(z, pow) + c;
                //z = z * z * z + 1.55 * z * z / (z * z + 1);
                //z = z / (Complex.Log(z*z));
                
                //z = (z * z * z+ c) / (z * z * z - c);
                //z = ((2 * c * z * z * z) + (2 * z * z)) / ((z * z * z) + (3 * c * z * z) - z + c);
                //z = (z * z * z + z * c) / (z + 1);
                //z = (-4.0004 * z * z * z * z * z + c1) / (c2 * z * z * z * z + c3);
                
                //z = (z * z) / (Complex.Pow(z, 9) + 2 * z + 0.05);
                //z = (z * z) / (Complex.Pow(z, 9) - z + 0.025);

                //z = new Complex((z * z * z * z * z).Real % 4, (z * z * z * z * z).Imaginary % 4);
                //z = z - Complex.Log(z/(z.Magnitude));
                z = z / (z * z * z * z + 6 * z * z + 1.001);
                //z = (z * z * z - z) / (d * z * z + 1);
                //z = (z * z) / (Complex.Pow(z,9) - z + 0.025);
                //z = (z * z * z * z * z - z) / (20 * z * z + 1);
                //z = (z * z + a) / (z * z + b);
                //z = (a*z * z * z + b) / (z * z + b);

                if (double.IsNaN(z.Real) || double.IsNaN(z.Imaginary) || double.IsInfinity(z.Real) || double.IsInfinity(z.Imaginary) || z.Magnitude > 1e50)
                {
                    Fn[k] = new Complex(double.PositiveInfinity, double.PositiveInfinity);
                    for (int kinf = k + 1; kinf < maxIterations; kinf++)
                        Fn[kinf] = Fn[k];
                    break;
                }
                Fn[k] = z;
            }

            int convergenceIteration = maxIterations -1;
            for (int k = 0; k < maxIterations - 1; k++)
            {
                if ((Fn[maxIterations - 1] - Fn[k]).Magnitude < convergenceThreshold || double.IsInfinity(Fn[k].Real) || double.IsInfinity(Fn[k].Imaginary))
                {
                    convergenceIteration = k;
                    break;
                }
            }
            /*
            double magnitude = double.IsNaN((Fn[convergenceIteration].Magnitude)) ? double.PositiveInfinity : Fn[convergenceIteration].Magnitude;
            decimal hue = (decimal)((720 / Math.PI) * Math.Atan(magnitude));
            */
            
            
            decimal hue;
            if (Fn[maxIterations - 1].Magnitude > infinityThreshold)
                hue = 0m;
            else
                hue = ((decimal)(360 / (2 * Math.PI) * ((Fn[maxIterations - 1].Phase < 0) ? Fn[maxIterations - 1].Phase + 2 * Math.PI : Fn[maxIterations - 1].Phase)));
              
              /*
            decimal hue;
            if (Fn[convergenceIteration].Magnitude > infinityThreshold)
                hue = 0m;
            else
                hue = ((decimal)(360 / (2 * Math.PI) * ((Fn[convergenceIteration].Phase < 0) ? Fn[convergenceIteration].Phase + 2 * Math.PI : Fn[convergenceIteration].Phase)));
               */ 

            decimal sat = 1;
            decimal val = 1;
            //decimal val = 1 - (decimal)convergenceIteration / maxIterations;
            return HSVtoRGBConversion(hue, sat, val);
        }
    }
}