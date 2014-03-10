using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;

namespace WindowsFormsApplication7.Business.Landscape
{
    class ChunkGenerator
    {
        Forrest forrest = new Forrest();
        bool disableRecursivecalls = false;

        internal void Generate(Chunk chunk)
        {
            if (disableRecursivecalls)
                return;
            disableRecursivecalls = true;
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    PositionBlock positionBlock = chunk.Position.GetGlobalPositionBlock(x, 0, z);
                    double t = positionBlock.X / 30d;
                    double t2 = positionBlock.Z / 20d;
                    double height = (Math.Sin(t) + Math.Cos(t2)) *10d+ 30d;
                    for (int y = 0; y < (int)height; y++)
                    {
                        bool isTopBlock = y == (int)height - 1;
                        
                        if (y < 35)
                        {
                            if(isTopBlock)
                            {
                                if (y < 15)
                                {
                                    for (int i = y+1; i < 13; i++)
                                    {
                                        chunk.SetLocalBlock(x, i, z, BlockRepository.Water.Id);
                                    }
                                    chunk.SetLocalBlock(x, y, z, BlockRepository.Sand.Id);
                                }
                                else
                                    chunk.SetLocalBlock(x, y, z, BlockRepository.Grass.Id);
                            }
                            else if(y==0)
                                chunk.SetLocalBlock(x, y, z, BlockRepository.BedRock.Id);
                            else if (y < 10)
                                chunk.SetLocalBlock(x, y, z, BlockRepository.Stone.Id);
                            else
                                chunk.SetLocalBlock(x, y, z, BlockRepository.Dirt.Id);

                        }
                    }
                }
            }
            forrest.Populate(chunk);
            disableRecursivecalls = false;
        }
    }
}
