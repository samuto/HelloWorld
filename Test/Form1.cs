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
        string path = @".";
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string sourcePath = @"C:\Users\ba2676\Desktop\jolicraft_6685327\assets\minecraft\textures";
            string destPath = @"C:\Users\ba2676\Desktop\GitHub\HelloWorld\Test\bin\Debug\..\..\..\HelloWorld\01.Frontend\Textures";
            CopyDir("Blocks", sourcePath, destPath);
            CopyDir("Items", sourcePath, destPath);
        }

        private void CopyDir(string subdir, string sourcePath, string destPath)
        {
            foreach (string file in Directory.GetFiles(Path.Combine(destPath, subdir)))
            {
                string sourceFile = Path.Combine(Path.Combine(sourcePath, subdir), Path.GetFileName(file));
                string destfile = Path.Combine(Path.Combine(destPath, subdir), Path.GetFileName(file));
                try
                {
                    File.Copy(sourceFile, destfile, true);
                }
                catch (FileNotFoundException ex)
                {
                    
                }
            }
        }
    }
}
