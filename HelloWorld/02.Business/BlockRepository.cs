using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using WindowsFormsApplication7.CrossCutting.Entities;

namespace WindowsFormsApplication7.Business
{
    class BlockRepository
    {
        public static Block[] Blocks = new Block[256];

        public static Block Grass = new Block(1, MaterialEnum.Generic).BlockColor(Color.SaddleBrown).TopColor(Color.YellowGreen).AddToRepository();
        public static Block Dirt = new Block(2, MaterialEnum.Generic).BlockColor(Color.SaddleBrown).AddToRepository();
        public static Block Stone = new Block(3, MaterialEnum.Generic).BlockColor(Color.Gray).AddToRepository();
        public static Block Wood = new Block(4, MaterialEnum.Generic).BlockColor(Color.Orange).TopBottomColor(Color.Yellow).AddToRepository();
        public static Block Leaf = new Block(5, MaterialEnum.Generic).BlockColor(Color.DarkGreen).AddToRepository();
        public static Block Brick = new Block(6, MaterialEnum.Generic).BlockColor(Color.Khaki).AddToRepository();
        public static Block Roof = new Block(7, MaterialEnum.Generic).BlockColor(Color.DarkOliveGreen).AddToRepository();
        public static Block Sand = new Block(8, MaterialEnum.Generic).BlockColor(Color.Yellow).AddToRepository();
        public static Block Water = new Block(9, MaterialEnum.Generic).BlockColor(Color.LightBlue).AddToRepository();
        public static Block BedRock = new Block(10, MaterialEnum.Generic).BlockColor(Color.DarkSlateGray).AddToRepository();
       
    }
}
