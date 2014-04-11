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

        public override bool OnUseOnBlock(PositionBlock pos)
        {
            if (World.Instance.GetBlock(pos) != BlockRepository.FarmlandDry.Id)
                return false;
            World.Instance.SetBlock(pos.X, pos.Y+1, pos.Z, BlockRepository.Wheat.Id);
            return true;
        }
    }
}
