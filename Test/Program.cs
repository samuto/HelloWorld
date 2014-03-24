using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Test();



            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static void Test()
        {
            int radius = 3;
            int x, y, z;
            x = 0;
            y = 0;
            z = 0;
            RenderChunk(x, y, z);
            for (int r = 1; r <= radius; r++)
            {
                // front+back
                for (int i = -r; i <= r; i++)
                {
                    for (int j = -r; j <= r; j++)
                    {
                        RenderChunk(x + i, y + j, z + r);
                        RenderChunk(x + i, y + j, z - r);
                    }
                }
                // left+right
                for (int i = -r; i <= r; i++)
                {
                    for (int j = -r + 1; j <= r - 1; j++)
                    {
                        RenderChunk(x - r, y + i, z + j);
                        RenderChunk(x + r, y + i, z + j);
                    }
                }
                // top+bottom
                for (int i = -r + 1; i <= r - 1; i++)
                {
                    for (int j = -r + 1; j <= r - 1; j++)
                    {
                        RenderChunk(x + i, y - r, z + j);
                        RenderChunk(x + i, y + r, z + j);
                    }
                }
            }
        }

        static int calls = 0;

        private static void RenderChunk(int x, int y, int z)
        {
            Console.WriteLine(string.Format("{0},{1},{2}", x, y, z));
            calls++;
        }
    }
}
