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
    class Water : Entity
    {
        public const int WaterStartLevel = 8;
            

        public Water()
        {
            EntityType = EntityTypeEnum.BlockRandomUpdate;
        }

        internal override void OnUpdate()
        {
            int waterLevel = (int)Parent.GetBlockMetaData(PositionBlock, "waterlvl");
            PositionBlock[] offsets = new Entities.PositionBlock[] {
                new PositionBlock(0,-1,0),
                new PositionBlock(-1,0,0),
                new PositionBlock(1,0,0),
                new PositionBlock(0,0,-1),
                new PositionBlock(0,0,1),
            };
            int newWaterLevel = waterLevel - 1;
            int maxLevel = waterLevel;
            for (int i = 0; i < offsets.Length; i++)
            {
                PositionBlock pos2 = new PositionBlock(PositionBlock.X + offsets[i].X, PositionBlock.Y + offsets[i].Y, PositionBlock.Z + offsets[i].Z);
                int destBlockId = Parent.SafeGetLocalBlock(pos2.X, pos2.Y, pos2.Z);
                if (destBlockId != 0 && destBlockId != BlockRepository.Water.Id)
                    continue;
                bool waterFlowsDown = offsets[i].Y < 0;
                if (waterFlowsDown)
                {
                    newWaterLevel = WaterStartLevel;
                }
                if (destBlockId == BlockRepository.Water.Id)
                {
                    int neighborLevel = (int)Parent.GetBlockMetaData(pos2, "waterlvl");
                    if (neighborLevel < waterLevel - 1 || waterFlowsDown)
                    {
                        UpdateWaterLevel(pos2, newWaterLevel);
                    }
                    maxLevel = Math.Max(maxLevel, neighborLevel);
                }
                else
                {
                    UpdateWaterLevel(pos2, newWaterLevel);
                }
                if (waterFlowsDown)
                    break;
            }
            if (maxLevel < WaterStartLevel && maxLevel == waterLevel)
            {
                UpdateWaterLevel(PositionBlock, waterLevel - 1);
                
            }
        }

        private void UpdateWaterLevel(PositionBlock PositionBlock, int newWaterLevel)
        {
            if (newWaterLevel <= 0)
                Parent.SafeSetLocalBlock(PositionBlock.X, PositionBlock.Y, PositionBlock.Z, 0);
            else
            {
                Parent.SafeSetLocalBlock(PositionBlock.X, PositionBlock.Y, PositionBlock.Z, BlockRepository.Water.Id);
                Parent.SetBlockMetaData(PositionBlock, "waterlvl", newWaterLevel);
            }
        }

        internal override void OnInitialize()
        {
            Parent.SetBlockMetaData(PositionBlock, "waterlvl", WaterStartLevel);
        }


    }
}
