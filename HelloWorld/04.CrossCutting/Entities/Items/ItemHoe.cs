using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.CrossCutting.Entities.Items
{
    class ItemHoe : Item
    {
        public ItemHoe(int itemId, string name)
            : base(itemId, name)
        {
        }

        public override bool UseOnBlock(PositionBlock pos)
        {
            World.Instance.SetBlock(pos.X, pos.Y, pos.Z, BlockRepository.FarmlandDry.Id);
            return true;
        }
    }
}
