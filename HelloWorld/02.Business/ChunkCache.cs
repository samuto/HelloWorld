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
        public PositionChunk LastCenterChunk = new PositionChunk();
        public List<Chunk> OrderedChunks;
        private Dictionary<object, Chunk> cachedChunks = new Dictionary<object, Chunk>();
        private Profiler p = Profiler.Instance;
        public bool IsDirty;

        public ChunkCache()
        {

        }

        internal void Update(Vector3 centerPos, float blockRadius)
        {
            float chunkUpdateDistSquared = (int)((blockRadius * blockRadius) / 256f);
            Vector3 centerBlockPos = new Vector3(
                MathLibrary.FloorToWorldGrid(centerPos.X),
                MathLibrary.FloorToWorldGrid(centerPos.Y),
                MathLibrary.FloorToWorldGrid(centerPos.Z));
            // cap it to world
            if (centerBlockPos.Y > Chunk.MaxSizeY - 1)
                centerBlockPos.Y = Chunk.MaxSizeY - 1;
            else if (centerBlockPos.Y < 0)
                centerBlockPos.Y = 0;
            Vector3 minPos = Vector3.Add(centerPos, new Vector3(-blockRadius, 0, -blockRadius));
            Vector3 maxPos = Vector3.Add(centerPos, new Vector3(blockRadius, 0, blockRadius));
            minPos.Y = 0;
            maxPos.Y = Chunk.MaxSizeY - 1;
            PositionChunk minChunk = PositionChunk.CreateFrom(minPos);
            PositionChunk maxChunk = PositionChunk.CreateFrom(maxPos);
            PositionChunk centerChunk = PositionChunk.CreateFrom(centerBlockPos);

            // add all chunks within blockradius
            if (!centerChunk.SameAs(LastCenterChunk))
            {
                // update the cache
                for (int x = minChunk.X; x <= maxChunk.X; x++)
                {
                    for (int z = minChunk.Z; z <= maxChunk.Z; z++)
                    {
                        int dx = x - centerChunk.X;
                        int dz = z - centerChunk.Z;
                        double chunkCurrentDistSquared = dx * dx + dz * dz;
                        if (chunkCurrentDistSquared > chunkUpdateDistSquared)
                            continue;
                        for (int y = minChunk.Y; y <= maxChunk.Y; y++)
                        {
                            Chunk chunk = World.Instance.GetChunk(new PositionChunk(x, y, z));
                            if (!cachedChunks.ContainsKey(chunk.Position.Key))
                            {
                                cachedChunks.Add(chunk.Position.Key, chunk);
                            }
                        }
                    }
                }
                OrderedChunks = cachedChunks.Values.Where(c =>
                {
                    float dx = c.Position.X - centerChunk.X;
                    float dz = c.Position.Z - centerChunk.Z;
                    return dx * dx + dz * dz <= chunkUpdateDistSquared;
                }).OrderBy(c =>
                {
                    float dx = c.Position.X - centerChunk.X;
                    float dy = c.Position.Y - centerChunk.Y;
                    float dz = c.Position.Z - centerChunk.Z;
                    return dx * dx + dy * dy + dz * dz;
                }).ToList();

                IsDirty = true;
            }

            // update chunks...
            bool allowHeavyTask = true;
            foreach (Chunk chunk in OrderedChunks)
            {
                chunk.HeavyTaskAllowed = allowHeavyTask;
                chunk.Update();
                if (chunk.HeavyTaskExecuted)
                    allowHeavyTask = false;
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

            LastCenterChunk = centerChunk;
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
