using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bifurcation
{
    public partial class Bifurcation : Form
    {
        private bool mouseDown;
        private Point lastLocation;

        public Bifurcation()
        {
            InitializeComponent();

            updateDisplay();

        }

        private void updateDisplay()
        {
            renderDisplayImage();
        }
        private void renderDisplayImage()
        {
            int imgWidth = pctBx_Display.Width, imgHeight = pctBx_Display.Height;

            double minR = 2, maxR = 4;
            double minY = -0.25, maxY = 1;
            double scaleR = (double)(maxR - minR) / imgWidth, scaleY = (double)(maxY - minY) / imgHeight;

            bool[,] buffer = new bool[imgWidth, imgHeight];

            int numIterations = 1000;

            int iteration, Yres;
            double R, Y;
            for (int r = 0; r < imgWidth; r++)
            {
                R = r * scaleR + minR;
                for (int y = 0; y < imgHeight; y++)
                {
                    iteration = 0;
                    Y = y * scaleY + minY;
                    while (iteration < numIterations)
                    {
                        Y = R * Y*(1 - Y);
                        iteration++;
                    }
                    Yres = (((int)((Y - minY) / scaleY)) % imgHeight > 0) ? ((int)((Y - minY) / scaleY)) % imgHeight : 0; 
                    buffer[r, Yres] = true;
                }
            }

            pctBx_Display.Image = new Bitmap(imgWidth, imgHeight);
            for (int i = 0; i < imgWidth; i++)
            {
                for (int j = 0; j < imgHeight; j++)
                {
                    if (buffer[i, imgHeight - j - 1])
                        ((Bitmap)pctBx_Display.Image).SetPixel(i, j, Color.Black);
                    else
                        ((Bitmap)pctBx_Display.Image).SetPixel(i, j, Color.White);
                }
            }

            pctBx_Display.Refresh();
            System.Media.SystemSounds.Beep.Play();
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
        private void MandlebrotDisp_ResizeEnd(object sender, EventArgs e)
        {
            resizeForm();
        }
        private void resizeForm()
        {
            this.btn_Close.Location = new Point(this.Size.Width - 20, 0);
            this.pctBx_Display.Size = new Size(this.Size.Width - pctBx_Display.Location.X - 12, this.Size.Height - pctBx_Display.Location.Y - 34);
            this.btn_DragRegion.Size = new Size(this.Size.Width - 40, 20);
            this.btn_Maximize.Location = new Point(this.Size.Width - 40, 0);

            updateDisplay();
        }
        private void btn_maximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Maximized)
                this.WindowState = FormWindowState.Maximized;
            else
                this.WindowState = FormWindowState.Normal;

            MandlebrotDisp_ResizeEnd(sender, e);
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
}
