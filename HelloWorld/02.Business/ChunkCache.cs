using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using SlimDX;
using WindowsFormsApplication7.Business.Profiling;

namespace WindowsFormsApplication7.Business
{
    class ChunkCache
    {
        private Dictionary<object, Chunk> cachedChunks = new Dictionary<object, Chunk>();
        public PositionChunk LastMinChunk = new PositionChunk();
        public PositionChunk LastMaxChunk = new PositionChunk();

        public ChunkCache()
        {

        }

        internal void Update(Vector3 position, float blockRadius)
        {
            Profiler p = Profiler.Instance;
            p.StartSection("setup");
            PositionChunk minChunk = PositionChunk.CreateFrom(Vector3.Add(position, new Vector3(-blockRadius, -blockRadius, -blockRadius)));
            PositionChunk maxChunk = PositionChunk.CreateFrom(Vector3.Add(position, new Vector3(blockRadius, blockRadius, blockRadius)));

            // limit y...
            if (minChunk.Y < 0) minChunk.Y = 0;
            if (maxChunk.Y >= Chunk.MaxSizeY / 16) maxChunk.Y = Chunk.MaxSizeY / 16 - 1;

            // skip updating cache if nothing has changed
            /*
            if (LastMinChunk.Equals(minChunk) && LastMaxChunk.Equals(maxChunk))
                return;
            
            */
            LastMinChunk = minChunk;
            LastMaxChunk = maxChunk;

            p.EndStartSection("loop");
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
                        chunk.Update();
                        // relocate entities
                        foreach (EntityStack stack in chunk.StackEntities.ToArray())
                        {
                            PositionChunk destChunk = PositionChunk.CreateFrom(stack.Position);
                            if (!chunk.Position.SameAs(destChunk))
                            {
                                // relocate
                                chunk.StackEntities.Remove(stack);
                                if (cachedChunks.ContainsKey(destChunk.Key))
                                {
                                    cachedChunks[destChunk.Key].StackEntities.Add(stack);
                                }
                            }
                            
                        }
                    }
                }
            }
            p.EndStartSection("dispose");
            // dispose and remove expired chunks
            var expiredChunks = cachedChunks.Where(pair => pair.Value.Expired).ToList();
            expiredChunks.ForEach(pair => 
            { 
                pair.Value.Dipose(); 
                cachedChunks.Remove(pair.Key); 
            });
            p.EndSection();
        }

        internal Chunk GetChunk(PositionChunk positionChunk)
        {
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
