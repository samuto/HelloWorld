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
        public const float WaterDecrease = 1f / 8f;

        public Water()
        {
            EntityType = EntityTypeEnum.BlockRandomUpdate;
        }

        internal override void OnUpdate()
        {
            float waterLevel = GetWaterLevel();
            float waterLevelUp = GetWaterLevel(0, 1, 0);
            float waterLevelDown = GetWaterLevel(0, -1, 0);
            float waterLevelLeft = GetWaterLevel(-1, 0, 0);
            float waterLevelRight = GetWaterLevel(1, 0, 0);
            float waterLevelFront = GetWaterLevel(0, 0, 1);
            float waterLevelBack = GetWaterLevel(0, 0, -1);
            float maxLevel = waterLevel;
            maxLevel = Math.Max(maxLevel, Math.Max(waterLevelLeft, Math.Max(waterLevelRight, Math.Max(waterLevelBack, waterLevelFront))));

            int blockIdBelow = Parent.SafeGetLocalBlock(BlockPosition.X, BlockPosition.Y - 1, BlockPosition.Z);

            bool waterAbove = waterLevelUp > 0f;
            bool waterCanFlowDown = blockIdBelow == 0 || blockIdBelow == BlockRepository.Water.Id;
            bool waterFlowsFromThisBlock = waterLevel == maxLevel;
            bool thisIsTheOriginalSource = waterLevel == 1f;
            float newWaterLevel = waterLevel - WaterDecrease;

            // water flow: vertical/horizontal
            if (waterCanFlowDown)
            {
                if (newWaterLevel <= 0f)
                    newWaterLevel = WaterDecrease;
                newWaterLevel = 1f - WaterDecrease;
                WaterFlow(0, -1, 0, newWaterLevel, waterLevelDown);
            }
            else
            {
                WaterFlow(-1, 0, 0, newWaterLevel, waterLevelLeft);
                WaterFlow(1, 0, 0, newWaterLevel, waterLevelRight);
                WaterFlow(0, 0, 1, newWaterLevel, waterLevelFront);
                WaterFlow(0, 0, -1, newWaterLevel, waterLevelBack);
            }


            // if no water source we dry out...
            if (!waterAbove && !thisIsTheOriginalSource && waterFlowsFromThisBlock)
            {
                if (!waterCanFlowDown)
                {
                    WaterDry(0, 0, 0, waterLevel - 2f * WaterDecrease, waterLevel);
                    WaterDry(-1, 0, 0, waterLevelLeft - WaterDecrease, waterLevelLeft);
                    WaterDry(1, 0, 0, waterLevelRight - WaterDecrease, waterLevelRight);
                    WaterDry(0, 0, -1, waterLevelBack - WaterDecrease, waterLevelBack);
                    WaterDry(0, 0, 1, waterLevelFront - WaterDecrease, waterLevelFront);
                    WaterDry(0, -1, 0, waterLevelDown - WaterDecrease, waterLevelDown);
                }
                else
                {
                    WaterDry(0, 0, 0, 0, waterLevel);
                }
            }
        }

        private float GetWaterLevel(int dx, int dy, int dz)
        {
            PositionBlock destPos = new PositionBlock(BlockPosition.X + dx, BlockPosition.Y + dy, BlockPosition.Z + dz);
            int destBlockId = Parent.SafeGetLocalBlock(destPos.X, destPos.Y, destPos.Z);
            if (destBlockId != BlockRepository.Water.Id)
                return 0f;
            float waterLevel = (float)Parent.GetBlockMetaData(destPos, "waterlvl");
            return waterLevel;
        }

        private void WaterDry(int x, int y, int z, float newWaterLevel, float prevWaterLevel)
        {
            x = BlockPosition.X + x;
            y = BlockPosition.Y + y;
            z = BlockPosition.Z + z;
            if (newWaterLevel >= prevWaterLevel)
                return;
            if (prevWaterLevel == 0f && Parent.SafeGetLocalBlock(x, y, z) != 0)
                return;
            UpdateWater(x, y, z, newWaterLevel);
        }

        private void WaterFlow(int x, int y, int z, float newWaterLevel, float prevWaterLevel)
        {
            x = BlockPosition.X + x;
            y = BlockPosition.Y + y;
            z = BlockPosition.Z + z;
            if (newWaterLevel <= prevWaterLevel)
                return;
            if (prevWaterLevel == 0f && Parent.SafeGetLocalBlock(x, y, z) != 0)
                return;
            if (newWaterLevel >= 1f)
                newWaterLevel = 1f;
            UpdateWater(x, y, z, newWaterLevel);

        }

        private void UpdateWater(int x, int y, int z, float newWaterLevel)
        {
            if (newWaterLevel <= 0)
                Parent.SafeSetLocalBlock(x, y, z, 0);
            else
            {
                Parent.SafeSetLocalBlock(x, y, z, BlockRepository.Water.Id);
                Parent.SetBlockMetaData(new PositionBlock(x, y, z), "waterlvl", newWaterLevel);
            }
        }

        internal override void OnInitialize()
        {
            Parent.SetBlockMetaData(BlockPosition, "waterlvl", 1f);
        }



        internal float GetWaterLevel()
        {
            return (float)Parent.GetBlockMetaData(BlockPosition, "waterlvl");
        }

    }
}
