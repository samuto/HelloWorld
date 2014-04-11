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
    class Item : Entity
    {
        public Item(int itemId, string name)
        {
            this.Id = ItemRepository.ItemIdOffset + itemId;
            this.Name = name;
            ItemRepository.Items[Id] = this;
        }

        internal new static Item FromId(int globalId)
        {
            return ItemRepository.Items[globalId];
        }

        //
        // setters // getters
        //
        internal Item SetConsumable()
        {
            this.Consumable = true;
            return this;
        }

        //
        // events / default behaviour
        //
        public virtual bool OnUseOnBlock(PositionBlock pos)
        {
            return false;
        }        

        internal virtual bool OnUseOnPlayer()
        {
            return false;
        }

        internal void OnAfterAttack()
        {
            
        }
    }
}
