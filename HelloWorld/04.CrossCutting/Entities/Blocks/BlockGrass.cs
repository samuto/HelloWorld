using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Frontend;
using WindowsFormsApplication7.Business;
using SlimDX;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.Business.Geometry;

namespace WindowsFormsApplication7.CrossCutting.Entities.Blocks
{
    class BlockGrass : Block
    {
        public BlockGrass(int blockId)
            : base(blockId)
        {
        }

        internal override int[] DroppedIds()
        {
            if(MathLibrary.GlobalRandom.NextDouble() > 0.1)
                return new int[0];
            return new int[1] { ItemRepository.SeedsWheat.Id };
        }
    }
}
