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
            if (cachedChunks.Count == 0)
                return;
            PositionChunk minChunk = cachedChunks.LastMinChunk;
            PositionChunk maxChunk = cachedChunks.LastMaxChunk;

            float centerX = (maxChunk.X - minChunk.X) / 2f + minChunk.X;
            float centerY = (maxChunk.Y - minChunk.Y) / 2f + minChunk.Y;
            float centerZ = (maxChunk.Z - minChunk.Z) / 2f + minChunk.Z;
            float viewRadius = GameSettings.ViewRadius / 16f - 1f;

            bool forceCachedRendering = false;
            for (int x = minChunk.X; x <= maxChunk.X; x++)
            {
                float deltaX = x - centerX;
                for (int y = minChunk.Y; y <= maxChunk.Y; y++)
                {
                    float deltaY = y - centerY;
                    for (int z = minChunk.Z; z <= maxChunk.Z; z++)
                    {
                        p.EndStartSection("precheck");
                        // skip if chunk is out of view range
                        float deltaZ = z - centerZ;
                        if ((deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ) > viewRadius * viewRadius)
                            continue;
                        p.EndStartSection("getchunk");
                        // get chunkrenderer for this chunk (create new of it does not exist)
                        Chunk chunk = cachedChunks.GetChunk(new PositionChunk(x, y, z));
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
                    }
                }
            }
            p.EndStartSection("disposestuff");
            // dispose and remove expired chunks
            var expiredRenderer = chunkRenderers.FirstOrDefault(pair => pair.Value.Expired);
            if(expiredRenderer.Key != null)
            {
                expiredRenderer.Value.Dispose();
                chunkRenderers.Remove(expiredRenderer.Key);
            }
            Counters.Instance.SetValue("expired", chunkRenderers.Values.Where(c => c.Expired).Count());
            Counters.Instance.SetValue("chunkRenderers", chunkRenderers.Count);
            p.EndSection();
        }
    }
}
