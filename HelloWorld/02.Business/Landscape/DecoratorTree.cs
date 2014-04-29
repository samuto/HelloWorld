using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.Business.Geometry;

namespace WindowsFormsApplication7.Business.Landscape
{
    class DecoratorTree
    {
        public ChunkPointer Pointer;

        internal void Plant(int x, int y, int z, int trunksize)
        {
            // trunk
            for (int dy = 0; dy < trunksize; dy++)
            {
                Pointer.ReplaceBlock(x, y + dy, z, BlockRepository.Air.Id, BlockRepository.Wood.Id);
            }
            // top
            int topsize = trunksize;
            for (int dx = -topsize / 2; dx <= topsize / 2; dx++)
            {
                for (int dy = 0; dy <= topsize; dy++)
                {
                    for (int dz = -topsize / 2; dz <= topsize / 2; dz++)
                    {
                        if (dx * dx + dy * dy + dz * dz > (topsize * topsize/3)+1)
                            continue;
                        Pointer.ReplaceBlock(x + dx, y + trunksize/2 + dy, z + dz, BlockRepository.Air.Id, BlockRepository.Leaf.Id);
                    }
                }
            }
        }
    }
}