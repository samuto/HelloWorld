using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;

namespace WindowsFormsApplication7.Business.Landscape
{
    class ChunkGenerator
    {
        bool disableRecursivecalls = false;

        internal void Generate(Chunk chunk)
        {
            if (disableRecursivecalls)
                return;
            disableRecursivecalls = true;
            PositionBlock positionBlock;
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        chunk.Position.GetGlobalPositionBlock(out positionBlock, x, y, z);
                        int globalY = positionBlock.Y;
                        if (globalY > 20)
                            continue;
                        else if (globalY < 20)
                        {
                            chunk.SetLocalBlock(x, y, z, BlockRepository.Dirt.Id);
                            continue;
                        }

                        chunk.SetLocalBlock(x, y, z, BlockRepository.Dirt.Id);

                        // hit me with some artifacts for collision detection:
                        if (chunk.Position.X == -2 && chunk.Position.Z == -2)
                        {
                            if (x % 4 + z % 4 == 0)
                            {
                                chunk.SafeSetLocalBlock(x, y + 1, z, BlockRepository.Sand.Id);
                            }
                        }
                        else if (chunk.Position.X == -3 && chunk.Position.Z == -3)
                        {
                            chunk.SafeSetLocalBlock(x, y + x + z + 1, z, BlockRepository.Grass.Id);
                        }
                        else if (chunk.Position.X == -1 && chunk.Position.Z == -1)
                        {
                            if (x % 2 + z % 2 == 0)
                            {
                                chunk.SafeSetLocalBlock(x, y + x+1, z, BlockRepository.Stone.Id);
                            }
                        }
                        else if (chunk.Position.X == -2 && chunk.Position.Z == -3)
                        {
                            if (x % 2 + z % 2 == 0)
                            {
                                chunk.SafeSetLocalBlock(x, y + 4, z, BlockRepository.Wood.Id);
                            }
                        }
                    }
                }
            }
            disableRecursivecalls = false;
        }
    }
}
