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
        private Profiler p = Profiler.Instance;

        public ChunkCache()
        {

        }

        internal void Update(Vector3 position, float blockRadius)
        {
            p.StartSection("setup");

            Vector3 minPos = Vector3.Add(position, new Vector3(-blockRadius, -blockRadius, -blockRadius));
            Vector3 maxPos = Vector3.Add(position, new Vector3(blockRadius, blockRadius, blockRadius));
            if (minPos.Y < 0) minPos.Y = 0;
            if (minPos.Y > (Chunk.MaxSizeY - 1)) minPos.Y = Chunk.MaxSizeY - 1;
            if (maxPos.Y < 0) maxPos.Y = 0;
            if (maxPos.Y > (Chunk.MaxSizeY - 1)) maxPos.Y = Chunk.MaxSizeY - 1;
            PositionChunk minChunk = PositionChunk.CreateFrom(minPos);
            PositionChunk maxChunk = PositionChunk.CreateFrom(maxPos);

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
                        p.StartSection("maintain");
                        Chunk chunk = World.Instance.GetChunk(new PositionChunk(x, y, z));
                        chunk.RenewLease();
                        p.EndStartSection("updatecahce");
                        if (!cachedChunks.ContainsKey(chunk.Position.Key))
                        {
                            cachedChunks.Add(chunk.Position.Key, chunk);
                        }
                        p.EndStartSection("updating");
                       
                        chunk.Update();


                        p.EndStartSection("relocating");
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
                        p.EndSection();
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
