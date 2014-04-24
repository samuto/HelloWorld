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
    class ChunkCache
    {
        private Dictionary<object, Chunk> cachedChunks = new Dictionary<object, Chunk>();
        public PositionChunk LastMinChunk = new PositionChunk();
        public PositionChunk LastMaxChunk = new PositionChunk();
        private Profiler p = Profiler.Instance;

        public ChunkCache()
        {

        }

        internal void Update(Vector3 position, float blockRadius)
        {
            Vector3 minPos = Vector3.Add(position, new Vector3(-blockRadius, -blockRadius, -blockRadius));
            Vector3 maxPos = Vector3.Add(position, new Vector3(blockRadius, blockRadius, blockRadius));
            if (minPos.Y < 0) minPos.Y = 0;
            if (minPos.Y > (Chunk.MaxSizeY - 1)) minPos.Y = Chunk.MaxSizeY - 1;
            if (maxPos.Y < 0) maxPos.Y = 0;
            if (maxPos.Y > (Chunk.MaxSizeY - 1)) maxPos.Y = Chunk.MaxSizeY - 1;
            PositionChunk minChunk = PositionChunk.CreateFrom(minPos);
            PositionChunk maxChunk = PositionChunk.CreateFrom(maxPos);

            // skip updating cache if nothing has changed
            LastMinChunk = minChunk;
            LastMaxChunk = maxChunk;

            // set chunks to be expired
            for (int x = minChunk.X; x <= maxChunk.X; x++)
            {
                for (int y = minChunk.Y; y <= maxChunk.Y; y++)
                {
                    for (int z = minChunk.Z; z <= maxChunk.Z; z++)
                    {
                        Chunk chunk = World.Instance.GetChunk(new PositionChunk(x, y, z));
                        chunk.RenewLease();
                        if (!cachedChunks.ContainsKey(chunk.Position.Key))
                        {
                            cachedChunks.Add(chunk.Position.Key, chunk);
                        }
                    }
                }
            }

            // update chunks...
            Vector3 playerPos = World.Instance.Player.Position;
            List<Chunk> orderedChunks = cachedChunks.Values.OrderBy(c =>
            {
                float dx = c.Position.X - MathLibrary.FloorToWorldGrid(playerPos.X / 16f);
                float dz = c.Position.Z - MathLibrary.FloorToWorldGrid(playerPos.Z / 16f);
                return dx * dx + dz * dz;
            }).ThenBy(c =>
            {
                float dy = c.Position.Y - MathLibrary.FloorToWorldGrid(playerPos.Y / 16f);
                return dy * dy;
            }).ToList();
            bool allowHeavyTask = true;
            foreach (Chunk chunk in orderedChunks)
            {
                allowHeavyTask = chunk.Update(allowHeavyTask);
                // relocate entities
                foreach (EntityStack stack in chunk.StackEntities.ToArray())
                {
                    PositionChunk destChunk = PositionChunk.CreateFrom(stack.Position);
                    if (!chunk.Position.SameAs(destChunk))
                    {
                        // relocate
                        chunk.RemoveEntity(stack);
                        if (cachedChunks.ContainsKey(destChunk.Key))
                        {
                            cachedChunks[destChunk.Key].AddEntity(stack);
                        }
                    }
                }
            }

            // dispose and remove expired chunks
            var expiredChunks = cachedChunks.Where(pair => pair.Value.Expired).ToList();
            expiredChunks.ForEach(pair =>
            {
                pair.Value.Dipose();
                cachedChunks.Remove(pair.Key);
            });
        }

        internal Chunk GetChunk(PositionChunk positionChunk)
        {
            if (!cachedChunks.ContainsKey(positionChunk.Key))
                return null;
            return cachedChunks[positionChunk.Key];
        }

        public int Count
        {
            get
            {
                return cachedChunks.Count;
            }
        }
    }
}
