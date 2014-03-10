using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;

namespace WindowsFormsApplication7.Business.Landscape
{
    class Forrest
    {
        internal void Populate(Chunk chunk)
        {
            int seed = chunk.Position.X * 1000 + chunk.Position.Z;
            Random rnd = new Random(seed);
            int treeCount = rnd.Next(20, 30);
            PositionBlock[] trees = new PositionBlock[treeCount];
            for (int i = 0; i < treeCount; i++)
            {
                trees[i] = new PositionBlock(
                    rnd.Next(0, 16),
                    0,
                    rnd.Next(0, 16));
            }
            // Populate trees
            for (int i = 0; i < treeCount; i++)
            {
                int x = trees[i].X;
                int z = trees[i].Z;
                int blockId = chunk.GetLocalBlock(x, Chunk.SizeY - 1, z);
                if (blockId != 0)
                    continue;
                int y = Chunk.SizeY - 1;
                while (chunk.GetLocalBlock(x, y, z) == 0 && y > 0)
                {
                    y--;
                }
                if (y == 0)
                    continue;
                int blockIdToBePlantedUpon = chunk.GetLocalBlock(x, y, z);
                if (blockIdToBePlantedUpon != BlockRepository.Grass.Id && blockIdToBePlantedUpon != BlockRepository.Dirt.Id)
                    continue;
                // plant tree
                PositionBlock pos = new PositionBlock(x, y, z);

                bool canGrow = true;
                int treeHeight;

                if (blockIdToBePlantedUpon == BlockRepository.Dirt.Id)
                    treeHeight = rnd.Next(3, 6);
                else
                {
                    if (rnd.Next(0, 4) < 3)
                        continue;
                    chunk.ReplaceBlock(pos.Add(0, 1, 0), 0, BlockRepository.Leaf.Id);
                    continue;
                }
                int step = treeHeight;
                while (step > 0 && (canGrow = chunk.ReplaceBlock(pos.Add(0, 1, 0), 0, BlockRepository.Wood.Id)))
                {
                    step--;
                }

                int r = (int)(treeHeight / 1f);
                PositionBlock pos2 = new PositionBlock();
                for (int x2 = -r+1; x2 <= r-1; x2++)
                {
                    for (int y2 = 0; y2 <= r; y2++)
                    {
                        for (int z2 = -r+1; z2 <= r-1; z2++)
                        {
                            if (x2 * x2 + y2 * y2 + z2 * z2 > r * r)
                                continue;
                            pos2.X = pos.X + x2;
                            pos2.Y = pos.Y + y2;
                            pos2.Z = pos.Z + z2;
                            chunk.ReplaceBlock(pos2, 0, BlockRepository.Leaf.Id);

                        }
                    }
                }

            }

        }
    }
}
