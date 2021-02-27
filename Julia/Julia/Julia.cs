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
        private bool mouseDown;
        private Point lastLocation;

        private PointD Plot_CenterCoord;
        private const double InitialX = 0d;
        private const double InitialY = 0d;
        private const double InitialPlotWidth = 7;
        private const double MaxX = 20;
        private const double MinX = -20;
        private const double MaxY = 20;
        private const double MinY = -20;
        private double MaxPlotScale;     //unit/pix on either axis
        private double PlotScale;
        private double MaxScale = 1d;
        private int MaxIterationDepth = 1000000;
        private int MinIterationDepth = 0;


        public Julia()
        {
            InitializeComponent();

            Plot_CenterCoord = new PointD(InitialX, InitialY);
            MaxPlotScale = (InitialPlotWidth / pctBx_Display.Size.Width);
            PlotScale = MaxScale;

            PaletteManager.initializePaletteManager();
            updateDisplay();

        }

        enum modes { BasinFinder = 0, Cauchy, Trace, BoundaryFinderV1, BoundaryFinderV2, EdgeDetection, Hist }

        modes currentMode = modes.BasinFinder;
        private void updateDisplay()
        {
            switch (currentMode)
            {
                case modes.BasinFinder:
                    {
                        renderDisplayImageBasinFinder();
                        break;
                    }
                case modes.Cauchy:
                    {
                        renderDisplayImageCAUCHYV1();
                        break;
                    }
                case modes.Trace:
                    {
                        renderDisplayImageTRACE();
                        break;
                    }
                case modes.BoundaryFinderV1:
                    {
                        renderDisplayImageBoundaryFinderV1();
                        break;
                    }
                case modes.BoundaryFinderV2:
                    {
                        renderDisplayImageBoundaryFinderV2();
                        break;
                    }
                case modes.EdgeDetection:
                    {
                        renderDisplayImageEdgeDetection();
                        break;
                    }
                case modes.Hist:
                    {
                        renderDisplayImageHist();
                        break;
                    }
                default:
                    {
                        renderDisplayImageBasinFinder();
                        break;
                    }
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int RESIZE_HANDLE_SIZE = 10;

            switch (m.Msg)
            {
                case 0x0084/*NCHITTEST*/ :
                    base.WndProc(ref m);

                    if ((int)m.Result == 0x01/*HTCLIENT*/)
                    {
                        Point screenPoint = new Point(m.LParam.ToInt32());
                        Point clientPoint = this.PointToClient(screenPoint);
                        if (clientPoint.Y <= RESIZE_HANDLE_SIZE)
                        {
                            /*if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)13;//HTTOPLEFT
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)12;//HTTOP
                            else
                                m.Result = (IntPtr)14;//HTTOPRIGHT*/
                        }
                        else if (clientPoint.Y <= (Size.Height - RESIZE_HANDLE_SIZE))
                        {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)10/*HTLEFT*/ ;
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)2/*HTCAPTION*/ ;
                            else
                                m.Result = (IntPtr)11/*HTRIGHT*/ ;
                        }
                        else
                        {
                            if (clientPoint.X <= RESIZE_HANDLE_SIZE)
                                m.Result = (IntPtr)16/*HTBOTTOMLEFT*/ ;
                            else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
                                m.Result = (IntPtr)15/*HTBOTTOM*/ ;
                            else
                                m.Result = (IntPtr)17/*HTBOTTOMRIGHT*/ ;
                        }
                    }
                    return;
            }
            base.WndProc(ref m);
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x20000; // <--- use 0x20000
                return cp;
            }
        }
        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btn_DragRegion_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }
        private void btn_DragRegion_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }
        private void btn_DragRegion_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
        private void pctBx_Display_DoubleClick(object sender, EventArgs e)
        {
            Point formScreenCoordLoc = pctBx_Display.PointToScreen(new Point(0, 0));
            Point convertedMouseCoords = new Point(MousePosition.X - formScreenCoordLoc.X, MousePosition.Y - formScreenCoordLoc.Y);

            if (convertedMouseCoords.X > 0 && convertedMouseCoords.Y > 0 && convertedMouseCoords.X < pctBx_Display.Width && convertedMouseCoords.Y < pctBx_Display.Height)
            {
                centerPlot(convertedMouseCoords.X, convertedMouseCoords.Y);
                updateDisplay();
            }
        }
        private void centerPlot(int x, int y)
        {
            double newX = ((x - 0.5 * pctBx_Display.Width) * (PlotScale * MaxPlotScale)) + Plot_CenterCoord.X;
            double newY = ((y - 0.5 * pctBx_Display.Height) * (PlotScale * MaxPlotScale)) + Plot_CenterCoord.Y;
            if (newX < MaxX && newX > MinX && newY < MaxY && newY > MinY)
                Plot_CenterCoord = new PointD(newX, newY);
        }
        private void JuliaDisp_ResizeEnd(object sender, EventArgs e)
        {
            resizeForm();
        }
        private void resizeForm()
        {
            this.btn_Close.Location = new Point(this.Size.Width - 20, 0);
            this.pctBx_Display.Size = new Size(this.Size.Width - pctBx_Display.Location.X - 12, this.Size.Height - pctBx_Display.Location.Y - 34);
            this.btn_DragRegion.Size = new Size(this.Size.Width - 40, 20);
            this.btn_Maximize.Location = new Point(this.Size.Width - 40, 0);

            MaxPlotScale = (InitialPlotWidth / pctBx_Display.Size.Width);

            updateDisplay();
        }
        private void btn_maximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Maximized)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;

            JuliaDisp_ResizeEnd(sender, e);
        }
        public static Color HSLtoRGBConversion(decimal hue, decimal saturation, decimal lightness)
        {
            if (!checkIfValidHSL(hue, saturation, lightness))
                return Color.White;

            decimal chroma = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            decimal hPrime = hue / 60;
            decimal interColor = chroma * (1 - Math.Abs(hPrime % 2 - 1));
            decimal red1 = 0, green1 = 0, blue1 = 0;

            if (0 <= hPrime && hPrime <= 1) { red1 = chroma; green1 = interColor; blue1 = 0; }
            else if (1 < hPrime && hPrime <= 2) { red1 = interColor; green1 = chroma; blue1 = 0; }
            else if (2 < hPrime && hPrime <= 3) { red1 = 0; green1 = chroma; blue1 = interColor; }
            else if (3 < hPrime && hPrime <= 4) { red1 = 0; green1 = interColor; blue1 = chroma; }
            else if (4 < hPrime && hPrime <= 5) { red1 = interColor; green1 = 0; blue1 = chroma; }
            else if (5 < hPrime && hPrime <= 6) { red1 = chroma; green1 = 0; blue1 = interColor; }

            decimal m = lightness - chroma / 2;

            int red = (int)((red1 + m) * 255);
            int green = (int)((green1 + m) * 255);
            int blue = (int)((blue1 + m) * 255);

            if (checkIfValidRGB(red, green, blue))
                return Color.FromArgb(red, green, blue);
            else
                return Color.White;
        }
        public static bool checkIfValidHSL(decimal hue, decimal saturation, decimal lightness)
        {
            if (!(hue >= 0 && hue <= 360) || !(saturation >= 0 && saturation <= 1) || !(lightness >= 0 && lightness <= 1))
                return false;
            return true;
        }
        public static Color HSVtoRGBConversion(decimal hue, decimal saturation, decimal value)
        {
            if (!checkIfValidHSV(hue, saturation, value))
                return Color.White;

            decimal chroma = value * saturation;
            decimal hPrime = hue / 60;
            decimal interColor = chroma * (1 - Math.Abs(hPrime % 2 - 1));
            decimal red1 = 0, green1 = 0, blue1 = 0;

            if (0 <= hPrime && hPrime <= 1) { red1 = chroma; green1 = interColor; blue1 = 0; }
            else if (1 < hPrime && hPrime <= 2) { red1 = interColor; green1 = chroma; blue1 = 0; }
            else if (2 < hPrime && hPrime <= 3) { red1 = 0; green1 = chroma; blue1 = interColor; }
            else if (3 < hPrime && hPrime <= 4) { red1 = 0; green1 = interColor; blue1 = chroma; }
            else if (4 < hPrime && hPrime <= 5) { red1 = interColor; green1 = 0; blue1 = chroma; }
            else if (5 < hPrime && hPrime <= 6) { red1 = chroma; green1 = 0; blue1 = interColor; }

            decimal m = value - chroma;

            int red = (int)((red1 + m) * 255);
            int green = (int)((green1 + m) * 255);
            int blue = (int)((blue1 + m) * 255);

            if (checkIfValidRGB(red, green, blue))
                return Color.FromArgb(red, green, blue);
            else
                return Color.White;
        }
        public static bool checkIfValidHSV(decimal hue, decimal saturation, decimal lightness)
        {
            if (!(hue >= 0 && hue <= 360) || !(saturation >= 0 && saturation <= 1) || !(lightness >= 0 && lightness <= 1))
                return false;
            return true;
        }
        public static bool checkIfValidRGB(int red, int green, int blue)
        {
            if (!(red >= 0 && red <= 0xFF) || !(green >= 0 && green <= 0xFF) || !(blue >= 0 && blue <= 0xFF))
                return false;
            return true;
        }
    }

    public class PointD
    {
        private double x, y;

        public double X
        {
            get { return x; }
            set { x = value; }
        }
        public double Y
        {
            get { return y; }
            set { x = value; }
        }

        public PointD(double x, double y)
        {
            this.x = x; this.y = y;
        }
    }

    public class PaletteManager : Julia
    {
        public const int InitialPaletteLength = 1000;
        public const decimal InitialPaletteOffset = 0;
        public const int InitialPalette = 7;
        public const bool InitialCyclic = true;
        public const bool InitialRandom = false;
        public const int InitialIterationLim = 1000;

        /////////////////////////////////////////////////////////////////////////////////////////////////
        public static Color[] CContainer { get; private set; }
        public static Color ColorContainer(int i)
        {
            return CContainer[i % CContainer.Length];
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////
        public static int PaletteLength { get; set; }
        /////////////////////////////////////////////////////////////////////////////////////////////////
        private static decimal offset;
        public static decimal Offset { get { return offset; } set { offset = value % 1; } }
        /////////////////////////////////////////////////////////////////////////////////////////////////
        private static List<Palette> availablePalettes;
        public static Palette AvailablePalettes(int i)
        {
            return availablePalettes[i % availablePalettes.Count];
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////
        public static int NumberOfPalettes
        {
            get { return availablePalettes.Count; }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////
        public static Palette CurrentPalette { get; private set; }
        /////////////////////////////////////////////////////////////////////////////////////////////////
        private static int indCurrentPalette;
        public static int IndCurrentPalette
        {
            get { return indCurrentPalette; }
            set { indCurrentPalette = value % availablePalettes.Count; setCurrentPaletteCopy(); }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool Cyclic { get; set; }
        public static bool RandomColors { get; set; }
        /////////////////////////////////////////////////////////////////////////////////////////////////
        public static uint IterationLim { get; set; }


        public static void initializePaletteManager()
        {

            Palette Spectrum = new Palette(
                                            "Spectrum",
                                            new List<PaletteColor>() {
                                                    new PaletteColor(0, 0, 1, .5m),
                                                    new PaletteColor(1, 360, 1, .5m)
                                            },
                                            true
                                        );

            Palette Earth = new Palette(
                                            "Earth",
                                            new List<PaletteColor>() {
                                                    new PaletteColor(0, 0xFF, 0xFF, 0xFF),
                                                    new PaletteColor(0.5m, 0xFF, 0xFF*0.8m, 0),
                                                    new PaletteColor(1, 0xFF*0.53m, 0xFF*0.12m, 0xFF*0.075m)
                                            },
                                            false
                                        );

            Palette EarthAndSky = new Palette(
                                            "EarthAndSky",
                                            new List<PaletteColor>() {
                                                    new PaletteColor(0, 0xFF, 0xFF, 0xFF),
                                                    new PaletteColor(.17m, 0xFF, 0xFF*0.8m, 0),
                                                    new PaletteColor(.34m, 0xFF*0.53m, 0xFF*0.12m, 0xFF*0.075m),
                                                    new PaletteColor(.66m, 0, 0, 0xFF*0.6m),
                                                    new PaletteColor(.83m, 0, 0xFF*0.4m, 0xFF),
                                                    new PaletteColor(1, 0xFF, 0xFF, 0xFF)
                                            },
                                            false
                                        );

            Palette HotAndCold = new Palette(
                                            "HotAndCold",
                                            new List<PaletteColor>() {
                                                    new PaletteColor(0, 0xFF, 0xFF, 0xFF),
                                                    new PaletteColor(.17m, 0, 0xFF*0.4m, 0xFF),
                                                    new PaletteColor(.5m, 0xFF*0.2m, 0xFF*0.2m, 0xFF*0.2m),
                                                    new PaletteColor(.83m, 0xFF, 0, 0xFF*0.8m),
                                                    new PaletteColor(1, 0xFF, 0xFF, 0xFF)
                                            },
                                            false
                                        );

            Palette Forest = new Palette(
                                            "Forest",
                                            new List<PaletteColor>() {
                                                    new PaletteColor(0, 46, .43m, .21m),
                                                    new PaletteColor(.33m, 32, .26m, .54m),
                                                    new PaletteColor(.66m, 223, .7m, .24m),
                                                    new PaletteColor(1, 46, .43m, .21m)
                                            },
                                            true
                                        );

            Palette Seashore = new Palette(
                                            "SeaShore",
                                            new List<PaletteColor>() {
                                                    new PaletteColor(0, 0xFF*0.791m, 0xFF*0.996m, 0xFF*0.763m),
                                                    new PaletteColor(.17m, 0xFF*0.897m, 0xFF*0.895m, 0xFF*0.656m),
                                                    new PaletteColor(.34m, 0xFF*0.947m, 0xFF*0.316m, 0xFF*0.127m),
                                                    new PaletteColor(.5m, 0xFF*0.518m, 0xFF*0.111m, 0xFF*0.0917m),
                                                    new PaletteColor(.66m, 0xFF*0.0198m, 0xFF*0.456m, 0xFF*0.684m),
                                                    new PaletteColor(.83m, 0xFF*0.538m, 0xFF*0.826m, 0xFF*0.818m),
                                                    new PaletteColor(1, 0xFF*0.791m, 0xFF*0.996m, 0xFF*0.763m)
                                            },
                                            false
                                        );

            Palette Fire = new Palette(
                                            "Fire",
                                            new List<PaletteColor>() {
                                                    new PaletteColor(0, 0, 0, 0),
                                                    new PaletteColor(.15m, 0xFF, 0, 0),
                                                    new PaletteColor(.85m, 0xFF, 0xFF, 0),
                                                    new PaletteColor(1, 0xFF, 0xFF, 0xFF)
                                            },
                                            false
                                        );

            Palette CyclicFire = new Palette(
                                            "CyclicFire",
                                            new List<PaletteColor>() {
                                                    new PaletteColor(0, 0, 0, 0),
                                                    new PaletteColor(.2m, 0xFF, 0, 0),
                                                    new PaletteColor(.4m, 0xFF, 0xFF, 0),
                                                    new PaletteColor(.5m, 0xFF, 0xFF, 0xFF),
                                                    new PaletteColor(.6m, 0xFF, 0xFF, 0),
                                                    new PaletteColor(.8m, 0xFF, 0, 0),
                                                    new PaletteColor(1, 0, 0, 0)
                                            },
                                            false
                                        );

            Palette RedCyan = new Palette(
                                            "RedCyan",
                                            new List<PaletteColor>() {
                                                    new PaletteColor(0, 0xFF, 0, 0),
                                                    new PaletteColor(0.5m, 0, 0xFF, 0xFF),
                                                    new PaletteColor(1, 0xFF, 0, 0),
                                            },
                                            false
                                        );

            Palette BlueGold = new Palette(
                                            "BlueGold",
                                            new List<PaletteColor>() {
                                                    new PaletteColor(0, 0xFF * 0.1m, 0xFF * 0.1m, 0xFF),
                                                    new PaletteColor(0.5m, 0xFF, 0xFF*0.6m, 0),
                                                    new PaletteColor(1, 0xFF * 0.1m, 0xFF * 0.1m, 0xFF),
                                            },
                                            false
                                        );

            Palette GrayScale = new Palette(
                                            "GrayScale",
                                            new List<PaletteColor>() {
                                                    new PaletteColor(1, 0xFF, 0xFF, 0xFF),
                                                    new PaletteColor(0, 0, 0, 0)
                                            },
                                            false
                                        );

            Palette CyclicGrayScale = new Palette(
                                            "CyclicGrayScale",
                                            new List<PaletteColor>() {
                                                    new PaletteColor(0, 0, 0, 0),
                                                    new PaletteColor(0.5m, 0xFF, 0xFF, 0xFF),
                                                    new PaletteColor(1, 0, 0, 0),
                                            },
                                            false
                                        );

            Random r = new Random();
            Palette Random = new Palette(
                                            "Random",
                                            new List<PaletteColor>() {
                                                    new PaletteColor(0, r.Next() % 255, r.Next() % 255, r.Next() % 255),
                                                    new PaletteColor(.17m, r.Next() % 255, r.Next() % 255, r.Next() % 255),
                                                    new PaletteColor(.34m, r.Next() % 255, r.Next() % 255, r.Next() % 255),
                                                    new PaletteColor(.5m, r.Next() % 255, r.Next() % 255, r.Next() % 255),
                                                    new PaletteColor(.66m, r.Next() % 255, r.Next() % 255, r.Next() % 255),
                                                    new PaletteColor(.83m, r.Next() % 255, r.Next() % 255, r.Next() % 255),
                                                    new PaletteColor(1, r.Next() % 255, r.Next() % 255, r.Next() % 255)
                                            },
                                            false
                                        );

            availablePalettes = new List<Palette>() { Spectrum, Earth, EarthAndSky, HotAndCold, Forest, Seashore, Fire, CyclicFire, RedCyan, BlueGold, GrayScale, CyclicGrayScale, Random };


            PaletteLength = InitialPaletteLength;
            Offset = InitialPaletteOffset;
            IndCurrentPalette = InitialPalette;
            Cyclic = InitialCyclic;
            RandomColors = InitialRandom;
            IterationLim = InitialIterationLim;

            colorSetup();
        }

        public static void colorSetup()
        {
            CContainer = new Color[IterationLim];

            if (!RandomColors)
            {

                int direction = 1; decimal locInPalette;
                for (int i = 0; i < IterationLim; i++)
                {
                    locInPalette = ((decimal)((i + Offset * PaletteLength) % PaletteLength)) / PaletteLength;
                    locInPalette = (direction == -1) ? 1 - locInPalette : locInPalette;
                    CContainer[i] = PaletteManager.CurrentPalette.getColor(locInPalette);

                    if (((i + 1 + (int)(Offset * PaletteLength)) % PaletteLength) == 0 && Cyclic)
                        direction *= -1;
                }
            }
            else
            {
                Random r = new Random();
                for (int i = 0; i < IterationLim; i++)
                {
                    CContainer[i] = Color.FromArgb((int)(0xFF000000 | r.Next()));
                }
            }

        }

        public static void setCurrentPaletteCopy()
        {
            CurrentPalette = new Palette(availablePalettes[IndCurrentPalette]);
        }

        public static void saveToCustomPalette()
        {
            if (AvailablePalettes(availablePalettes.Count - 1).Name != "Custom")
            {
                availablePalettes.Add(new Palette(CurrentPalette));
                IndCurrentPalette = availablePalettes.Count - 1;
                AvailablePalettes(IndCurrentPalette).Name = "Custom";
            }
            else
            {
                availablePalettes.RemoveAt(availablePalettes.Count - 1);
                availablePalettes.Add(new Palette(CurrentPalette));
                IndCurrentPalette = availablePalettes.Count - 1;
                AvailablePalettes(IndCurrentPalette).Name = "Custom";
            }
            //availablePalettes[indCurrentPalette] = new Palette(CurrentPalette);
        }

        public static void saveToNewPalette(string name, List<PaletteColor> clrs, bool hsl)
        {
            availablePalettes.Add(new Palette(name, clrs, hsl));
            IndCurrentPalette = availablePalettes.Count - 1;
        }
    }


    public class Palette
    {
        public string Name { get; set; }
        public bool HSL { get; private set; }
        private List<PaletteColor> colors;
        public PaletteColor this[int i]
        {
            get { return this.colors[i]; }
            set { this.colors[i] = value; }
        }
        public int Count
        {
            get { return colors.Count; }
        }

        public Palette(string name, List<PaletteColor> clrs, bool hsl)
        {
            this.Name = name;
            this.colors = new List<PaletteColor>();
            this.HSL = hsl;

            PaletteColor defaultStartColor, defaultEndColor;

            if (this.HSL)
            {
                defaultStartColor = new PaletteColor(0m, 0, 1, 0.5m);
                defaultEndColor = new PaletteColor(1m, 360, 1, 0.5m);
            }
            else
            {
                defaultStartColor = new PaletteColor(0m, 0xFF, 0xFF, 0xFF);
                defaultEndColor = new PaletteColor(1m, 0, 0, 0);
            }

            if (clrs.Count == 0)
            {
                this.colors.Add(defaultStartColor);
                this.colors.Add(defaultEndColor);
            }
            else if (clrs.Count == 1)
            {
                this.colors.Add(clrs[0]);
                this.colors[0].Location = 0;

                this.colors.Add(defaultEndColor);
            }
            else
            {
                decimal prevLoc = -0.05m;
                foreach (PaletteColor pc in clrs)
                {
                    if (!(pc.Location - prevLoc < 0.05m || pc.Location > 1m))
                        if ((hsl && checkIfValidHSL(pc.Component1, pc.Component2, pc.Component3))
                        || (!hsl && checkIfValidRGB((int)pc.Component1, (int)pc.Component2, (int)pc.Component3)))
                            this.colors.Add(pc);
                }

                if (this.colors.Count == 0)
                {
                    this.colors.Add(defaultStartColor);
                    this.colors.Add(defaultEndColor);
                }
                else if (this.colors.Count == 1)
                {
                    this.colors.Add(defaultEndColor);
                }

                this.colors[0].Location = 0;
                this.colors[this.colors.Count - 1].Location = 1;
            }

        }

        public Palette(Palette p)
        {
            this.HSL = p.HSL;
            this.colors = new List<PaletteColor>();
            p.colors.ForEach((item) =>
            {
                this.colors.Add(new PaletteColor(item));
            });
        }

        public void Insert(int index, PaletteColor pc)
        {
            colors.Insert(index, pc);
        }

        public Color getColor(decimal locInPalette)
        {
            if (locInPalette > 1)
                return Color.White;

            int i;
            for (i = 0; i < colors.Count; i++)
                if (colors[i].Location > locInPalette)
                    break;
            if (i == colors.Count) i--;

            int startingColor = i - 1;
            int endingColor = i;
            PaletteColor sColor = colors[startingColor];
            PaletteColor eColor = colors[endingColor];
            decimal percentLocBetweenColors = ((locInPalette - sColor.Location) / (eColor.Location - sColor.Location));
            percentLocBetweenColors = (percentLocBetweenColors > 1) ? 1 : percentLocBetweenColors;

            decimal c1diff = eColor.Component1 - sColor.Component1;
            decimal c2diff = eColor.Component2 - sColor.Component2;
            decimal c3diff = eColor.Component3 - sColor.Component3;

            decimal newComponent1 = c1diff * percentLocBetweenColors + sColor.Component1;
            decimal newComponent2 = c2diff * percentLocBetweenColors + sColor.Component2;
            decimal newComponent3 = c3diff * percentLocBetweenColors + sColor.Component3;

            if (this.HSL)
                return HSLtoRGBConversion(newComponent1, newComponent2, newComponent3);
            else
                return Color.FromArgb((int)newComponent1, (int)newComponent2, (int)newComponent3);
        }

        public void removeColor(int ind)
        {
            colors.Remove(colors[ind % colors.Count]);
        }

        public static Color HSLtoRGBConversion(decimal hue, decimal saturation, decimal lightness)
        {
            if (!checkIfValidHSL(hue, saturation, lightness))
                return Color.White;

            decimal chroma = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            decimal hPrime = hue / 60;
            decimal interColor = chroma * (1 - Math.Abs(hPrime % 2 - 1));
            decimal red1 = 0, green1 = 0, blue1 = 0;

            if (0 <= hPrime && hPrime <= 1) { red1 = chroma; green1 = interColor; blue1 = 0; }
            else if (1 < hPrime && hPrime <= 2) { red1 = interColor; green1 = chroma; blue1 = 0; }
            else if (2 < hPrime && hPrime <= 3) { red1 = 0; green1 = chroma; blue1 = interColor; }
            else if (3 < hPrime && hPrime <= 4) { red1 = 0; green1 = interColor; blue1 = chroma; }
            else if (4 < hPrime && hPrime <= 5) { red1 = interColor; green1 = 0; blue1 = chroma; }
            else if (5 < hPrime && hPrime <= 6) { red1 = chroma; green1 = 0; blue1 = interColor; }

            decimal m = lightness - chroma / 2;

            int red = (int)((red1 + m) * 255);
            int green = (int)((green1 + m) * 255);
            int blue = (int)((blue1 + m) * 255);

            if (checkIfValidRGB(red, green, blue))
                return Color.FromArgb(red, green, blue);
            else
                return Color.White;
        }

        public static bool checkIfValidHSL(decimal hue, decimal saturation, decimal lightness)
        {
            if (!(hue >= 0 && hue <= 360) || !(saturation >= 0 && saturation <= 1) || !(lightness >= 0 && lightness <= 1))
                return false;
            return true;
        }

        public static bool checkIfValidRGB(int red, int green, int blue)
        {
            if (!(red >= 0 && red <= 0xFF) || !(green >= 0 && green <= 0xFF) || !(blue >= 0 && blue <= 0xFF))
                return false;
            return true;
        }
    }

    public class PaletteColor
    {
        private decimal location;
        public decimal Location
        {
            get { return this.location; }
            set
            {

                if (value >= 0 && value <= 1)
                    this.location = value;
                else
                    this.location = 0;
            }
        }
        public decimal Component1 { get; set; }
        public decimal Component2 { get; set; }
        public decimal Component3 { get; set; }

        public PaletteColor(decimal loc, decimal c1, decimal c2, decimal c3)
        {
            this.Location = Math.Round(loc, 3);
            this.Component1 = c1;
            this.Component2 = c2;
            this.Component3 = c3;
        }
        public PaletteColor(PaletteColor pc)
        {
            this.Location = pc.Location;
            this.Component1 = pc.Component1;
            this.Component2 = pc.Component2;
            this.Component3 = pc.Component3;
        }
        public void convertToHSL()
        {
            decimal red = Component1 / 255;
            decimal green = Component2 / 255;
            decimal blue = Component3 / 255;

            decimal max = (red > green) ? red : green;
            max = (max > blue) ? max : blue;

            decimal min = (red < green) ? red : green;
            min = (min < blue) ? min : blue;

            decimal hue = 0;
            if (max == min)
                hue = 0;
            else if (max == red)
                hue = 60 * (0 + (green - blue) / (max - min));
            else if (max == green)
                hue = 60 * (2 + (blue - red) / (max - min));
            else if (max == blue)
                hue = 60 * (4 + (red - green) / (max - min));
            hue = (hue < 0) ? hue + 360 : hue;
            Component1 = hue;

            decimal saturation = 0;
            if (max == 0 || min == 1)
                saturation = 0;
            else
                saturation = (max - min) / (1 - Math.Abs(max + min - 1));
            Component2 = saturation;

            decimal light = (max + min) / 2;
            Component3 = light;
        }
    }
}
