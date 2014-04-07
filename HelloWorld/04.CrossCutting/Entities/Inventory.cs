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

        internal void CollectStack(EntityStack stackToCollect)
        {
            int id = stackToCollect.Id;
            // find first stack of same type
            foreach (Slot slot in Slots)
            {
                if (slot.Content.Id == id && !slot.IsFull)
                {
                    stackToCollect.TransferAll(slot.Content);
                    if (stackToCollect.IsEmpty)
                        return;
                }
            }

            // find first empty slot
            foreach (Slot slot in Slots)
            {
                if (slot.IsEmpty)
                {
                    slot.Content.Id = id;
                    stackToCollect.TransferAll(slot.Content);
                    return;
                }
            }
        }
    }
}
