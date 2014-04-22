using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.Business.Landscape
{
    class DebugGenerator : ChunkGeneratorBase
    {
        bool disableRecursivecalls = false;

        internal override void Generate(Chunk chunk)
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

                        if (globalY < 32)
                        {
                            chunk.SetLocalBlock(x, y, z, BlockRepository.Stone.Id);
                            continue;
                        }
                        else if (globalY < 64)
                        {
                            chunk.SetLocalBlock(x, y, z, BlockRepository.Dirt.Id);
                            continue;
                        }
                        if (globalY == 64)
                        {
                            if (chunk.Position.X == 1 && chunk.Position.Z == -1 && globalY == 64)
                            {
                                //chunk.SafeSetLocalBlock(x, y, z, BlockRepository.Water.Id);
                            }
                            else 
                                chunk.SetLocalBlock(x, y, z, BlockRepository.DirtWithGrass.Id);
                            continue;
                        }

                        // hit me with some artifacts for collision detection:
                        if (chunk.Position.X == -2 && chunk.Position.Z == -2 && globalY == 65)
                        {
                            if (x % 4 + z % 4 == 0)
                            {
                                chunk.SafeSetLocalBlock(x, y, z, BlockRepository.Sand.Id);
                            }
                        }
                        else if (chunk.Position.X == -1 && chunk.Position.Z == -2 && globalY == 65)
                        {
                            chunk.SafeSetLocalBlock(x, y, z, BlockRepository.TallGrass.Id);
                        }
                        else if (chunk.Position.X == -3 && chunk.Position.Z == -3 && globalY < 16-x -  z + 65)
                        {
                            chunk.SafeSetLocalBlock(x, y, z, globalY == 16-x - z + 64 ? BlockRepository.DirtWithGrass.Id : BlockRepository.Dirt.Id);
                        }
                        else if (chunk.Position.X == -1 && chunk.Position.Z == -1 && globalY < 16 - x - z + 65)
                        {
                            chunk.SafeSetLocalBlock(15 - x, y, 15 - z, globalY == 16 - x - z + 64 ? BlockRepository.CobbleStone.Id : BlockRepository.Stone.Id);
                        }
                        
                        else if (chunk.Position.X == -2 && chunk.Position.Z == -3 && globalY > 64 && globalY < 68)
                        {
                            if (x % 2 + z % 2 == 0)
                            {
                                chunk.SafeSetLocalBlock(x, y, z, BlockRepository.Wood.Id);
                            }
                        }
                    }
                }
            }
            disableRecursivecalls = false;
        }
    }
}
