using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.Business.Landscape
{
    class GeneratorFlat : GeneratorBase
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

                            chunk.SetLocalBlock(x, y, z, BlockRepository.DirtWithGrass.Id);
                            continue;
                        }

                    }
                }
            }
            disableRecursivecalls = false;
        }
    }
}
