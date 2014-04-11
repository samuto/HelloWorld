using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.CrossCutting.Entities.Items;

namespace WindowsFormsApplication7.Business.Repositories
{
    class ItemRepository
    {
        public const int ItemIdOffset = 256;
        public static Item[] Items = new Item[1024];
        public static Item Stick = new Item(0, "Stick");
        public static Item StoneAxe = new Item(1, "Axe");
        public static ItemHoe StoneHoe = new ItemHoe(2, "Hoe");
        public static Item StonePickAxe = new Item(3, "Pick Axe");
        public static Item StoneShovel = new Item(4, "Shovel");
        public static Item StoneSword = new Item(5, "Sword");
        public static ItemSeeds SeedsWheat = (ItemSeeds)new ItemSeeds(6, "Wheat Seeds").SetConsumable();
        public static Item Wheat = new Item(7, "Wheat");
        public static ItemFood Bread = (ItemFood)new ItemFood(8, "Bread").SetHungerDecrease(10).SetConsumable();
        public static Item Coal = (Item)new Item(9, "Coal").SetHeatOfCombustion(50);
    }
}
