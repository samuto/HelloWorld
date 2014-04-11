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
            int stage;
            Dictionary<string, object> metaData;
            metaData = Parent.GetBlockMetaData(PositionBlock);
            if (!metaData.ContainsKey("stage"))
            {
                metaData["stage"] = -1;
            }
            stage = (int)metaData["stage"];
            if(stage < 7)
                stage++;
           
            metaData["stage"] = stage;
            Parent.Invalidate();
        }

    }
}
