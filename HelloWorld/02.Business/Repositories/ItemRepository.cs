using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.Business.Repositories
{
    class ItemRepository
    {
        public const int ItemIdOffset = 256;
        public static Item[] Items = new Item[1024];
        public static Item Stick = new Item(0, "Stick");
        public static Item StoneAxe = new Item(1, "Axe");
        public static Item StoneHoe = new Item(2, "Hoe");
        public static Item StonePickAxe = new Item(3, "Pick Axe");
        public static Item StoneShovel = new Item(4, "Shovel");
        public static Item StoneSword = new Item(5, "Sword");
    }
}
