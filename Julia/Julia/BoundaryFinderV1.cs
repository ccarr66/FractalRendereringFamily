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
        private void renderDisplayImageBoundaryFinderV1()
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

            int workers = 32;
            int maxIterations = 100;
            int numPasses = 3;
            int numPoints = 10;
            double etaDecreaseFactor = 10;
            double maxIterationsIncreaseFactor = 1.5;
            double numPointsIncreaseFactor = 1.5;
            double additionalPassThreshold = 0.8;
            Complex eta = new Complex(1, 1);

            double[,] sumLattice = new double[imgWidth, imgHeight];
            bool[,] needsAdditionalPass = new bool[imgWidth, imgHeight];
            for (int p = 0; p < numPasses; p++)
            {
                eta = new Complex(eta.Real / etaDecreaseFactor, eta.Imaginary / etaDecreaseFactor);
                maxIterations = (int)((double)maxIterations * maxIterationsIncreaseFactor);
                numPoints = (int)((double)numPoints * numPointsIncreaseFactor);

                Parallel.For(0, workers, index =>
                {
                    for (int i = (int)(((double)index / workers) * imgWidth); i < (int)(((double)(index + 1) / workers) * imgWidth); i++)
                    {
                        for (int j = 0; j < imgHeight; j++)
                        {
                            if (p == 0 || needsAdditionalPass[i, j])
                            {
                                sumLattice[i, j] = computeSumBoundaryFinderV1(lattice[i, j], eta, maxIterations, adjPlotScale, numPoints);
                                needsAdditionalPass[i, j] = (sumLattice[i, j] >= additionalPassThreshold);
                                if (!needsAdditionalPass[i, j])
                                    sumLattice[i, j] = 0;
                            }
                        }

                    }
                }
                );
            }

            pctBx_Display.Image = new Bitmap(imgWidth, imgHeight);
            int juliaColor;
            for (int i = 0; i < imgWidth; i++)
            {
                for (int j = 0; j < imgHeight; j++)
                {
                    juliaColor = (int)(sumLattice[i, j] * 255) % 255;
                    ((Bitmap)pctBx_Display.Image).SetPixel(i, j, Color.FromArgb(juliaColor, juliaColor, juliaColor));
                }
            }

            pctBx_Display.Refresh();
            System.Media.SystemSounds.Beep.Play();
        }
        private Complex[] juliaFunctionBoundaryFinderV1(Complex z, int maxIterations)
        {
            Complex[] outputs = new Complex[maxIterations];
            //Complex c = new Complex(0.001, 0);
            //Complex pow = new Complex(1.5, 0);
            Complex c = new Complex(-0.2, 0.7);
            for (int k = 0; k < maxIterations; k++)
            {
                //prevz = new Complex(z.Real, z.Imaginary);
                //z -= (z * z * z - 1) / (3 * z * z);
                //z = (z * z * z + c)/z;
                z = z * z + c;
                //z -= (z * z * z + 0.5 * z - 1.5) * (3 * z * z + 0.5) / ((3 * z * z + 0.5) * (3 * z * z + 0.5) - (3 * z * (z * z * z + 0.5 * z - 1.5)));
                //z = Complex.Pow(z, pow) + c;
                //z = z / (z * z * z * z + 6 * z * z + 1.001);
                //z = (z * z) / (Complex.Pow(z, 9) - z + 0.025);

                if (double.IsNaN(z.Real) || double.IsNaN(z.Imaginary) || double.IsInfinity(z.Real) || double.IsInfinity(z.Imaginary))
                {
                    outputs[k] = outputs[k - 1];
                    for (int kinf = k + 1; kinf < maxIterations; kinf++)
                        outputs[kinf] = outputs[k];
                    return outputs;
                }
                outputs[k] = z;
            }
            return outputs;
        }
        private double computeSumBoundaryFinderV1(Complex z, Complex eta, int maxIterations, double scale, int numPoints)
        {
            Complex nearZ;
            double normSum = 0, maxNormSum = 0;
            Random r = new Random();
            Complex[] juliaZOutputs, juliaZEtaOutputs;
            for (int n = 0; n < numPoints; n++)
            {
                normSum = 0;
                //nearZ = new Complex(z.Real + scale*(r.NextDouble() - 0.5), z.Imaginary + scale * (r.NextDouble() - 0.5));
                nearZ = new Complex(z.Real + scale * Math.Cos(n * 2 * Math.PI / numPoints), z.Imaginary + scale * Math.Sin(n * 2 * Math.PI / numPoints));
                juliaZOutputs = juliaFunctionBoundaryFinderV1(nearZ, maxIterations);
                juliaZEtaOutputs = juliaFunctionBoundaryFinderV1(nearZ + eta, maxIterations);

                for (int k = 0; k < maxIterations; k++)
                {
                    normSum += (juliaZOutputs[k] - juliaZEtaOutputs[k]).Magnitude;
                }
                normSum = (2 / Math.PI) * Math.Atan(normSum);
                if (normSum >= 1)
                    return 1;
                else if (normSum > maxNormSum)
                    maxNormSum = normSum;
            }
            return maxNormSum;
        }
    }
}