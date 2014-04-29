using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.Business.Geometry;

namespace WindowsFormsApplication7.Business.Landscape
{
    class DecoratorBiome
    {
        private DecoratorTree tree = new DecoratorTree();
        private DecoratorBush bush = new DecoratorBush();
        private DecoratorHouse house = new DecoratorHouse();

        internal void Decorate(Chunk chunk)
        {
            Random rnd = new Random(chunk.Position.X*12345 ^ chunk.Position.Y*23456 ^ chunk.Position.Z*34657);
            PositionBlock chunkCorner;
            chunk.Position.GetMinCornerBlock(out chunkCorner);
            ChunkPointer pointer = ChunkPointer.Create(chunkCorner.X, chunkCorner.Y, chunkCorner.Z);
            tree.Pointer = pointer;
            bush.Pointer = pointer;
            house.Pointer = pointer;
            for (int x = chunkCorner.X; x < chunkCorner.X + 16; x++)
            {
                for (int z = chunkCorner.Z; z < chunkCorner.Z + 16; z++)
                {
                    for (int y = Chunk.MaxSizeY - 1; y >= 0; y--)
                    {
                        // find top block
                        if (pointer.GetBlock(x, y, z) == BlockRepository.Stone.Id)
                        {
                            pointer.SetBlock(x, y, z, BlockRepository.DirtWithGrass.Id);
                            double propability = rnd.NextDouble();
                            if (propability < 0.02)
                            {
                                tree.Plant(x, y + 1, z, rnd.Next(4, 10));
                            }
                            else if (propability < 0.04)
                            {
                                bush.Plant(x, y + 1, z, rnd.Next(1, 3));
                            }
                            else if (propability < 0.041)
                            {
                                house.Build(x, y + 1, z, rnd.Next(1, 8));
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
