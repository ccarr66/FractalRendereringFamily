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
        private void renderDisplayImageHist()
        {
            int imgWidth = (int)(pctBx_Display.Width), imgHeight = (int)(pctBx_Display.Height);

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
            int numIntensity = 256;

            double[,] rawBuffer = new double[imgWidth, imgHeight];
            Complex eta = (new Complex(adjPlotScale, adjPlotScale)) / 1e5;
            Parallel.For(0, workers, index =>
            {
                for (int i = (int)(((double)index / workers) * imgWidth); i < (int)(((double)(index + 1) / workers) * imgWidth); i++)
                    for (int j = 0; j < imgHeight; j++)
                        rawBuffer[i, j] = juliaPixelHist(lattice[i, j], eta, maxIterations);
            }
            );

            double maxVal = 0;
            for (int i = 0; i < imgWidth; i++)
                for (int j = 0; j < imgHeight; j++)
                {
                    if (rawBuffer[i, j] > maxVal)
                        maxVal = rawBuffer[i, j];
                }

            int[,] pixelBuffer = new int[imgWidth, imgHeight];
            for (int i = 0; i < imgWidth; i++)
                for (int j = 0; j < imgHeight; j++)
                {
                    pixelBuffer[i, j] = (int)((numIntensity - 1) * (rawBuffer[i, j] / maxVal)) % numIntensity;
                }

            double[] Pr = new double[numIntensity];
            for (int i = 0; i < imgWidth; i++)
                for (int j = 0; j < imgHeight; j++)
                {
                    Pr[pixelBuffer[i, j]]++;
                }

            for (int nk = 0; nk < numIntensity; nk++)
                Pr[nk] /= imgHeight * imgWidth;

            int[] sk = new int[numIntensity];
            for (int rj = 0; rj < numIntensity; rj++)
            {
                double rsum = 0;

                for (int nk = 0; nk < rj; nk++)
                    rsum += Pr[nk];

                sk[rj] = (int)Math.Round(rsum * (double)(numIntensity - 1));
            }

            for (int i = 0; i <imgWidth; i++)
                for (int j = 0; j < imgHeight; j++)
                {
                    pixelBuffer[i,j] = sk[pixelBuffer[i, j]] % numIntensity;
                }

            pctBx_Display.Image = new Bitmap(imgWidth, imgHeight);
            for (int i = 0; i < imgWidth; i++)
                for (int j = 0; j < imgHeight; j++)
                {
                    ((Bitmap)pctBx_Display.Image).SetPixel(i, j, Color.FromArgb(pixelBuffer[i, j], pixelBuffer[i, j], pixelBuffer[i, j]));

                }


            pctBx_Display.Refresh();
            System.Media.SystemSounds.Beep.Play();
        }
        private double juliaPixelHist(Complex z, Complex eta, int maxIterations)
        {
            Complex[] z1Values = juliaValues(z, maxIterations);
            Complex[] z2Values = juliaValues(z + eta, maxIterations);

            double sumDif = 0;

            Tuple<int, int> z1Inf = cIsInfinity(z1Values[maxIterations - 1]);
            Tuple<int, int> z2Inf = cIsInfinity(z2Values[maxIterations - 1]);

            if (z1Inf.Item1 != z2Inf.Item1 || z1Inf.Item2 != z2Inf.Item2)
                sumDif = double.PositiveInfinity;
            else
            {
                for (int v = 0; v < maxIterations; v++)
                {
                    sumDif += (z1Values[v] - z2Values[v]).Magnitude;
                    if (double.IsNaN(sumDif) || double.IsInfinity(sumDif) || sumDif > 1e150)
                    {
                        //sumDif = double.PositiveInfinity;
                        break;
                    }
                }
            }

            return sumDif;
        }

        private Complex[] juliaValues(Complex z, int maxIterations)
        {
            //Complex d = new Complex(-0.2, -0.7);
            //Complex a = new Complex(1.005, 0.002);
            //Complex b = new Complex(0.1, 0);
            //Complex c = new Complex(-0.75, 0.11);
            Complex c = new Complex(-0.2, -0.7);
            Complex[] outputs = new Complex[maxIterations];

            for (int k = 0; k < maxIterations; k++)
            {
                //z = (z * z) / (Complex.Pow(z, 9) - z + 0.025);
                z -= (z * z * z - 1) / (3 * z * z);
                //z = z * z + c;
                //z = z * z;
                //z = (z * z) / (Complex.Pow(z, 9) + 2 * z + 0.05);
                //z = (z * z * z * z * z - z) / (20 * z * z + 1);
                //z = z / (z * z * z * z + 6 * z * z + 1.001);
                //z = (z * z * z - z) / (d * z * z + 1);
                //z = (z * z) / (Complex.Pow(z, 9) + 2 * z + 0.001);
                //z = (a * z * z * z + b) / (z * z + b);

                if (double.IsNaN(z.Real) || double.IsNaN(z.Imaginary) || double.IsInfinity(z.Real) || double.IsInfinity(z.Imaginary) || z.Magnitude > 1e150)
                {
                    if (double.IsNaN(z.Real) || double.IsNaN(z.Imaginary))
                        outputs[k] = new Complex(double.PositiveInfinity, double.PositiveInfinity);
                    else
                        outputs[k] = z;

                    for (int kinf = k + 1; kinf < maxIterations; kinf++)
                        outputs[kinf] = outputs[k];
                    return outputs;
                }
                outputs[k] = z;
            }
            return outputs;
        }

        private Tuple<int,int> cIsInfinity(Complex z)
        {
            int real = 0;
            if (double.IsPositiveInfinity(z.Real)) real = 1;
            else if (double.IsNegativeInfinity(z.Real)) real = -1;

            int img = 0;
            if (double.IsPositiveInfinity(z.Imaginary)) img = 1;
            else if (double.IsNegativeInfinity(z.Imaginary)) img = -1;

            return new Tuple<int, int>(real, img);
        }
    }
}
