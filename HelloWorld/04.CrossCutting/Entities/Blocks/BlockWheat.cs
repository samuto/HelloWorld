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
    class BlockWheat : Block
    {
        public BlockWheat(int blockId)
            : base(blockId)
        {
        }

        internal override int[] OnDroppedIds()
        {
            double dropChance = MathLibrary.GlobalRandom.NextDouble();
            double c = 0;
            // 50% chance of dropping 2x Wheat + 2x seed
            if (dropChance < (c += 0.50))
                return new int[] { ItemRepository.Wheat.Id, ItemRepository.SeedsWheat.Id };
            // 25% change of dropping 1x Wheat + 2x seed 
            else if (dropChance < (c += 0.25))
                return new int[] { ItemRepository.Wheat.Id, ItemRepository.SeedsWheat.Id, ItemRepository.SeedsWheat.Id };
            // 25% change of dropping 1x Wheat + 1x seed 
            else
                return new int[] { ItemRepository.Wheat.Id, ItemRepository.Wheat.Id, ItemRepository.SeedsWheat.Id, ItemRepository.SeedsWheat.Id };
        }

        internal override void OnDestroy(PositionBlock pos)
        {
            Chunk chunk = World.Instance.GetChunk(PositionChunk.CreateFrom(pos));
            PositionBlock localpos = pos;
            chunk.Position.ConvertToLocalPosition(ref localpos);
            int stage = chunk.MetaDataGetInt("stage", localpos);
            if (stage != 7)
            {
                DropStack(new EntityStack(ItemRepository.SeedsWheat.Id, 1), pos);
                return;
            }
            base.OnDestroy(pos);
        }

        internal override Entity CreateEntity()
        {
            return new Wheat();
        }

    }
}
