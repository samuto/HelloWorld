﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using SlimDX;

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
            PositionChunk minChunk = PositionChunk.CreateFrom(Vector3.Add(position, new Vector3(-blockRadius, -blockRadius, -blockRadius)));
            PositionChunk maxChunk = PositionChunk.CreateFrom(Vector3.Add(position, new Vector3(blockRadius, blockRadius, blockRadius)));

            // skip updating cache if nothing has changed
            if (LastMinChunk.Equals(minChunk) && LastMaxChunk.Equals(maxChunk))
                return;
            LastMinChunk = minChunk;
            LastMaxChunk = maxChunk;

            // set chunks to be expired
            cachedChunks.Values.ToList().ForEach(c => c.Expired = true);
            for (int x = minChunk.X; x <= maxChunk.X; x++)
            {
                for (int z = minChunk.Z; z <= maxChunk.Z; z++)
                {
                    Chunk chunk = World.Instance.GetChunk(new PositionChunk(x, z));
                    chunk.Expired = false;
                    if (!cachedChunks.ContainsKey(chunk.Position.Key))
                    {
                        cachedChunks.Add(chunk.Position.Key, chunk);
                    }
                    if (!chunk.Initialized)
                    {
                        chunk.Initialize();
                    }
                }
            }
            // dispose and remove expired chunks
            List<Chunk> expiredChunks = cachedChunks.Values.Where(c => c.Expired).ToList();
            expiredChunks.ForEach(c => { c.Dipose(); cachedChunks.Remove(c); });
        }

        internal Chunk GetChunk(PositionChunk positionChunk)
        {
            return cachedChunks[positionChunk.Key];
        }
    }
}