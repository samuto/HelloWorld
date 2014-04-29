using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using SlimDX;
using WindowsFormsApplication7.Business.Profiling;
using WindowsFormsApplication7.Business.Geometry;

namespace WindowsFormsApplication7.Business
{
    class ChunkPointer
    {
        private Chunk chunk;
        private ChunkCache chunkCache;
        private PositionBlock chunkCorner;

        private ChunkPointer(int x, int y, int z)
        {
            ChangeChunk(x,y,z);
        }

        private void ChangeChunk(int x, int y, int z)
        {
            PositionChunk positionChunk = PositionChunk.CreateFrom(new PositionBlock(x,y,z));
            chunkCache = World.Instance.GetCachedChunks();
            chunk = chunkCache.GetChunk(positionChunk);
            chunk.Position.GetMinCornerBlock(out chunkCorner);

        }

        internal static ChunkPointer Create(int x, int y, int z)
        {
            ChunkPointer pointer = new ChunkPointer(x, y, z);
            return pointer;
        }

        private void ConvertToLocal(int x, int y, int z, out int localX, out int localY, out int localZ)
        {
            localX = x - chunkCorner.X;
            localY = y - chunkCorner.Y;
            localZ = z - chunkCorner.Z;
            bool newChunk = false;
            if (localX < 0)
            {
                newChunk = true;
            }
            else if (localX > 15)
            {
                newChunk = true;
            }
            if (localY < 0)
            {
                newChunk = true;
            }
            else if (localY > 15)
            {
                newChunk = true;
            }
            if (localZ < 0)
            {
                newChunk = true;
            }
            else if (localZ > 15)
            {
                newChunk = true;
            }
            if (newChunk)
            {
                ChangeChunk(x, y, z);
                localX = x - chunkCorner.X;
                localY = y - chunkCorner.Y;
                localZ = z - chunkCorner.Z;
            }
        }

        internal int GetBlock(int x, int y, int z)
        {
            int localX, localY, localZ;
            ConvertToLocal(x, y, z, out localX, out localY, out localZ);
            return chunk.SafeGetLocalBlock(localX, localY, localZ);
        }

        internal void SetBlock(int x, int y, int z, int blockId)
        {
            int localX, localY, localZ;
            ConvertToLocal(x,y,z, out localX, out localY, out localZ);
            chunk.SafeSetLocalBlock(localX, localY, localZ, blockId);
        }


        internal void ReplaceBlock(int x, int y, int z, int oldId, int newId)
        {
            int localX, localY, localZ;
            ConvertToLocal(x, y, z, out localX, out localY, out localZ);
            chunk.ReplaceBlock(localX, localY, localZ, oldId, newId);
        }
    }
}
