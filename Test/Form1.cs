using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        string path = @"..\..\..\HelloWorld\01.Frontend\Textures\";
        
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Load(Path.Combine(path, "terrain.png"));

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (Image img = Image.FromFile(Path.Combine(path, "terrain.png")))
            {
                Size outsize = new System.Drawing.Size(64, 64);
                Unstitch(img, 0, 0, outsize, "grass");
                Unstitch(img, 2, 0, outsize, "dirt");
                Unstitch(img, 3, 0, outsize, "grass_side");
                Unstitch(img, 2, 1, outsize, "sand");
                Unstitch(img, 1, 0, outsize, "stone");
                Unstitch(img, 5, 1, outsize, "wood_top");
                Unstitch(img, 4, 1, outsize, "wood_side");
                Unstitch(img, 5, 3, outsize, "leaf");
                Unstitch(img, 7, 0, outsize, "brick");
                Unstitch(img, 1, 1, outsize, "bedrock");
                Unstitch(img, 2, 3, outsize, "diamond");
            }


        }

        private void Unstitch(Image img, int x, int y, Size outsize, string name)
        {
            int tileWidth = img.Size.Width / 16;
            int tileHeight = img.Size.Height / 16;
            Bitmap bmp = new Bitmap(outsize.Width, outsize.Height, img.PixelFormat);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(img, new Rectangle(0, 0, outsize.Width, outsize.Height), new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight), GraphicsUnit.Pixel);
            bmp.Save(Path.Combine(path, "blocks\\"+name+".png"));
        }
    }
}
