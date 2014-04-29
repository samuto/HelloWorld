using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using SlimDX;
using WindowsFormsApplication7.Business.Profiling;
using WindowsFormsApplication7.Business.Geometry;
using WindowsFormsApplication7.CrossCutting;

namespace WindowsFormsApplication7.Business
{
    class ChunkCache
    {
        public PositionChunk LastCenterChunk = new PositionChunk();
        public List<Chunk> OrderedChunks;
        private Dictionary<object, Chunk> cachedChunks = new Dictionary<object, Chunk>();
        public Dictionary<object, ChunkColumn> ChunkColumns = new Dictionary<object, ChunkColumn>();

        private Profiler p = Profiler.Instance;
        public bool IsDirty;

        public ChunkCache()
        {
        }

        internal void Update(Vector3 centerPos)
        {

            float chunkRadius = (int)((GameSettings.CachingRadius) / 16f);
            float chunkRadiusSquared = chunkRadius * chunkRadius;
            Vector3 centerBlockPos = new Vector3(
                MathLibrary.FloorToWorldGrid(centerPos.X),
                MathLibrary.FloorToWorldGrid(centerPos.Y),
                MathLibrary.FloorToWorldGrid(centerPos.Z));
            if (centerBlockPos.Y > Chunk.MaxSizeY - 1)
                centerBlockPos.Y = Chunk.MaxSizeY - 1;
            else if (centerBlockPos.Y < 0)
                centerBlockPos.Y = 0;
            PositionChunk centerChunk = PositionChunk.CreateFrom(centerBlockPos);
            PositionChunk minChunk = new PositionChunk(centerChunk.X - (int)chunkRadius, 0, centerChunk.Z - (int)chunkRadius);
            PositionChunk maxChunk = new PositionChunk(centerChunk.X + (int)chunkRadius, Chunk.MaxSizeY / 16 - 1, centerChunk.Z + (int)chunkRadius);

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
                        if (chunkCurrentDistSquared > chunkRadiusSquared)
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
                    return dx * dx + dz * dz <= chunkRadiusSquared;
                }).OrderBy(c =>
                {
                    float dx = c.Position.X - centerChunk.X;
                    float dz = c.Position.Z - centerChunk.Z;
                    return dx * dx + dz * dz;
                }).ThenBy(c =>
                {
                    float dy = c.Position.Y - centerChunk.Y;
                    return dy * dy;
                }).ToList();

                IsDirty = true;
            }

            // update chunks...
            bool allowHeavyTask = true;
            foreach (var column in ChunkColumns.Values)
            {
                column.Active = false;
            }
            foreach (Chunk chunk in OrderedChunks)
            {
                chunk.Column = GetChunkColumn(chunk.Position);
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
            // remove unused columnds
            var columnsToRemove = ChunkColumns.Where(pair => pair.Value.Active == false).ToList();
            columnsToRemove.ForEach(pair =>
            {
                ChunkColumns.Remove(pair.Key);
            });

            LastCenterChunk = centerChunk;
        }

        private ChunkColumn GetChunkColumn(PositionChunk chunkPos)
        {
            PositionChunk columnPos = new PositionChunk(chunkPos.X, 0, chunkPos.Z);
            ChunkColumn column;
            if (ChunkColumns.ContainsKey(columnPos.Key))
            {
                column = ChunkColumns[columnPos.Key];
            }
            else
            {
                column = new ChunkColumn(columnPos.X, columnPos.Z);
                ChunkColumns.Add(columnPos.Key, column);
                column.InitializeStage();
            }
            column.Active = true;
            return column;
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

        internal List<ChunkColumn> AllNeighborColumns(ChunkColumn chunkColumn)
        {
            Vector2[] deltas = new Vector2[] { 
                new Vector2(-1,-1),
                new Vector2(0,-1),
                new Vector2(1,-1),
                new Vector2(-1,0),
                new Vector2(1,0),
                new Vector2(-1,1),
                new Vector2(0,1),
                new Vector2(1,1),
            };
            List<ChunkColumn> neighbors = new List<ChunkColumn>();
            foreach (Vector2 delta in deltas)
            {
                PositionChunk chunkPos = new PositionChunk(chunkColumn.Position.X + (int)delta.X, 0, chunkColumn.Position.Z + (int)delta.Y);
                if (ChunkColumns.ContainsKey(chunkPos.Key))
                    neighbors.Add(ChunkColumns[chunkPos.Key]);
                else
                    neighbors.Add(null);

            }
            return neighbors;
        }
    }
}
