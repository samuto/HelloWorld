using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using WindowsFormsApplication7.CrossCutting.Entities;

namespace WindowsFormsApplication7.Business.Repositories
{
    class BlockRepository
    {
        public static Block[] Blocks = new Block[256];

        public static Block Grass = new Block(1).TopColor(Color.FromArgb(170,255,100));
        public static Block Dirt = new Block(2);
        public static Block Stone = new Block(3);
        public static Block Wood = new Block(4);
        public static Block Leaf = new Block(5).BlockColor(Color.DarkGreen);
        public static Block Brick = new Block(6);
        public static Block Sand = new Block(7);
        public static Block BedRock = new Block(8);
        public static Block Diamond = new Block(9).SetAlpha(0.5f);
        public static Block Plank = new Block(10);
        public static Block CobbleStone = new Block(11);
       
    }
}
