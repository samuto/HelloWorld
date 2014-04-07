using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using SlimDX;
using WindowsFormsApplication7.Business.Geometry;
using System.Diagnostics;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Chunk
    {
        public const int timeout = 30 * 1000;
        public const int MaxSizeY = 128;
        public PositionChunk Position;
        private byte[] blocks = new byte[16 * 16 * 16];
        public bool IsDirty = true;
        private bool initialized;
        public List<EntityStack> StackEntities = new List<EntityStack>();
        private Stopwatch stopwatch = new Stopwatch();

        public Chunk()
        {
            stopwatch.Start();
        }

        public void RenewLease()
        {
            stopwatch.Restart();
        }

        public bool Expired
        {
            get
            {
                return stopwatch.ElapsedMilliseconds > timeout;
            }
        }

        public void SetLocalBlock(int x, int y, int z, int blockId)
        {
            blocks[x * 16 * 16 + y * 16 + z] = (byte)blockId;
        }

        public void Invalidate()
        {
            // invalidates all pass'es
            IsDirty = true;
        }

        public void Update()
        {
            if (!initialized)
            {
                Initialize();
            }
            foreach (EntityStack stack in StackEntities)
            {
                stack.Update();
            }
        }

        public void InvalidateMeAndNeighbors()
        {
            Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X - 1, Position.Y - 1, Position.Z - 1)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X - 1, Position.Y + 0, Position.Z - 1)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X - 1, Position.Y + 1, Position.Z - 1)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X - 1, Position.Y - 1, Position.Z + 0)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X - 1, Position.Y + 0, Position.Z + 0)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X - 1, Position.Y + 1, Position.Z + 0)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X - 1, Position.Y - 1, Position.Z + 1)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X - 1, Position.Y + 0, Position.Z + 1)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X - 1, Position.Y + 1, Position.Z + 1)).Invalidate();

            World.Instance.GetChunk(new PositionChunk(Position.X + 0, Position.Y - 1, Position.Z - 1)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X + 0, Position.Y + 0, Position.Z - 1)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X + 0, Position.Y + 1, Position.Z - 1)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X + 0, Position.Y - 1, Position.Z + 0)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X + 0, Position.Y + 0, Position.Z + 0)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X + 0, Position.Y + 1, Position.Z + 0)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X + 0, Position.Y - 1, Position.Z + 1)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X + 0, Position.Y + 0, Position.Z + 1)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X + 0, Position.Y + 1, Position.Z + 1)).Invalidate();

            World.Instance.GetChunk(new PositionChunk(Position.X + 1, Position.Y - 1, Position.Z - 1)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X + 1, Position.Y + 0, Position.Z - 1)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X + 1, Position.Y + 1, Position.Z - 1)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X + 1, Position.Y - 1, Position.Z + 0)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X + 1, Position.Y + 0, Position.Z + 0)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X + 1, Position.Y + 1, Position.Z + 0)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X + 1, Position.Y - 1, Position.Z + 1)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X + 1, Position.Y + 0, Position.Z + 1)).Invalidate();
            World.Instance.GetChunk(new PositionChunk(Position.X + 1, Position.Y + 1, Position.Z + 1)).Invalidate();


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

        private void Initialize()
        {
            World.Instance.Generate(this);
            initialized = true;
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

        internal List<EntityStack> EntitiesInArea(AxisAlignedBoundingBox collectArea)
        {
            List<EntityStack> stacksInArea = new List<EntityStack>();
            foreach (EntityStack stack in StackEntities)
            {
                AxisAlignedBoundingBox aabb = stack.AABB;
                aabb.Translate(stack.Position);
                if (aabb.OverLaps(collectArea))
                {
                    stacksInArea.Add(stack);
                }
            }
            return stacksInArea;
        }
    }
}
