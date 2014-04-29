using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.Business.Landscape
{
    class GeneratorBiome : GeneratorBase
    {
        private float[] noise3D;
        private float[] noise2D;
        private Chunk chunk;

        internal override void Generate(Chunk chunk)
        {
            this.chunk = chunk;
            PositionBlock pos;
            chunk.Position.GetGlobalPositionBlock(out pos, 0, 0, 0);
            noise3D = GetScaledNoise(pos, 0.04f);
            PositionBlock pos2 = pos;
            pos2.Y = 0;
            noise2D = GetScaledNoise(pos2, 0.01f);

            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        float height = pos.Y + y;
                        if (height == 0)
                        {
                            chunk.SetLocalBlock(x, y, z, BlockRepository.BedRock.Id);
                        }
                        else if (height <= 60)
                        {
                            chunk.SetLocalBlock(x, y, z, BlockRepository.Stone.Id);
                        }
                        else if (height <= 80)
                        {
                            float limit = CalcOffset(height, 60, 80) * 2f - 1f;
                            Magic(height, x, y, z, BlockRepository.Stone.Id, limit);
                        }
                    }
                }
            }
        }

        internal override void Decorate(Chunk chunk)
        {
            DecoratorBiome decorator = new DecoratorBiome();
            decorator.Decorate(chunk);
        }

        private float CalcOffset(float height, float min, float max)
        {
            float offset = (height - min) / (max - min);
            if (offset > 1f)
                offset = 1f;
            else if (offset < 0)
                offset = 0f;
            return offset;
        }


        internal void Magic(float height, int x, int y, int z, int blockId, float limit = 0)
        {
            float n1 = Terp(noise3D, x, y, z);
            if (n1 > limit)
            {
                chunk.SetLocalBlock(x, y, z, blockId);
            }
        }


        private float[] GetScaledNoise(PositionBlock pos, float scale, int offset = 0)
        {
            pos.X += offset;
            pos.Z += offset;
            float x = pos.X * scale;
            float y = pos.Y * scale;
            float z = pos.Z * scale;

            float x2 = (pos.X + 16f) * scale;
            float y2 = (pos.Y + 16f) * scale;
            float z2 = (pos.Z + 16f) * scale;
            float[] noise = new float[] { 
                 Noise.Generate(x, y, z2),
                 Noise.Generate(x, y, z),
                 Noise.Generate(x2, y, z),
                 Noise.Generate(x2, y, z2),
                
                 Noise.Generate(x, y2, z2),
                 Noise.Generate(x, y2, z),
                 Noise.Generate(x2, y2, z),
                 Noise.Generate(x2, y2, z2),
            };
            return noise;
        }

        public static float Terp(float[] noiseValues, int x, int y, int z)
        {
            float dx = x / 16.0f;
            float dy = y / 16.0f;
            float dz = z / 16.0f;
            float x1 = noiseValues[1] * (1f - dx) + noiseValues[2] * dx;
            float x2 = noiseValues[5] * (1f - dx) + noiseValues[6] * dx;
            float x3 = noiseValues[0] * (1f - dx) + noiseValues[3] * dx;
            float x4 = noiseValues[4] * (1f - dx) + noiseValues[7] * dx;

            float y1 = x1 * (1f - dy) + x2 * dy;
            float y2 = x3 * (1f - dy) + x4 * dy;

            float z1 = y1 * (1f - dz) + y2 * dz;

            return z1;
        }


    }
}
