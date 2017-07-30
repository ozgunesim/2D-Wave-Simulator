using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WaterSample
{
    public partial class Form1 : Form
    {
        private List<wave> waves = new List<wave>();
        public static readonly object threadLock = 1;
        private Random rnd = new Random();
        private Bitmap b;
        private Thread th;

        private class wave
        {
            public Rectangle r,rl;
            public int lifeTime;
            public Color color;
        }

        public Form1()
        {
            InitializeComponent();
            th = new Thread(new ThreadStart(makeWave));
            CheckForIllegalCrossThreadCalls = false;
            th.Start();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }
        private enum Sides
        {
            LEFT,
            RIGHT,
            UP,
            DOWN
        }

        public class SideRects
        {
            public Rectangle LeftRect, RightRect, UpRect, DownRect;
            private Rectangle r;
            public SideRects(Rectangle r,Size area)
            {
                LeftRect = new Rectangle(area.Width - (r.Left-area.Width) - r.Width, r.Y, r.Width, r.Height);
                RightRect = new Rectangle(area.Width*3-(r.Left-area.Width)-r.Width,r.Y,r.Width,r.Height);
                UpRect = new Rectangle(r.X, area.Height - (r.Top-area.Height)-r.Height, r.Width, r.Height);
                DownRect = new Rectangle(r.X, area.Height * 3 - (r.Top - area.Height) - r.Height, r.Width, r.Height);
            }
        }




        private void makeWave() 
        {
            while (true)
            {
                lock (threadLock)
                {
                    b = new Bitmap(pictureBox1.Width*3, pictureBox1.Height*3);
                    for (int i = 0; i < waves.Count; i++)
                    {
                        if (waves[i].lifeTime != 0)
                        {
                            Rectangle r = waves[i].r;
                            Graphics g = Graphics.FromImage(b);
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                            SideRects sr = new SideRects(r,pictureBox1.Size);
                            g.DrawEllipse(new Pen(waves[i].color, 5),r);
                            g.DrawEllipse(new Pen(Color.LightSkyBlue, 5), new Rectangle(r.X+5,r.Y+5,r.Width-10,r.Height-10));
                            g.DrawEllipse(new Pen(Color.LightSkyBlue, 5), sr.LeftRect);
                            g.DrawEllipse(new Pen(Color.LightSkyBlue, 5), sr.RightRect);
                            g.DrawEllipse(new Pen(Color.LightSkyBlue, 5), sr.UpRect);
                            g.DrawEllipse(new Pen(Color.LightSkyBlue, 5), sr.DownRect);
                        }
                        else
                        {
                            waves.RemoveAt(i);
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        } 
                    }
                    for (int i = 0; i < waves.Count; i++)
                    {
                        waves[i].lifeTime--;
                        waves[i].r.Width += 2;
                        waves[i].r.Height += 2;
                        waves[i].r.X--;
                        waves[i].r.Y--;

                    }
                    pictureBox1.Refresh();
                }
                Thread.Sleep(10);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            th.Abort();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            lock (threadLock)
            {
                wave w = new wave();

                Rectangle r = new Rectangle();
                r.Location = new Point(pictureBox1.Width + e.X, pictureBox1.Height + e.Y);
                r.Size = new Size(1, 1);
                w.r = r;
                w.lifeTime = 300;
                w.color =  Color.DarkBlue;
                waves.Add(w);
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if(b!=null)
                e.Graphics.DrawImage(b, new Point(-pictureBox1.Width, -pictureBox1.Width));
        }
    }
}
