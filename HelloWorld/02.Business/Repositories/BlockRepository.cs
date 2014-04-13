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
        public static int id = 0;
        public static Block Air = new Block().ResetOpaque().SetTransparent();
        public static Block Dirt = new Block().SetDensity(50);
        public static Block DirtWithGrass = new Block().SetDensity(50).SetDropId(Dirt.Id).SetTopColor(Color.FromArgb(170, 255, 100));
        public static Block CobbleStone = new Block().SetDensity(100).SetEnergyToTransform(10);
        public static Block Stone = new Block().SetDropId(CobbleStone.Id).SetDensity(100);
        public static Block Wood = (Block)new Block().SetDensity(75).SetHeatOfCombustion(50);
        public static Block Leaf = new Block().SetDensity(10).SetBlockColor(Color.DarkGreen);
        public static Block Brick = new Block().SetDensity(100);
        public static Block Sand = new Block().SetDensity(25);
        public static Block BedRock = new Block().SetDensity(int.MaxValue);
        public static Block Diamond = new Block().SetDensity(400);
        public static Block Plank = new Block().SetDensity(75);
        public static Block TallGrass = new Block().SetDropId(ItemRepository.SeedsWheat.Id, 0.1f).ResetOpaque().SetTransparent().SetDensity(1).SetBlockColor(Color.FromArgb(170, 255, 100));
        public static Block FarmlandDry = new Block().SetDensity(50);
        public static Block FarmlandWet = new Block().SetDensity(50);
        public static BlockWheat Wheat = (BlockWheat)new BlockWheat().SetDensity(1).ResetOpaque().SetTransparent().SetHasStages();
        public static BlockCraftingTable CraftingTable = (BlockCraftingTable)new BlockCraftingTable().SetDensity(75);
        public static BlockFurnace FurnaceOff = (BlockFurnace)new BlockFurnace().SetDensity(100);
        public static BlockFurnace FurnaceOn = (BlockFurnace)new BlockFurnace().SetDensity(100).SetDropId(FurnaceOff.Id);
        public static BlockGlass Glass = (BlockGlass)new BlockGlass().SetTransparent().SetDensity(10);
        public static BlockWater Water = (BlockWater)new BlockWater().SetTransparent().SetDensity(10).ResetOpaque();


        internal static int NextId()
        {
            return id++;
        }
    }
}
