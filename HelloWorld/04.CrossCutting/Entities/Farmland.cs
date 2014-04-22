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
    class Farmland : Entity
    {
        public Farmland()
        {
            EntityType = EntityTypeEnum.BlockRandomUpdate;
        }

        internal override void OnUpdate()
        {
            MakeMeWetIfWaterHere(-1, 0, -1);
            MakeMeWetIfWaterHere(0, 0, -1);
            MakeMeWetIfWaterHere(1, 0, -1);
            MakeMeWetIfWaterHere(-1, 0, 0);
            MakeMeWetIfWaterHere(0, 0, 0);
            MakeMeWetIfWaterHere(1, 0, 0);
            MakeMeWetIfWaterHere(-1, 0, 1);
            MakeMeWetIfWaterHere(0, 0, 1);
            MakeMeWetIfWaterHere(1, 0, 1);
            Parent.Invalidate();
        }

        private void MakeMeWetIfWaterHere(int dx, int dy, int dz)
        {
            if (Parent.SafeGetLocalBlock(BlockPosition.X + dx, BlockPosition.Y + dy, BlockPosition.Z + dz) == BlockRepository.Water.Id)
                Parent.SafeSetLocalBlock(BlockPosition.X, BlockPosition.Y, BlockPosition.Z, BlockRepository.FarmlandWet.Id);
        }
    }
}
