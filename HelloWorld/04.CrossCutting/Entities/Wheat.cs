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
            int stage = (int)Parent.GetBlockMetaData(PositionBlock, "stage");
            if (stage < 7)
            {
                stage++;
            }
            Parent.SetBlockMetaData(PositionBlock, "stage", stage);
            Parent.Invalidate();
        }

        internal override void OnInitialize()
        {
            Parent.SetBlockMetaData(PositionBlock, "stage", 0);
        }
    }
}
