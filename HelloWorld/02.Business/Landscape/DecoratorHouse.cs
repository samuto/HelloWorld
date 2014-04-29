using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.Business.Geometry;

namespace WindowsFormsApplication7.Business.Landscape
{
    class DecoratorHouse
    {
        public ChunkPointer Pointer;

        internal void Build(int x, int y, int z, int levels)
        {
            // build house
            for (int i = 0; i < levels; i++)
            {
                bool buildDoor = i == 0;
                BuildLevel(x, y + i * 4, z, buildDoor);
            }
            //build stairs
            for (int i = 0; i < levels; i++)
            {
                BuildStairs(x, y + i * 4, z);
            }
            //build roof
            BuildRoof(x, y + levels*4, z);
        }

        private void BuildRoof(int x, int y, int z)
        {
            for (int dx = 0; dx < 8; dx++)
            {
                for (int dz = 0; dz < 8; dz++)
                {
                    if ((dx + dz) % 2 == 0)
                        continue;
                    if (dx == 0 || dz == 0 || dx == 7 || dz == 7)
                    {
                        Pointer.SetBlock(x + dx, y, z + dz, BlockRepository.CobbleStone.Id);
                    }
                }
            }
        }

        private void BuildStairs(int x, int y, int z)
        {
            Pointer.SetBlock(x + 1, y + 1, z + 3, BlockRepository.Plank.Id);
            Pointer.SetBlock(x + 1, y + 2, z + 4, BlockRepository.Plank.Id);
            Pointer.SetBlock(x + 1, y + 3, z + 5, BlockRepository.Plank.Id);
            Pointer.SetBlock(x + 1, y + 4, z + 6, BlockRepository.Plank.Id);

            Pointer.SetBlock(x + 1, y + 3, z + 1, BlockRepository.Air.Id);
            Pointer.SetBlock(x + 1, y + 3, z + 2, BlockRepository.Air.Id);
            Pointer.SetBlock(x + 1, y + 3, z + 3, BlockRepository.Air.Id);
            Pointer.SetBlock(x + 1, y + 3, z + 4, BlockRepository.Air.Id);

            Pointer.SetBlock(x + 1, y + 4, z + 3, BlockRepository.Air.Id);
            Pointer.SetBlock(x + 1, y + 4, z + 4, BlockRepository.Air.Id);
            Pointer.SetBlock(x + 1, y + 4, z + 5, BlockRepository.Air.Id);
            
        }

        private void BuildLevel(int x, int y, int z, bool door)
        {
            // trunk
            for (int dx = 0; dx < 8; dx++)
            {
                for (int dy = 0; dy < 4; dy++)
                {
                    for (int dz = 0; dz < 8; dz++)
                    {
                        if (dy == 0)
                        {
                            Pointer.SetBlock(x + dx, y + dy, z + dz, BlockRepository.Plank.Id);
                            continue;
                        }
                        if (dy == 3)
                        {
                            Pointer.SetBlock(x + dx, y + dy, z + dz, BlockRepository.CobbleStone.Id);
                            continue;
                        }
                        if (dx == 0 || dz == 0 || dx == 7 || dz == 7)
                        {
                            Pointer.SetBlock(x + dx, y + dy, z + dz, BlockRepository.CobbleStone.Id);
                        }
                        else
                            Pointer.SetBlock(x + dx, y + dy, z + dz, BlockRepository.Air.Id);
                    }
                }
            }
            //pillars
            for (int dy = -4; dy < 4; dy++)
            {
                Pointer.SetBlock(x + 0, y + dy, z + 0, BlockRepository.Wood.Id);
                Pointer.SetBlock(x + 0, y + dy, z + 7, BlockRepository.Wood.Id);
                Pointer.SetBlock(x + 7, y + dy, z + 0, BlockRepository.Wood.Id);
                Pointer.SetBlock(x + 7, y + dy, z + 7, BlockRepository.Wood.Id);

            }
            if (door)
            {
                Pointer.SetBlock(x + 1, y + 1, z + 0, BlockRepository.Air.Id);
                Pointer.SetBlock(x + 1, y + 2, z + 0, BlockRepository.Air.Id);
            }
            // windows
            Pointer.SetBlock(x + 3, y + 2, z + 0, BlockRepository.Glass.Id);
            Pointer.SetBlock(x + 4, y + 2, z + 0, BlockRepository.Glass.Id);
            Pointer.SetBlock(x + 6, y + 2, z + 0, BlockRepository.Glass.Id);
            Pointer.SetBlock(x + 3, y + 2, z + 7, BlockRepository.Glass.Id);
            Pointer.SetBlock(x + 4, y + 2, z + 7, BlockRepository.Glass.Id);
            Pointer.SetBlock(x + 6, y + 2, z + 7, BlockRepository.Glass.Id);
            Pointer.SetBlock(x + 7, y + 2, z + 2, BlockRepository.Glass.Id);
            Pointer.SetBlock(x + 7, y + 2, z + 3, BlockRepository.Glass.Id);
            Pointer.SetBlock(x + 7, y + 2, z + 4, BlockRepository.Glass.Id);
            Pointer.SetBlock(x + 0, y + 2, z + 2, BlockRepository.Glass.Id);
            Pointer.SetBlock(x + 0, y + 2, z + 3, BlockRepository.Glass.Id);
            Pointer.SetBlock(x + 0, y + 2, z + 4, BlockRepository.Glass.Id);

        }
    }
}