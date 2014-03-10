using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Landscape;
using WindowsFormsApplication7.DataAccess;

namespace WindowsFormsApplication7.Business
{
    class World
    {
        public static World Instance = new World();

        // entities
        public Player Player = new Player();
        public FlyingCamera FlyingCamera = new FlyingCamera();

        private ChunkCache chunkCache = new ChunkCache();
        private ChunkStorage storage = new ChunkStorage();
        private ChunkGenerator generator = new ChunkGenerator();

        internal void Update()
        {
            chunkCache.Update(Player.Position, GameSettings.CachingRadius);
            if (GameSettings.EnableEntityUpdate)
            {
                Player.Update();
            }
            FlyingCamera.Update();
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
            PositionBlock positionBlock = new PositionBlock(x, y, z);
            PositionChunk positionChunk = PositionChunk.CreateFrom(positionBlock);
            Chunk chunk = GetChunk(positionChunk);
            return chunk.GetLocalBlock(positionChunk.GetLocalPositionBlock(positionBlock));
        }

        internal void SetBlock(int x, int y, int z, int blockId)
        {
            PositionBlock positionBlock = new PositionBlock(x, y, z);
            PositionChunk positionChunk = PositionChunk.CreateFrom(positionBlock);
            Chunk chunk = GetChunk(positionChunk);
            chunk.SetLocalBlock(positionChunk.GetLocalPositionBlock(positionBlock), blockId);
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
    }
}
