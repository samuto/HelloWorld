using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using System.Diagnostics;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Profiling;
using SlimDX;
using WindowsFormsApplication7.Business.Geometry;

namespace WindowsFormsApplication7.Frontend
{
    class ChunkCacheRenderer
    {
        private Dictionary<object, ChunkRenderer> chunkRenderers = new Dictionary<object, ChunkRenderer>();
        private List<ChunkRenderer> pass2ChunkRenderers = new List<ChunkRenderer>();
        private ChunkCache cachedChunks;
        private int centerX;
        private int centerY;
        private int centerZ;
        private bool forceCachedRendering;
        private int viewRadius;
        private Tessellator t = Tessellator.Instance;
        private Profiler p = Profiler.Instance;

        internal void Render()
        {
            p.StartSection("pass1");
            RenderPass1AndBuildPass2List();
            p.EndStartSection("pass2");
            RenderPass2();
            p.EndSection();
            
        }

        private void RenderPass2()
        {
            t.ResetTransformation();
            foreach (ChunkRenderer pass2ChunkRenderer in pass2ChunkRenderers)
            {
                pass2ChunkRenderer.RenderPass2();
            }
        }

        private void RenderPass1AndBuildPass2List()
        {
            t.ResetTransformation();
            pass2ChunkRenderers.Clear();
            cachedChunks = World.Instance.GetCachedChunks();
            Counters.Instance.SetValue("total cached", cachedChunks.Count);

            if (cachedChunks.Count == 0)
                return;
            PositionChunk minChunk = cachedChunks.LastMinChunk;
            PositionChunk maxChunk = cachedChunks.LastMaxChunk;

            centerX = MathLibrary.FloorToWorldGrid((maxChunk.X - minChunk.X) / 2f + minChunk.X);
            centerY = MathLibrary.FloorToWorldGrid((maxChunk.Y - minChunk.Y) / 2f + minChunk.Y);
            centerZ = MathLibrary.FloorToWorldGrid((maxChunk.Z - minChunk.Z) / 2f + minChunk.Z);
            viewRadius = (int)(GameSettings.ViewRadius / 16 - 1);
            forceCachedRendering = false;

            p.StartSection("renderchunks");

            int x, y, z;
            x = centerX;
            y = centerY;
            z = centerZ;

            RenderChunk(x, y, z);
            for (int r = 1; r <= viewRadius; r++)
            {
                // front+back
                for (int i = -r; i <= r; i++)
                {
                    for (int j = -r; j <= r; j++)
                    {
                        RenderChunk(x + i, y + j, z + r);
                        RenderChunk(x + i, y + j, z - r);
                    }
                }
                // left+right
                for (int i = -r; i <= r; i++)
                {
                    for (int j = -r + 1; j <= r - 1; j++)
                    {
                        RenderChunk(x - r, y + i, z + j);
                        RenderChunk(x + r, y + i, z + j);
                    }
                }
                // top+bottom
                for (int i = -r + 1; i <= r - 1; i++)
                {
                    for (int j = -r + 1; j <= r - 1; j++)
                    {
                        RenderChunk(x + i, y - r, z + j);
                        RenderChunk(x + i, y + r, z + j);
                    }
                }
            }
            p.EndStartSection("dispose");


            // dispose and remove expired chunks
            var expiredRenderer = chunkRenderers.Where(pair => pair.Value.Expired).ToList();
            expiredRenderer.ForEach(pair =>
            {
                pair.Value.Dispose();
                chunkRenderers.Remove(pair.Key);
            });
            Counters.Instance.SetValue("ChunkRenderer objects", chunkRenderers.Count);
            p.EndSection();

        }

        private void RenderChunk(int x, int y, int z)
        {
            if (y < 0)
                return;
            if (y >= Chunk.MaxSizeY / 16)
                return;
            int deltaX = x - centerX;
            int deltaY = y - centerY;
            int deltaZ = z - centerZ;
                        
            // skip if chunk is out of view range
            if ((deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ) > viewRadius * viewRadius) return;
            
            Counters.Instance.Increment("chunks in view range");
            Chunk chunk = cachedChunks.GetChunk(new PositionChunk(x, y, z));
            if (!ChunkRenderer.InsideViewFrustum(chunk)) return;
            Counters.Instance.Increment("chunks rendered");
            
            // get chunkrenderer for this chunk (create new of it does not exist)
            object key = chunk.Position.Key;
            ChunkRenderer chunkRenderer;
            if (chunkRenderers.ContainsKey(key))
                chunkRenderer = chunkRenderers[key];
            else
            {
                chunkRenderer = new ChunkRenderer(chunk);
                chunkRenderers.Add(key, chunkRenderer);
            }
            forceCachedRendering |= chunkRenderer.Render(forceCachedRendering);
            pass2ChunkRenderers.Add(chunkRenderer);
        }
    }
}
