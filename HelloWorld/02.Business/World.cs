using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Landscape;
using WindowsFormsApplication7.DataAccess;
using WindowsFormsApplication7.Business.Profiling;
using SlimDX;
using WindowsFormsApplication7.Frontend;

namespace WindowsFormsApplication7.Business
{
    class World
    {
        public static World Instance = new World();

        // entities
        public static float TicksPrDay = 5000;
        public List<Entity> globalEntities = new List<Entity>();
        public Player Player;
        public Entity entityToControl;
        private ChunkCache chunkCache;
        private ChunkStorage storage;
        private GeneratorBase generator;
        public VoxelTrace PlayerVoxelTrace = new VoxelTrace();
        private Profiler p = Profiler.Instance;
        
        internal void Update()
        {
            p.StartSection("ChunkCache");
            // update landscape
            chunkCache.Update(Player.Position);

            // update entities
            p.EndStartSection("Entities");
            foreach (Entity entity in globalEntities)
            {
                entity.OnUpdate();
            }
            p.EndSection();

            // calculate voxel ray
            Vector3 direction = Player.Direction;
            Vector3 eye = Vector3.Add(Player.EyePosition, Player.Position);
            PlayerVoxelTrace.Update(eye, direction);

        }

        public float TimeOfDay
        {
            get
            {
                return 24f * ((TheGame.Instance.CurrentTick + TicksPrDay / 4f) % TicksPrDay) / TicksPrDay;
            }
        }

        internal ChunkCache GetCachedChunks()
        {
            return chunkCache;
        }

        internal Chunk GetChunk(PositionChunk positionChunk)
        {
            Chunk chunk = storage.GetChunk(positionChunk);
            if (chunk == null)
            {
                chunk = new Chunk();
                chunk.Position = positionChunk;
                storage.AddChunk(chunk);
            }
            return chunk;
        }

        internal int GetBlock(PositionBlock pos)
        {
            if (pos.Y < 0) return 0;
            if (pos.Y >= Chunk.MaxSizeY) return 0;
            PositionChunk positionChunk = PositionChunk.CreateFrom(pos);
            Chunk chunk = GetChunk(positionChunk);
            positionChunk.ConvertToLocalPosition(ref pos);
            int blockId = chunk.GetLocalBlock(pos.X, pos.Y, pos.Z);
            return blockId;
        }

        internal int GetBlock(int x, int y, int z)
        {
            PositionBlock pos = new PositionBlock(x, y, z);
            return GetBlock(pos);
        }

        internal void SetBlock(PositionBlock pos, int blockId)
        {
            PositionChunk positionChunk = PositionChunk.CreateFrom(pos);
            Chunk chunk = GetChunk(positionChunk);
            positionChunk.ConvertToLocalPosition(ref pos);
            chunk.SetLocalBlock(pos.X, pos.Y, pos.Z, blockId);
        }

        internal void SetBlock(int x, int y, int z, int blockId)
        {
            PositionBlock pos = new PositionBlock(x, y, z);
            SetBlock(pos, blockId);
        }

        internal bool ReplaceBlock(PositionBlock pos, int oldId, int newId)
        {
            if (GetBlock(pos) == oldId)
            {
                SetBlock(pos, newId);
                return true;
            }
            return false;
        }

        internal void SpawnStack(EntityStack stack)
        {
            PositionChunk positionChunk = PositionChunk.CreateFrom(stack.Position);
            Chunk chunk = GetChunk(positionChunk);
            chunk.AddEntity(stack);
        }



        internal Entity GetBlockEntity(PositionBlock position)
        {
            PositionChunk positionChunk = PositionChunk.CreateFrom(position);
            Chunk chunk = GetChunk(positionChunk);
            positionChunk.ConvertToLocalPosition(ref position);
            Entity entity = chunk.GetBlockEntity(position);
            return entity;
        }



        internal object GetBlockMetaData(PositionBlock pos, string variable)
        {
            if (pos.Y < 0) return null;
            if (pos.Y >= Chunk.MaxSizeY) return null;
            PositionChunk positionChunk = PositionChunk.CreateFrom(pos);
            Chunk chunk = GetChunk(positionChunk);
            positionChunk.ConvertToLocalPosition(ref pos);
            return chunk.GetBlockMetaData(pos, variable);
        }

        

        internal void SetBlockMetaData(PositionBlock pos, string variable, object value)
        {
            if (pos.Y < 0) return;
            if (pos.Y >= Chunk.MaxSizeY) return;
            PositionChunk positionChunk = PositionChunk.CreateFrom(pos);
            Chunk chunk = GetChunk(positionChunk);
            positionChunk.ConvertToLocalPosition(ref pos);
            chunk.SetBlockMetaData(pos, variable, value);
        }

        internal Entity GetBlockEntityFromPosition(PositionBlock pos)
        {
            if (pos.Y < 0) return null;
            if (pos.Y >= Chunk.MaxSizeY) return null;
            PositionChunk positionChunk = PositionChunk.CreateFrom(pos);
            Chunk chunk = GetChunk(positionChunk);
            positionChunk.ConvertToLocalPosition(ref pos);
            return chunk.GetBlockEntityFromPosition(pos);
        }

        internal void Initialize(WorldConfiguration config)
        {
            chunkCache = new ChunkCache();
            storage = new ChunkStorage();
            generator = config.Generator;
            Player = new Player();
            Player.PrevPosition = World.Instance.Player.Position = new Vector3(0, 100, -20);
            entityToControl = Player;
            globalEntities.Add(new Sun());
            globalEntities.Add(new Moon());
            globalEntities.Add(Player);
        }


        internal GeneratorBase Generator(Chunk chunk)
        {
            return generator;
        }
    }
}
