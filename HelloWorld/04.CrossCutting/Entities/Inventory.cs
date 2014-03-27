using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Inventory
    {
        public Slot[] Slots = new Slot[27];
        public Inventory()
        {
            for (int i = 0; i < Slots.Length; i++)
            {
                Slots[i] = new Slot();
            }
        }
    }
}
