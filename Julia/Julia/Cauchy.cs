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
        private void renderDisplayImageCAUCHYV1()
        {
            int imgWidth = pctBx_Display.Width, imgHeight = pctBx_Display.Height;


            double adjPlotScale = PlotScale * MaxPlotScale;

            PointD firstPixelGCoord = new PointD(
                                                    Plot_CenterCoord.X - 0.5d * adjPlotScale * imgWidth,
                                                    Plot_CenterCoord.Y - 0.5d * adjPlotScale * imgHeight
                                                    );

            Complex[,] lattice = new Complex[imgWidth, imgHeight];
            for (int i = 0; i < imgWidth; i++)
                for (int j = 0; j < imgHeight; j++)
                    lattice[i, j] = new Complex(i * adjPlotScale + firstPixelGCoord.X, j * adjPlotScale + firstPixelGCoord.Y);

            int maxIterations = 100;
            Complex eta = new Complex(1e-300, 1e-300);
            double[,] sumLattice = new double[imgWidth, imgHeight];
            int workers = 32;

            Parallel.For(0, workers, index =>
            {
                for (int i = (int)(((double)index / workers) * imgWidth); i < (int)(((double)(index + 1) / workers) * imgWidth); i++)
                {
                    for (int j = 0; j < imgHeight; j++)
                        sumLattice[i, j] = computeSum(lattice[i, j], eta, maxIterations);

                }
            }
            );

            double delta = 1e-20, expv, stdev;
            computeStats(sumLattice, delta, out expv, out stdev);

            double K = expv + 0.5 * stdev;
            double[,] juliaLattice = new double[imgWidth, imgHeight];
            for (int i = 0; i < imgWidth; i++)
                for (int j = 0; j < imgHeight; j++)
                    juliaLattice[i, j] = Math.Log(delta + sumLattice[i, j]) - K;

            pctBx_Display.Image = new Bitmap(imgWidth, imgHeight);
            decimal juliaColor;
            for (int i = 0; i < imgWidth; i++)
            {
                for (int j = 0; j < imgHeight; j++)
                {
                    //juliaColor = (int)((juliaLattice[i, j] / (4 * stdev))*255) % 255;
                    juliaColor = (decimal)(0.5 + (1 / Math.PI * Math.Atan(Math.PI * 1 * juliaLattice[i, j])));

                    if (juliaLattice[i, j] > 0)
                        ((Bitmap)pctBx_Display.Image).SetPixel(i, j, Color.White);
                    //((Bitmap)pctBx_Display.Image).SetPixel(i, j, PaletteManager.CurrentPalette.getColor(juliaColor));
                    else
                        ((Bitmap)pctBx_Display.Image).SetPixel(i, j, Color.Black);
                }
            }

            pctBx_Display.Refresh();
            System.Media.SystemSounds.Beep.Play();
        }

        private double tw(Complex z, Complex w)
        {
            double result;
            if (w == Complex.Zero)
                result = z.Magnitude;
            else if (double.IsInfinity(w.Real) || double.IsInfinity(w.Imaginary))
                result = 1 / z.Magnitude;
            else if (double.IsInfinity(z.Real) || double.IsInfinity(z.Imaginary))
                result = 1 / w.Magnitude;
            else
                result = (1 / w.Magnitude) * ((z * Complex.Conjugate(w) - w.Magnitude * w.Magnitude) / (z * Complex.Conjugate(w) + 1)).Magnitude;

            if (double.IsNaN(result))
                return z.Magnitude;
            else
                return result;
        }
        private double dMetric(Complex z, Complex w)
        {
            return 2 * Math.Atan(tw(z, w));
        }
        private Complex[] juliaFunctionCauchy(Complex z, int maxIterations)
        {
            Complex[] outputs = new Complex[maxIterations];
            Complex c = new Complex(0.001, 0);
            Complex prevz;
            for (int k = 0; k < maxIterations; k++)
            {
                prevz = new Complex(z.Real, z.Imaginary);
                //z -= (z * z * z - 1) / (3 * z * z);
                //z = (z * z * z + c)/z;
                //z = z * z;
                if (double.IsNaN(z.Real) || double.IsNaN(z.Imaginary) || double.IsInfinity(z.Real) || double.IsInfinity(z.Imaginary))
                {
                    outputs[k] = prevz;
                    for (int kinf = k + 1; kinf < maxIterations; kinf++)
                        outputs[kinf] = outputs[k];
                    return outputs;
                }
                outputs[k] = z;
            }
            return outputs;
        }
        private double computeSum(Complex z, Complex eta, int maxIterations)
        {
            double sum = 0;
            Complex[] juliaZOutputs = juliaFunctionCauchy(z, maxIterations);
            Complex[] juliaZEtaOutputs = juliaFunctionCauchy(z + eta, maxIterations);
            for (int k = 0; k < maxIterations; k++)
            {
                sum += dMetric(juliaZOutputs[k], juliaZEtaOutputs[k]);
            }
            return sum;
        }
        private void computeStats(double[,] sumLattice, double delta, out double expv, out double stdev)
        {
            double[] xLattice = new double[sumLattice.Length];
            int ind = 0;
            foreach (double d in sumLattice)
                xLattice[ind++] = Math.Log(delta + d);

            double EX = 0, EX2 = 0;
            foreach (double x in xLattice)
            {
                EX += x;
                EX2 += x * x;
            }

            if (xLattice.Length > 0)
            {
                EX /= xLattice.Length;
                EX2 /= xLattice.Length;
            }
            else
                MessageBox.Show("E1");

            expv = EX;
            stdev = Math.Sqrt(EX2 - EX * EX);
        }
    }
}