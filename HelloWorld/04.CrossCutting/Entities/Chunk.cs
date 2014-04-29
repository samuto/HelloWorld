using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using SlimDX;
using WindowsFormsApplication7.Business.Geometry;
using System.Diagnostics;
using WindowsFormsApplication7.CrossCutting.Entities.Blocks;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Chunk
    {
        public bool HeavyTaskAllowed;
        public bool HeavyTaskExecuted;
        public const int timeout = 30 * 1000;
        public const int MaxSizeY = 128;
        public PositionChunk Position;
        public bool IsDirty = true;
        private byte[] blocks = new byte[16 * 16 * 16];
        private Dictionary<int, Dictionary<string, object>> chunkMetaData = new Dictionary<int, Dictionary<string, object>>();
        private List<EntityStack> stackEntities = new List<EntityStack>();
        private List<Entity> blockEntityFullUpdate = new List<Entity>();
        private Stopwatch stopwatch = new Stopwatch();
        public ChunkColumn Column;

        public enum ChunkStageEnum
        {
            NotGenerated,
            Generated,
            Update
        }
        public ChunkStageEnum Stage;

        public Chunk()
        {
            stopwatch.Start();
            Stage = ChunkStageEnum.NotGenerated;
        }

        private void RenewLease()
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

        public void SetLocalBlock(int x, int y, int z, int blockId, bool registerBlock = true)
        {
            Block block = Block.FromId(blockId);
            if (registerBlock)
            {
                Entity entity = block.CreateEntity();
                if (entity != null)
                {
                    entity.Parent = this;
                    entity.BlockPosition = new PositionBlock(x, y, z);
                    entity.AddToParent();
                    entity.OnInitialize();
                }
            }
            if (blockId == 0)
            {
                RemoveMetaData(new PositionBlock(x, y, z));
            }
            blocks[x * 16 * 16 + y * 16 + z] = (byte)blockId;
            Invalidate();
        }

        public void Invalidate()
        {
            // invalidates all pass'es
            IsDirty = true;
        }


        int test = 0;
        public void Update()
        {
            HeavyTaskExecuted = false;
            RenewLease();
            if (Stage == ChunkStageEnum.Update)
            {
                UpdateLogic();
            }
            else if (Column.Stage == ChunkColumn.ColumnStageEnum.AllNeighborsGenerated)
            {
                if (HeavyTaskAllowed)
                {
                    Column.Decorate();
                    HeavyTaskExecuted = true;
                }
            }
            else if (Stage == ChunkStageEnum.NotGenerated)
            {
                if (HeavyTaskAllowed)
                {
                    World.Instance.Generator(this).Generate(this);
                    Stage = ChunkStageEnum.Generated;
                    Column.OnChunkGenerated();
                    HeavyTaskExecuted = true;
                }
            }
           

        }

        private void UpdateLogic()
        {
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
                block.UpdateBlock(this, new PositionBlock(x, y, z));
            }

            for (int i = 0; i < 64; i++)
            {
                test = (test + 3) % (16 * 16 * 16);
                if (blocks[test] == BlockRepository.Water.Id)
                {
                    int x = (test >> 8) & 0x0f;
                    int y = (test >> 4) & 0x0f;
                    int z = test & 0x0f;
                    Block block = Block.FromId(GetLocalBlock(x, y, z));
                    block.UpdateBlock(this, new PositionBlock(x, y, z));
                    break;
                }
            }
        }

        

        public void InvalidateMeAndNeighbors()
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (Position.Y + dy <= 0)
                            break;
                        else if (Position.Y + dy >= Chunk.MaxSizeY / 16f - 1)
                            break;
                        World.Instance.GetChunk(new PositionChunk(Position.X + dx, Position.Y + dy, Position.Z + dz)).Invalidate();
                    }
                }
            }
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
            return blockEntityFullUpdate.Where(e => e.BlockPosition.SameAs(localPos)).FirstOrDefault();
        }

        public IEnumerable<EntityStack> StackEntities { get { return (IEnumerable<EntityStack>)stackEntities; } }

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

        internal object GetBlockMetaData(PositionBlock pos, string variable)
        {
            int x = pos.X;
            int y = pos.Y;
            int z = pos.Z;
            if (x < 0 || x >= 16 ||
                y < 0 || y >= 16 ||
                z < 0 || z >= 16)
            {
                PositionBlock globalPosition;
                Position.GetGlobalPositionBlock(out globalPosition, x, y, z);
                return World.Instance.GetBlockMetaData(globalPosition, variable);
            }
            return RawGetBlockMetaData(pos, variable);
        }

        private object RawGetBlockMetaData(PositionBlock positionBlock, string variable)
        {
            int key = positionBlock.X * 16 * 16 + positionBlock.Y * 16 + positionBlock.Z;
            if (chunkMetaData.ContainsKey(key))
                return chunkMetaData[key][variable];
            return null;
        }

        internal void SetBlockMetaData(PositionBlock pos, string variable, object value)
        {
            int x = pos.X;
            int y = pos.Y;
            int z = pos.Z;
            if (x < 0 || x >= 16 ||
                y < 0 || y >= 16 ||
                z < 0 || z >= 16)
            {
                PositionBlock globalPosition;
                Position.GetGlobalPositionBlock(out globalPosition, x, y, z);
                World.Instance.SetBlockMetaData(globalPosition, variable, value);
                return;
            }
            RawSetBlockMetaData(pos, variable, value);
        }

        private void RawSetBlockMetaData(PositionBlock positionBlock, string variable, object value)
        {
            int key = positionBlock.X * 16 * 16 + positionBlock.Y * 16 + positionBlock.Z;
            if (!chunkMetaData.ContainsKey(key))
            {
                chunkMetaData.Add(key, new Dictionary<string, object>());
            }
            chunkMetaData[key][variable] = value;
        }


        internal Entity GetBlockEntityFromPosition(PositionBlock pos)
        {
            int x = pos.X;
            int y = pos.Y;
            int z = pos.Z;
            if (x < 0 || x >= 16 ||
               y < 0 || y >= 16 ||
               z < 0 || z >= 16)
            {
                PositionBlock globalPosition;
                Position.GetGlobalPositionBlock(out globalPosition, x, y, z);
                return World.Instance.GetBlockEntityFromPosition(globalPosition);
            }
            return RawGetBlockEntityFromPosition(x, y, z);

        }

        private Entity RawGetBlockEntityFromPosition(int x, int y, int z)
        {
            Block block = Block.FromId(GetLocalBlock(x, y, z));
            Entity entity = block.CreateEntity();
            if (entity != null)
            {
                entity.Parent = this;
                entity.BlockPosition = new PositionBlock(x, y, z);
            }
            return entity;
        }
    }
}
