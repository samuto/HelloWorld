using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using SlimDX;
using WindowsFormsApplication7.Business.Geometry;
using System.Diagnostics;
using WindowsFormsApplication7.CrossCutting.Entities.Blocks;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Chunk
    {
        public const int timeout = 30 * 1000;
        public const int MaxSizeY = 128;
        public PositionChunk Position;
        private byte[] blocks = new byte[16 * 16 * 16];
        private Dictionary<int, Dictionary<string, object>> chunkMetaData = new Dictionary<int, Dictionary<string, object>>();
        public bool IsDirty = true;
        private bool initialized;
        private List<EntityStack> stackEntities = new List<EntityStack>();
        private List<Entity> blockEntityFullUpdate = new List<Entity>();
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

        public void SetLocalBlock(int x, int y, int z, int blockId, bool registerBlock=true)
        {
            Block block = Block.FromId(blockId);
            if (registerBlock)
            {
                Entity entity = block.CreateEntity();
                if (entity != null)
                {
                    entity.Parent = this;
                    entity.AddToParent();
                    entity.PositionBlock = new PositionBlock(x, y, z);
                }
            }
            if (blockId == 0)
            {
                RemoveMetaData(new PositionBlock(x, y, z));
            }
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
            foreach (EntityStack stack in stackEntities)
            {
                stack.OnUpdate();
            }
            foreach (Entity blockEntity in blockEntityFullUpdate)
            {
                blockEntity.OnUpdate();
            }
            for (int i = 0; i < 3; i++)
            {
                int x = MathLibrary.GlobalRandom.Next(0, 16);
                int y = MathLibrary.GlobalRandom.Next(0, 16);
                int z = MathLibrary.GlobalRandom.Next(0, 16);
                Block block = Block.FromId(GetLocalBlock(x, y, z));
                Entity entity = block.CreateEntity();
                if (entity != null)
                {
                    entity.Parent = this;
                    entity.PositionBlock = new PositionBlock(x, y, z);
                    entity.OnUpdate();
                }
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
                World.Instance.SetBlock(globalPosition, blockId);
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
                return World.Instance.GetBlock(globalPosition);
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
            foreach (EntityStack stack in stackEntities)
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

        internal Entity GetBlockEntity(PositionBlock localPos)
        {
            return blockEntityFullUpdate.Where(e => e.PositionBlock.SameAs(localPos)).FirstOrDefault();
        }

        public IEnumerable<EntityStack> StackEntities { get { return (IEnumerable<EntityStack>)stackEntities; } }

        internal Dictionary<string, object> GetBlockMetaData(PositionBlock positionBlock)
        {
            int key = positionBlock.X * 16 * 16 + positionBlock.Y * 16 + positionBlock.Z;
            if(chunkMetaData.ContainsKey(key))
                return chunkMetaData[key];
            Dictionary<string, object> blockMetaData = new Dictionary<string,object>();
            chunkMetaData.Add(key, blockMetaData);
            return blockMetaData;
        }

        internal int MetaDataGetInt(string key, PositionBlock positionBlock)
        {
            var blockMetaData = GetBlockMetaData(positionBlock);
            if (blockMetaData.ContainsKey(key))
                return (int)(blockMetaData[key]);
            blockMetaData[key] = 0;
            return 0;
        }

        internal void AddEntity(Entity entity)
        {
            switch (entity.EntityType)
            {
                case Entity.EntityTypeEnum.BlockFullUpdate:
                    blockEntityFullUpdate.Add(entity);
                    break;
               
                case Entity.EntityTypeEnum.EntityStackFullUpdate:
                    stackEntities.Add((EntityStack)entity);
                    break;
            }
        }

        internal void RemoveEntity(Entity entity)
        {
            switch (entity.EntityType)
            {
                case Entity.EntityTypeEnum.BlockFullUpdate:
                    blockEntityFullUpdate.Remove(entity);
                    break;
                case Entity.EntityTypeEnum.EntityStackFullUpdate:
                    stackEntities.Remove((EntityStack)entity);
                    break;
            }

        }

        internal void RemoveMetaData(PositionBlock localPos)
        {
            int key = localPos.X * 16 * 16 + localPos.Y * 16 + localPos.Z;
            if (chunkMetaData.ContainsKey(key))
                chunkMetaData.Remove(key);
        }
    }
}
