using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using SlimDX;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Chunk
    {
        public const int MaxSizeY = 128;
        public bool Expired;
        public PositionChunk Position;
        private byte[] blocks = new byte[16 * 16 * 16];
        public bool RequiresRendering = true;
        public bool Initialized;

        public Chunk()
        {
        }

        public void SetLocalBlock(int x, int y, int z, int blockId)
        {
            blocks[x * 16 * 16 + y * 16 + z] = (byte)blockId;
        }

        public void InvalidateMeAndNeighbors()
        {
            RequiresRendering = true;
            World.Instance.GetChunk(new PositionChunk(Position.X-1, Position.Y, Position.Z)).RequiresRendering = true;
            World.Instance.GetChunk(new PositionChunk(Position.X, Position.Y-1, Position.Z)).RequiresRendering = true;
            World.Instance.GetChunk(new PositionChunk(Position.X, Position.Y, Position.Z-1)).RequiresRendering = true;
            World.Instance.GetChunk(new PositionChunk(Position.X+1, Position.Y, Position.Z)).RequiresRendering = true;
            World.Instance.GetChunk(new PositionChunk(Position.X, Position.Y+1, Position.Z)).RequiresRendering = true;
            World.Instance.GetChunk(new PositionChunk(Position.X, Position.Y, Position.Z+1)).RequiresRendering = true;
        }

        public byte GetLocalBlock(int x, int y, int z)
        {
            return blocks[x * 16 * 16 + y * 16 + z];
        }

        public void SafeSetLocalBlock(int x, int y, int z, int blockId)
        {
            if (x < 0 || x >= 16 ||
                y < 0 || y >= 16 ||
                z < 0 || z >= 16)
            {
                PositionBlock globalPosition;
                Position.GetGlobalPositionBlock(out globalPosition, x, y, z);
                World.Instance.SetBlock(globalPosition.X, globalPosition.Y, globalPosition.Z, blockId);
                return;
            }
            SetLocalBlock(x, y, z, blockId);
        }

        internal int SafeGetLocalBlock(int x, int y, int z)
        {
            if (x < 0 || x >= 16 ||
                y < 0 || y >= 16 ||
                z < 0 || z >= 16)
            {
                PositionBlock globalPosition;
                Position.GetGlobalPositionBlock(out globalPosition, x, y, z);
                return World.Instance.GetBlock(globalPosition.X, globalPosition.Y, globalPosition.Z);
            }
            return GetLocalBlock(x, y, z);
        }


        internal void Dipose()
        {
        }


        internal bool ReplaceBlock(int x, int y, int z, int oldId, int newId)
        {
            if (oldId == SafeGetLocalBlock(x, y, z))
            {
                SafeSetLocalBlock(x, y, z, newId);
                return true;
            }
            return false;
        }

        internal void Initialize()
        {
            World.Instance.Generate(this);
            Initialized = true;
        }

        internal void RenderingDone()
        {
            RequiresRendering = false;
        }

        internal void OnVertexBufferDisposed()
        {
            RequiresRendering = true;
        }

        internal SlimDX.BoundingBox GetBoundingBox()
        {
            PositionBlock minGlobalPos, maxGlobalPos;
            Position.GetGlobalPositionBlock(out minGlobalPos, 0, 0, 0);
            Position.GetGlobalPositionBlock(out maxGlobalPos, 16, 16, 16);
            return new SlimDX.BoundingBox(
                new Vector3(minGlobalPos.X, minGlobalPos.Y, minGlobalPos.Z), 
                new Vector3(maxGlobalPos.X, maxGlobalPos.Y, maxGlobalPos.Z));
        }
    }
}
