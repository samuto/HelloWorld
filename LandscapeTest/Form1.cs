using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LandscapeTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            Bitmap bmp = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            int maxHeight = 100;
            int waterLevel = 18;
            int sandLevel = waterLevel + 2;
            int grassLevel = waterLevel + 20;
            float zMax = 250f;
            for (int z = 0; z < zMax; z++)
            {
                for (int x = 0; x < 600; x++)
                {
                    float noise1 = GetNoise(x, 0, z, 0.01f);
                    noise1 = Tarracing(noise1, 0.2f, 0.4f);
                    noise1 = Tarracing(noise1, 0.5f, 0.6f);
                    noise1 = Tarracing(noise1, 0.7f, 0.9f);
                    noise1 = noise1 * maxHeight / 2f;
                    noise1 += GetNoise(x, 0, z, 0.1f) * maxHeight / 40f;
                    noise1 += GetNoise(x, 0, z, 0.05f) * maxHeight / 20f;
                    noise1 += GetNoise(x, 0, z, 0.025f) * maxHeight / 10f;

                    for (int y = 0; y < maxHeight; y++)
                    {
                        int px = z + x;
                        int py = maxHeight - y + z;
                        float colorScale = (z + y) / (zMax + maxHeight/4f);
                        if (y <= noise1)
                        {
                            Color color = ColorMult(Color.Gray, colorScale);
                            if (y + 1 > noise1)
                            {
                                if(y<=sandLevel)
                                    color = ColorMult(Color.Yellow, colorScale);
                                else  if(y<=grassLevel)
                                    color = ColorMult(Color.Green, colorScale);
                                else
                                    color = ColorMult(Color.White, colorScale);
                            }
                            bmp.SetPixel(px, py, color);
                        }
                        else if (y <= waterLevel)
                        {
                            Color color = ColorMult(Color.FromArgb(40, 40, 190), colorScale);
                            if (y + 1 > waterLevel)
                                color = ColorMult(Color.FromArgb(80, 80, 200), colorScale);
                            bmp.SetPixel(px, py, color);
                        }
                    }
                }
            }
            pictureBox1.Image = bmp;
        }

        private float GetNoise(float xd, float yd, float zd, float scale)
        {
            return (Noise.Generate(xd * scale, 0, zd * scale) + 1f) / 2f;
        }

        private float Tarracing(float noise1, float height1, float height2)
        {
            if (noise1 > height1)
            {
                float diff1 = (noise1 - height1);
                if (diff1 > (height2 - height1))
                    diff1 = (height2 - height1);
                noise1 -= diff1;
            }
            return noise1;
        }

        private Color ColorMult(Color color, float factor)
        {
            factor = factor * 0.8f;
            factor += 0.2f;
            if (factor > 1f)
                factor = 1f;

            return Color.FromArgb((int)(color.R * factor), (int)(color.G * factor), (int)(color.B * factor));
        }


    }
}
