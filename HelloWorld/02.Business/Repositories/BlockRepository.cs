using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.CrossCutting.Entities.Blocks;

namespace WindowsFormsApplication7.Business.Repositories
{
    class BlockRepository
    {
        public static Block[] Blocks = new Block[256];

        public static Block Air = new Block(0).Opaque(false);
        public static Block DirtWithGrass = new Block(1).SetDensity(50).TopColor(Color.FromArgb(170, 255, 100));
        public static Block Dirt = new Block(2).SetDensity(50);
        public static Block Stone = new Block(3).SetDensity(100);
        public static Block Wood = new Block(4).SetDensity(75);
        public static Block Leaf = new Block(5).SetDensity(10).BlockColor(Color.DarkGreen);
        public static Block Brick = new Block(6).SetDensity(100);
        public static Block Sand = new Block(7).SetDensity(25);
        public static Block BedRock = new Block(8).SetDensity(int.MaxValue);
        public static Block Diamond = new Block(9).SetDensity(400);
        public static Block Plank = new Block(10).SetDensity(75);
        public static Block CobbleStone = new Block(11).SetDensity(100);
        public static BlockGrass TallGrass = (BlockGrass)new BlockGrass(12).Opaque(false).SetDensity(1).BlockColor(Color.FromArgb(170, 255, 100));
        public static Block FarmlandDry = new Block(13).SetDensity(50);
        public static Block FarmlandWet = new Block(14).SetDensity(50);
        public static BlockWheat Wheat = (BlockWheat)new BlockWheat(15).SetDensity(1).Opaque(false);
       
    }
}
