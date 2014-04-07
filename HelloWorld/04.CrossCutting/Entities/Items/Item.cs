using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Frontend;
using WindowsFormsApplication7.Business;
using SlimDX;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.CrossCutting.Entities.Items
{
    class Item
    {
        public int Id;
        public string Name;
        public bool Consumable = false;

        public Item(int itemId, string name)
        {
            this.Id = ItemRepository.ItemIdOffset + itemId;
            this.Name = name;
            ItemRepository.Items[Id] = this;
        }


        internal static Item FromId(int globalId)
        {
            return ItemRepository.Items[globalId];
        }

        public virtual bool UseOnBlock(PositionBlock pos)
        {
            return false;
        }

        internal Item SetConsumable()
        {
            this.Consumable = true;
            return this;
        }

        internal virtual void OnPunchedWith()
        {
            
        }
    }
}
