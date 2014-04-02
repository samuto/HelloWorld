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
            // An inventory is made of 100% empty slots :)
            for (int i = 0; i < Slots.Length; i++)
            {
                Slots[i] = new Slot();
            }
        }

        internal void AddBlock(int blockId)
        {
            // find first stack of same type
            foreach (Slot slot in Slots)
            {
                if (slot.Content.Id == blockId && !slot.IsFull)
                {
                    slot.Content.AddItems(1);
                    return;
                }
            }

            // find first empty slot
            foreach (Slot slot in Slots)
            {
                if (slot.IsEmpty)
                {
                    slot.Content.Id = blockId;
                    slot.Content.AddItems(1);
                    return;
                }
            }
        }
    }
}
