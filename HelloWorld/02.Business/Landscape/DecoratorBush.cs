using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.Business.Geometry;

namespace WindowsFormsApplication7.Business.Landscape
{
    class DecoratorBush
    {
        public ChunkPointer Pointer;

        internal void Plant(int x, int y, int z, int bushsize)
        {
            for (int dx = -bushsize / 2; dx <= bushsize / 2; dx++)
            {
                for (int dy = 0; dy <= bushsize; dy++)
                {
                    for (int dz = -bushsize / 2; dz <= bushsize / 2; dz++)
                    {
                        if (dx * dx + dy * dy + dz * dz > (bushsize * bushsize / 3))
                            continue;
                        Pointer.ReplaceBlock(x + dx, y + dy, z + dz, BlockRepository.Air.Id, BlockRepository.Leaf.Id);
                    }
                }
            }
        }
    }
}