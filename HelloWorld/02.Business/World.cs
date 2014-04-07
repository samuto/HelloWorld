using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Landscape;
using WindowsFormsApplication7.DataAccess;
using WindowsFormsApplication7.Business.Profiling;
using SlimDX;

namespace WindowsFormsApplication7.Business
{
    class World
    {
        public static World Instance = new World();

        // entities
        public static float TicksPrDay = 5000;
        public Player Player = new Player();
        public FlyingCamera FlyingCamera = new FlyingCamera();
        public Sun Sun = new Sun();
        public Moon Moon = new Moon();
        private ChunkCache chunkCache = new ChunkCache();
        private ChunkStorage storage = new ChunkStorage();
        private ChunkGenerator generator = new ChunkGenerator();

        public VoxelTrace PlayerVoxelTrace = new VoxelTrace();

        internal void Update()
        {
            Profiler p = Profiler.Instance;
            p.StartSection("ChunkCache");
            // update landscape
            chunkCache.Update(Player.Position, GameSettings.CachingRadius);

            // update entities
            p.EndStartSection("Entities"); 
            Player.Update();
            FlyingCamera.Update();
            Sun.Update();
            Moon.Update();

            // calculate voxel ray
            p.EndStartSection("voxelray");
            Vector3 direction = Player.Direction;
            Vector3 eye = Vector3.Add(Player.EyePosition, Player.Position);
            PlayerVoxelTrace.Update(eye, direction);
            p.EndSection();

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

        internal void Generate(Chunk chunk)
        {
            generator.Generate(chunk);
        }

      
        internal int GetBlock(int x, int y, int z)
        {
            PositionBlock pos = new PositionBlock(x, y, z);
            PositionChunk positionChunk = PositionChunk.CreateFrom(pos);
            Chunk chunk = GetChunk(positionChunk);
            positionChunk.ConvertToLocalPosition(ref pos);
            int blockId = chunk.GetLocalBlock(pos.X, pos.Y, pos.Z);
            return blockId;
        }

        internal void SetBlock(int x, int y, int z, int blockId)
        {
            PositionBlock pos = new PositionBlock(x, y, z);
            PositionChunk positionChunk = PositionChunk.CreateFrom(pos);
            Chunk chunk = GetChunk(positionChunk);
            positionChunk.ConvertToLocalPosition(ref pos);
            chunk.SetLocalBlock(pos.X, pos.Y, pos.Z, blockId);
        }

        internal bool ReplaceBlock(PositionBlock pos, int oldId, int newId)
        {
            if (GetBlock(pos.X, pos.Y, pos.Z) == oldId)
            {
                SetBlock(pos.X, pos.Y, pos.Z, newId);
                return true;
            }
            return false;
        }

        internal void SpawnStack(EntityStack stack)
        {
            PositionChunk positionChunk = PositionChunk.CreateFrom(stack.Position);
            Chunk chunk = GetChunk(positionChunk);
            chunk.StackEntities.Add(stack);
        }
    }
}
