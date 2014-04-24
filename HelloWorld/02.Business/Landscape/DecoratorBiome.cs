using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.Business.Landscape
{
    class DecoratorBiome
    {
        internal void Decorate(GeneratorBiome generatorBiome)
        {
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    for (int y = Chunk.MaxSizeY-1; y >=0; y--)
                    {
                        if (World.Instance.GetBlock(x, y, z) != BlockRepository.Air.Id)
                        {
                            int k = 8;
                            continue;
                        }

                    }
                }
            }
        }
    }
}
