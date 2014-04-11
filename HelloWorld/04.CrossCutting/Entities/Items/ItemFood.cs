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
    class ItemFood : Item
    {
        private float hungerDecrease = 10;

        public ItemFood(int itemId, string name)
            : base(itemId, name)
        {
        }

        internal ItemFood SetHungerDecrease(float percentPoints)
        {
            hungerDecrease = percentPoints;
            return this;
        }

        internal override bool OnUseOnPlayer()
        {
            World.Instance.Player.DecreaseHunger(10);
            return true;
        }
    }
}
