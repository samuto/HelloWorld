using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using System.Diagnostics;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Profiling;
using SlimDX;

namespace WindowsFormsApplication7.Frontend
{
    class ChunkCacheRenderer
    {
        private Dictionary<object, ChunkRenderer> chunkRenderers = new Dictionary<object, ChunkRenderer>();

        internal void Render()
        {
            Profiler p = Profiler.Instance;
            p.StartSection("setuploop");
           
            Vector3 pos = World.Instance.Player.Position;
            ChunkCache cachedChunks = World.Instance.GetCachedChunks();
            PositionChunk minChunk = cachedChunks.LastMinChunk;
            PositionChunk maxChunk = cachedChunks.LastMaxChunk;

            float centerX = (maxChunk.X - minChunk.X) / 2f + minChunk.X;
            float centerZ = (maxChunk.Z - minChunk.Z) / 2f + minChunk.Z;
            float viewRadius = GameSettings.ViewRadius / 16f - 1f;
            
            bool forceCachedRendering = false;
            chunkRenderers.Values.ToList().ForEach(c => c.Expired = true);
            for (int x = minChunk.X; x <= maxChunk.X; x++)
            {
                float deltaX = x - centerX;
                for (int z = minChunk.Z; z <= maxChunk.Z; z++)
                {
                    p.EndStartSection("precheck");
                    // skip if chunk is out of view range
                    float deltaZ = z - centerZ;
                    if ((deltaX * deltaX + deltaZ * deltaZ) > viewRadius * viewRadius)
                        continue;
                    p.EndStartSection("getchunk");
                    // get chunkrenderer for this chunk (create new of it does not exist)
                    Chunk chunk = cachedChunks.GetChunk(new PositionChunk(x, z));
                    object key = chunk.Position.Key;
                    ChunkRenderer chunkRenderer;
                    if (chunkRenderers.ContainsKey(key))
                        chunkRenderer = chunkRenderers[key];
                    else
                    {
                        chunkRenderer = new ChunkRenderer(chunk);
                        chunkRenderers.Add(key, chunkRenderer);
                    }

                    p.EndStartSection("renderchunk");
                    forceCachedRendering |= chunkRenderer.Render(forceCachedRendering);
                    
                    chunkRenderer.Expired = false;
                }
            }
            p.EndStartSection("disposestuff");
            // dispose and remove expired chunks
            List<ChunkRenderer> expired = chunkRenderers.Values.Where(c => c.Expired).ToList();
            expired.ForEach(c => { c.Dispose(); chunkRenderers.Remove(c); });
            p.EndSection();
        }
    }
}
