using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mandlebrot
{
    public class Slider
    {
        public const int SliderChevSide = 13;
        public const int SliderLinesOffset = SliderChevSide;
        public const int SliderLineHeight = 5;
        public const int TotalHeight = SliderLineHeight + 2 * SliderLinesOffset;
        public const int SliderChevRadius = ((int)SliderChevSide / 2);

        private int Id { get; }
        public int Width { get; }
        public decimal SLoc { get; private set; }
        public int Left
        {
            set
            {
                int newX = value;
                if (newX <= SliderLinesOffset)
                    newX = SliderLinesOffset - SliderChevRadius;
                else if (newX >= Width + SliderLinesOffset - SliderChevRadius - 1)
                    newX = Width + SliderLinesOffset - SliderChevRadius - 1;
                else
                    newX = value - SliderChevRadius;

                chev.Left = newX;

                setSliderLocFromSliderImgLoc();
            }
        }


        private Point loc;
        public PictureBox background;
        public PictureBox chev;
        public TextBox input;
        public Label componentLabel;

        public Slider(int id, Point loc, int sliderWidth, MouseEventHandler background_MouseUp, MouseEventHandler chev_MouseDown, MouseEventHandler chev_MouseMove, MouseEventHandler chev_MouseUp, KeyEventHandler Input_KeyDown, EventHandler Input_LostFocus)
        {
            this.Id = id;
            this.loc = loc;
            this.Width = sliderWidth;

            background = new PictureBox();
            background.Location = this.loc;
            background.Name = id.ToString();
            background.Image = generateBackground();
            background.Size = this.background.Image.Size;
            background.TabIndex = 0;
            background.TabStop = false;
            background.Visible = true;
            background.Enabled = true;
            background.BackColor = Color.Transparent;
            background.MouseUp += background_MouseUp;


            chev = new PictureBox();
            chev.Image = generateChev();
            chev.Location = new Point(-SliderChevRadius + SliderLinesOffset, SliderLinesOffset - SliderChevRadius + 2);
            chev.Name = id.ToString();
            chev.Size = this.chev.Image.Size;
            chev.TabIndex = 0;
            chev.TabStop = false;
            chev.Visible = true;
            chev.Enabled = true;
            chev.BackColor = Color.Transparent;
            chev.MouseDown += new MouseEventHandler(chev_MouseDown);
            chev.MouseMove += new MouseEventHandler(chev_MouseMove);
            chev.MouseUp += new MouseEventHandler(chev_MouseUp);
            background.Controls.Add(this.chev);

            input = new TextBox();
            input.BackColor = System.Drawing.Color.Black;
            input.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            input.ForeColor = System.Drawing.Color.Gray;
            input.Location = new System.Drawing.Point(this.loc.X + this.background.Size.Width + 5, this.loc.Y + 6);
            input.Name = id.ToString();
            input.Size = new System.Drawing.Size(60, 20);
            input.TabIndex = 0;
            input.TabStop = false;
            input.KeyDown += new KeyEventHandler(Input_KeyDown);
            input.LostFocus += new EventHandler(Input_LostFocus);

            componentLabel = new Label();
            componentLabel.BackColor = System.Drawing.Color.Transparent;
            componentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            componentLabel.ForeColor = System.Drawing.Color.LightGray;
            componentLabel.Location = new System.Drawing.Point(this.loc.X - 55, this.loc.Y);
            componentLabel.TextAlign = ContentAlignment.MiddleCenter;
            componentLabel.Name = id.ToString();
            componentLabel.Size = new System.Drawing.Size(55, this.background.Image.Size.Height);
            componentLabel.TabIndex = 0;
            componentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            setSliderLocFromSliderImgLoc();
        }
        private Bitmap generateBackground()
        {
            Bitmap SliderLineImg = new Bitmap(this.Width + 2 * SliderLinesOffset, SliderLineHeight + 2 * SliderLinesOffset);

            for (int x = 0; x < SliderLineImg.Width; x++)
            {
                for (int y = 0; y < SliderLineImg.Height; y++)
                {
                    SliderLineImg.SetPixel(x, y, Color.FromArgb(100, Color.Black));
                }
            }

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < SliderLineHeight; y++)
                {

                    if ((x < 2 || x > this.Width - 3) && (y >= 0 && y <= 4))
                        SliderLineImg.SetPixel(x + SliderLinesOffset, y + SliderLinesOffset, Color.Red);
                    else if (y > 0 && y < 4)
                        SliderLineImg.SetPixel(x + SliderLinesOffset, y + SliderLinesOffset, Color.DarkGray);
                }
            }
            return SliderLineImg;
        }
        private Bitmap generateChev()
        {
            Bitmap SliderChevImg = new Bitmap(SliderChevSide, SliderChevSide);
            int centerX = ((int)SliderChevSide / 2), centerY = ((int)SliderChevSide / 2);


            for (int x = 0; x < SliderChevImg.Width; x++)
                for (int y = 0; y < SliderChevImg.Height; y++)
                {
                    if ((SliderChevRadius - Math.Abs(x - centerX)) >= Math.Abs(y - centerY))
                        SliderChevImg.SetPixel(x, y, Color.FromArgb(64, Color.DarkGray));
                    else
                        SliderChevImg.SetPixel(x, y, Color.Transparent);
                }

            for (int x = 0; x < SliderChevImg.Width; x++)
                for (int y = 0; y < SliderChevImg.Height; y++)
                {
                    if (SliderChevImg.GetPixel(x, y) == Color.FromArgb(64, Color.DarkGray))
                    {
                        if (x > 0 && x < SliderChevImg.Width - 1)
                        {
                            if ((SliderChevImg.GetPixel(x - 1, y) == Color.FromArgb(0, 0xFF, 0xFF, 0xFF)) || (SliderChevImg.GetPixel(x + 1, y) == Color.FromArgb(0, 0xFF, 0xFF, 0xFF)))
                                SliderChevImg.SetPixel(x, y, Color.FromArgb(255, Color.DarkGray));
                        }

                        if (y > 0 && y < SliderChevImg.Height - 1)
                        {
                            if ((SliderChevImg.GetPixel(x, y - 1) == Color.FromArgb(0, 0xFF, 0xFF, 0xFF)) || (SliderChevImg.GetPixel(x, y + 1) == Color.FromArgb(0, 0xFF, 0xFF, 0xFF)))
                                SliderChevImg.SetPixel(x, y, Color.FromArgb(255, Color.DarkGray));
                        }
                    }
                }

            return SliderChevImg;
        }
        private void setSliderLocFromSliderImgLoc()
        {
            SLoc = Math.Round((decimal)(chev.Left - SliderLinesOffset + SliderChevRadius) / (Width - 1), 3);
            input.Text = SLoc.ToString();
        }
        public void setSliderImgLocFromInput()
        {
            decimal tempSliderLoc;
            if (Decimal.TryParse(input.Text, out tempSliderLoc) && tempSliderLoc >= 0m)
            {
                SLoc = tempSliderLoc;
                if (SLoc <= 1)
                    chev.Left = (int)(SLoc * (Width - 1) + SliderLinesOffset - SliderChevRadius);
                else
                    chev.Left = (int)(1 * (Width - 1) + SliderLinesOffset - SliderChevRadius);
            }
        }
        public void setSliderImgLocFromInput(decimal input)
        {
            if (input >= 0m && input <= 1m)
            {
                SLoc = Math.Round(input, 3);
                this.input.Text = SLoc.ToString();
                chev.Left = (int)(SLoc * (Width - 1) + SliderLinesOffset - SliderChevRadius);
            }
        }

    }

}