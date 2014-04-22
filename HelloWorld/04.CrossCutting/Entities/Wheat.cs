using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities.Blocks;
using WindowsFormsApplication7.Business.Recipes;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Wheat : Entity
    {
        public Wheat()
        {
            EntityType = EntityTypeEnum.BlockRandomUpdate;
        }

        internal override void OnUpdate()
        {
            float stage = (float)Parent.GetBlockMetaData(BlockPosition, "stage");
            if (stage < 1)
            {
                int sourceBlockId = Parent.SafeGetLocalBlock(BlockPosition.X, BlockPosition.Y - 1, BlockPosition.Z);
                if (sourceBlockId == BlockRepository.FarmlandWet.Id)
                    stage += 0.25f;
                else
                    stage += 0.05f;
            }
            if (stage > 1f)
                stage = 1f;
            Parent.SetBlockMetaData(BlockPosition, "stage", stage);
            Parent.Invalidate();
        }

        internal override void OnInitialize()
        {
            Parent.SetBlockMetaData(BlockPosition, "stage", 0f);
        }
    }
}
