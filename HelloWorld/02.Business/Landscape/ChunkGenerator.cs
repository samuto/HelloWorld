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
                        if (globalY != 20)
                            continue;
                        double t = positionBlock.X / 10d;
                        double t2 = positionBlock.Z / 20d;
                        double v = (Math.Sin(t) + Math.Cos(t2)) * 50d + 50d;
                        if (v < 10)
                            chunk.SetLocalBlock(x, y, z, BlockRepository.Sand.Id);
                        else if (v < 20)
                            chunk.SetLocalBlock(x, y, z, BlockRepository.Grass.Id);
                        else if (v < 30)
                            chunk.SetLocalBlock(x, y, z, BlockRepository.BedRock.Id);
                        else if (v < 40)
                            chunk.SetLocalBlock(x, y, z, BlockRepository.Stone.Id);
                        else
                            chunk.SetLocalBlock(x, y, z, BlockRepository.Dirt.Id);

                    }


                }
            }
            disableRecursivecalls = false;
        }
    }
}
