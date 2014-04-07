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
    class ItemSeeds : Item
    {
        public ItemSeeds(int itemId, string name)
            : base(itemId, name)
        {
        }

        public override bool UseOnBlock(PositionBlock pos)
        {
            if (World.Instance.GetBlock(pos.X, pos.Y + 1, pos.Z) != 0 || World.Instance.GetBlock(pos.X, pos.Y, pos.Z) != BlockRepository.FarmlandDry.Id)
                return false;
            World.Instance.SetBlock(pos.X, pos.Y+1, pos.Z, BlockRepository.Wheat.Id);
            return true;
        }
    }
}
