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
        private void renderDisplayImageEdgeDetection()
        {
            double pointsPerPixel = 4;
            int imgWidth = (int)(pctBx_Display.Width * pointsPerPixel), imgHeight = (int)(pctBx_Display.Height * pointsPerPixel);

            double adjPlotScale = PlotScale * MaxPlotScale / pointsPerPixel;

            PointD firstPixelGCoord = new PointD(
                                                    Plot_CenterCoord.X - 0.5d * adjPlotScale * imgWidth,
                                                    Plot_CenterCoord.Y + 0.5d * adjPlotScale * imgHeight
                                                    );

            Complex[,] lattice = new Complex[imgWidth, imgHeight];
            for (int i = 0; i < imgWidth; i++)
                for (int j = 0; j < imgHeight; j++)
                    lattice[i, j] = new Complex(i * adjPlotScale + firstPixelGCoord.X, firstPixelGCoord.Y - j * adjPlotScale);

            int workers = 32;
            int maxIterations = 1000;

            Complex[,] pixelBuffer = new Complex[imgWidth, imgHeight];
            Parallel.For(0, workers, index =>
            {
                for (int i = (int)(((double)index / workers) * imgWidth); i < (int)(((double)(index + 1) / workers) * imgWidth); i++)
                    for (int j = 0; j < imgHeight; j++)
                        pixelBuffer[i, j] = juliaPixelEdgeDetection(lattice[i, j], maxIterations);
            }
            );

            double[,] Gx = new double[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            double[,] Gy = new double[3, 3] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
            double[,] magnitude = new double[imgWidth, imgHeight];
            double S1, S2;
            double max = 0;
            for (int i = 0; i < imgWidth - 3; i++)
                for (int j = 0; j < imgHeight - 3; j++)
                {
                    S1 = 0; S2 = 0;
                    for (int k = 0; k < 3; k++)
                        for (int l = 0; l < 3; l++)
                        {
                            S1 += Math.Atan(pixelBuffer[i + k, j + l].Magnitude) * 4 * Gx[k, l];
                            S2 += Math.Atan(pixelBuffer[i + k, j + l].Magnitude) * 4 * Gy[k, l];
                            //S1 += ((pixelBuffer[i + k, j + l].Phase > 0) ? pixelBuffer[i + k, j + l].Phase : pixelBuffer[i + k, j + l].Phase + 2 * Math.PI) * Gx[k, l];
                            //S2 += ((pixelBuffer[i + k, j + l].Phase > 0) ? pixelBuffer[i + k, j + l].Phase : pixelBuffer[i + k, j + l].Phase + 2 * Math.PI) * Gy[k, l];
                        }
                    magnitude[i + 1, j + 1] = Math.Sqrt(S1 * S1 + S2 * S2);
                    if (magnitude[i + 1, j + 1] > max)
                        max = magnitude[i + 1, j + 1];
                }

            double threshold = 0.1;
            int[,] img = new int[pctBx_Display.Width, pctBx_Display.Height];
            for (int i = 0; i < imgWidth; i++)
                for (int j = 0; j < imgHeight; j++)
                {
                    if (magnitude[i, j] / max > threshold)
                        img[(int)((double)i / pointsPerPixel), (int)((double)j / pointsPerPixel)]++;
                }

            pctBx_Display.Image = new Bitmap(imgWidth, imgHeight);
            double scaleFactor = 255d / (pointsPerPixel * pointsPerPixel);
            for (int i = 0; i < pctBx_Display.Width; i++)
                for (int j = 0; j < pctBx_Display.Height; j++)
                {
                    ((Bitmap)pctBx_Display.Image).SetPixel(i, j, Color.FromArgb((int)((double)img[i, j]*scaleFactor), (int)((double)img[i, j] * scaleFactor), (int)((double)img[i, j] * scaleFactor)));

                }


            pctBx_Display.Refresh();
            System.Media.SystemSounds.Beep.Play();
        }
        private Complex juliaPixelEdgeDetection(Complex z, int maxIterations)
        {
            //Complex d = new Complex(-0.2, -0.7);
            //Complex a = new Complex(1.005, 0.002);
            //Complex b = new Complex(0.1, 0);
            //Complex c = new Complex(-0.75, 0.11);
            Complex c = new Complex(-0.2, -0.7);
            Complex prevZ;
            for (int k = 0; k < maxIterations; k++)
            {
                prevZ = z;
                //z -= (z * z * z + 0.5 * z - 1.5) * (3 * z * z + 0.5) / ((3 * z * z + 0.5) * (3 * z * z + 0.5) - (3 * z * (z * z * z + 0.5 * z - 1.5)));
                //z = (z * z) / (Complex.Pow(z, 9) - z + 0.025);
                //z -= (z * z * z - 1) / (3 * z * z);
                //z = z / (Complex.Log(z*z));
                //z = z * z + c;
                //z = z * z;
                //z = (z * z) / (Complex.Pow(z, 9) + 2 * z + 0.05);
                //z = (z * z * z * z * z - z) / (20 * z * z + 1);
                //z = z / (z * z * z * z + 6 * z * z + 1.001);
                //z = (z * z * z - z) / (d * z * z + 1);
                z = (z * z) / (Complex.Pow(z, 9) + 2 * z + 0.001);
                //z = (a * z * z * z + b) / (z * z + b);

                if (double.IsNaN(z.Real) || double.IsNaN(z.Imaginary) || double.IsInfinity(z.Real) || double.IsInfinity(z.Imaginary) || z.Magnitude > 1e50)
                {
                    //return prevZ;
                    return new Complex(double.PositiveInfinity, double.PositiveInfinity);
                    //break;
                }
            }
            return z;
        }
    }
}
