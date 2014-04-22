using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.Business.Geometry;

namespace WindowsFormsApplication7.CrossCutting.Entities.Items
{
    class ItemHoe : Item
    {
        public ItemHoe(int itemId, string name)
            : base(itemId, name)
        {
        }

        public override bool OnUseOnBlock(PositionBlock pos)
        {
            int sourceBlockId = World.Instance.GetBlock(pos);
            if (sourceBlockId == BlockRepository.DirtWithGrass.Id || sourceBlockId == BlockRepository.Dirt.Id)
            {
                World.Instance.SetBlock(pos, BlockRepository.FarmlandDry.Id);
            }
            return true;
        }
    }
}
