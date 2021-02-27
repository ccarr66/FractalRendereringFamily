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
        public void renderDisplayImageBoundaryFinderV2()
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
            double desiredScale = 0.001;
            double juliaThreshold = 0.01;
            bool[,] juliaSet = new bool[imgWidth, imgHeight];
            Parallel.For(0, workers, index =>
           // for(int index = 0; index < workers; index++)
                {
                    for (int i = (int)(((double)index / workers) * imgWidth); i < (int)(((double)(index + 1) / workers) * imgWidth); i++)
                    {
                        for (int j = 0; j < imgHeight; j++)
                        {
                            juliaSet[i,j] = juliaPixelBoundaryFinderV2(lattice[i, j], adjPlotScale, desiredScale, maxIterations, juliaThreshold);
                        }
                    }
                }
            );

            pctBx_Display.Image = new Bitmap(imgWidth, imgHeight);
            for (int i = 0; i < imgWidth; i++)
            {
                for (int j = 0; j < imgHeight; j++)
                {
                    if (juliaSet[i, j])
                        ((Bitmap)pctBx_Display.Image).SetPixel(i, j, Color.White);
                    else
                        ((Bitmap)pctBx_Display.Image).SetPixel(i, j, Color.Black);

                }
            }

            pctBx_Display.Refresh();
            System.Media.SystemSounds.Beep.Play();
        }
        bool juliaPixelBoundaryFinderV2(Complex z, double startingScale, double desiredAccuracy, int maxIterations, double juliaThreshold)
        {
            const int numDir = 8;
            for (int dir = 0; dir < numDir; dir++)
            {
                double xdir = Math.Cos(Math.PI / numDir * dir);
                double ydir = Math.Sin(Math.PI / numDir * dir);
                List<Tuple<Complex, double>> PointList = new List<Tuple<Complex, double>>();
                PointList.Add(new Tuple<Complex, double>(z, startingScale));

                Complex z0, z1;
                double currEta, normSum;
                Complex[] juliaZOutputs, juliaZEtaOutputs;
                bool juliaCandidate;
                while (PointList.Count > 0)
                {
                    z0 = PointList[0].Item1;
                    currEta = PointList[0].Item2;
                    PointList.RemoveAt(0);
                    z1 = new Complex(z0.Real + currEta * xdir, z0.Imaginary + currEta * ydir);

                    juliaZOutputs = juliaFunctionBoundaryFinderV2(z, maxIterations);
                    juliaZEtaOutputs = juliaFunctionBoundaryFinderV2(z1, maxIterations);

                    juliaCandidate = false;
                    if ((double.IsInfinity(juliaZOutputs[maxIterations - 1].Real) || double.IsInfinity(juliaZOutputs[maxIterations - 1].Imaginary)) ^ (double.IsInfinity(juliaZEtaOutputs[maxIterations - 1].Real) || double.IsInfinity(juliaZEtaOutputs[maxIterations - 1].Imaginary)))
                        juliaCandidate = false;
                    else if ((double.IsInfinity(juliaZOutputs[maxIterations - 1].Real) || double.IsInfinity(juliaZOutputs[maxIterations - 1].Imaginary)) && (double.IsInfinity(juliaZEtaOutputs[maxIterations - 1].Real) || double.IsInfinity(juliaZEtaOutputs[maxIterations - 1].Imaginary)))
                        juliaCandidate = false;
                    else
                    {
                        normSum = (juliaZOutputs[maxIterations - 1] - juliaZEtaOutputs[maxIterations - 1]).Magnitude;// / juliaZOutputs[maxIterations - 1].Magnitude;

                        normSum = (2 / Math.PI) * Math.Atan(normSum);

                        if (normSum > juliaThreshold)
                            juliaCandidate = true;
                    }

                    if (juliaCandidate)
                    {
                        if (currEta <= desiredAccuracy)
                            return true;

                        currEta = currEta / 2;
                        PointList.Add(new Tuple<Complex, double>(z0, currEta));
                        PointList.Add(new Tuple<Complex, double>(new Complex(z0.Real + currEta * xdir, z0.Imaginary + currEta * ydir), currEta));
                    }

                }
            }

            return false;
        }
        private Complex[] juliaFunctionBoundaryFinderV2(Complex z, int maxIterations)
        {
            Complex[] outputs = new Complex[maxIterations];
            //Complex d = new Complex(-1.05,-.21225);
            //Complex c = new Complex(-0.2, 0);
            //Complex c = new Complex(0.001, 0);
            //Complex c = new Complex(-0.63, -0.407);
            //Complex pow = new Complex(1.5, 0);
            //Complex c1 = new Complex(0.005, -0.005); 
            //Complex c2 = new Complex(0, -4);
            //Complex c3 = new Complex(0, 0.001);
            //Complex c = new Complex(0.5, 0.5);
            //Complex d = new Complex(-0.7, 1);
            //Complex d = new Complex(-0.003, 0.995);
            Complex c = new Complex(-0.2, 0.7);

            for (int k = 0; k < maxIterations; k++)
            {
                //prevz = new Complex(z.Real, z.Imaginary);
                //z -= (z * z * z - 1) / (3 * z * z);
                //z = (z * z * z + c) / (d * z);
                //z = z * z;
                //z -= (z * z * z + 0.5 * z - 1.5) * (3 * z * z + 0.5) / ((3 * z * z + 0.5) * (3 * z * z + 0.5) - (3 * z * (z * z * z + 0.5 * z - 1.5)));
                //z = Complex.Pow(z, pow) + c;
                z = z / (Complex.Log(z*z));
                //z = z * z + c;
                //z = (-4.0004 * z * z * z * z * z + c1) / (c2 * z * z * z * z + c3);
                //z = (2 * c * z * z * z + 2 * z * z) / (z * z * z + 3 * c * z * z - z + c);
                //z = (z * z) / (Complex.Pow(z, 9) + 2 * z + 0.05);
                //z = z / (z * z * z * z + 6 * z * z + 1.001);
                //z = (z * z * z - z) / (d * z * z + 1);
                //z = (z * z) / (Complex.Pow(z, 9) - z + 0.025);



                if (double.IsNaN(z.Real) || double.IsNaN(z.Imaginary) || double.IsInfinity(z.Real) || double.IsInfinity(z.Imaginary))
                {
                    outputs[k] = new Complex(double.PositiveInfinity, double.PositiveInfinity);
                    for (int kinf = k + 1; kinf < maxIterations; kinf++)
                        outputs[kinf] = outputs[k];
                    return outputs;
                }
                outputs[k] = z;
            }
            return outputs;
        }
    }
}