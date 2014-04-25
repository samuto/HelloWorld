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
        private List<Chunk> chunksToRender;
        private Dictionary<object, ChunkRenderer> chunkRenderers = new Dictionary<object, ChunkRenderer>();
        private List<ChunkRenderer> pass2ChunkRenderers = new List<ChunkRenderer>();
        private ChunkCache cachedChunks;
        private bool forceCachedRendering;
        private Tessellator t = Tessellator.Instance;
        private Profiler p = Profiler.Instance;

        internal void Render()
        {
            p.StartSection("pass1");
            RenderPass1AndBuildPass2List();
            p.EndSection();
        }

        internal void RenderPass2()
        {
            p.StartSection("pass1");
            t.ResetTransformation();
            foreach (ChunkRenderer pass2ChunkRenderer in pass2ChunkRenderers)
            {
                pass2ChunkRenderer.RenderPass2();
            }
            p.EndSection();
        }

        private void RenderPass1AndBuildPass2List()
        {
            t.ResetTransformation();

            pass2ChunkRenderers.Clear();
            cachedChunks = World.Instance.GetCachedChunks();
            Counters.Instance.SetValue("total cached", cachedChunks.Count);

            if (cachedChunks.Count == 0)
                return;

            float viewRadiusSquared = (int)((GameSettings.ViewRadius * GameSettings.ViewRadius) / 256f);
            forceCachedRendering = false;
            if (cachedChunks.IsDirty)
            {
                chunksToRender = cachedChunks.OrderedChunks.Where(c =>
                {
                    float dx = c.Position.X - cachedChunks.LastCenterChunk.X;
                    float dy = c.Position.Y - cachedChunks.LastCenterChunk.Y;
                    float dz = c.Position.Z - cachedChunks.LastCenterChunk.Z;
                    return dx * dx + dy * dy + dz * dz <= viewRadiusSquared;
                }).ToList();
                cachedChunks.IsDirty = false;
            }

            p.StartSection("renderchunks");
            foreach (Chunk chunk in chunksToRender)
            {
                RenderChunk(chunk);
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

        private void RenderChunk(Chunk chunk)
        {
            Counters.Instance.Increment("chunks in view range");
            if (chunk == null || !ChunkRenderer.InsideViewFrustum(chunk)) return;
            Counters.Instance.Increment("chunks rendered");

            // get chunkrenderer for this chunk (create new if it does not exist)
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
