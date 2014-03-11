using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.Frontend;
using SlimDX.Direct3D11;
using SlimDX;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Profiling;
using System.Diagnostics;

namespace WindowsFormsApplication7.Frontend
{
    class ChunkRenderer
    {
        public const int timeout = 10*1000;
        public int debug = 0;
        private DrawBufferWrapper wrapper;
        private Stopwatch stopwatch = new Stopwatch();
        
        public ChunkRenderer()
        {
            stopwatch.Start();
        }

        private class DrawBufferWrapper
        {
            public SlimDX.Direct3D11.Buffer DrawBuffer;
            public int VertexCount = 0;
            public Chunk Chunk;
            public bool Disposed = false;

            public DrawBufferWrapper(Chunk chunk)
            {
                this.Chunk = chunk;
            }

            internal void Dispose()
            {
                if (Disposed)
                    return;
                if (DrawBuffer != null && !DrawBuffer.Disposed)
                    DrawBuffer.Dispose();
                DrawBuffer = null;
                Chunk.RendererDetached();
                Disposed = true;
            }
        }

        public ChunkRenderer(Chunk chunk)
        {
            wrapper = new DrawBufferWrapper(chunk);
        }

        internal bool Render(bool forceCachedRendering)
        {
            RenewLease();
            Profiler p = Profiler.Instance;

            bool rerenderingOccured = false;
            Tessellator tessellator = Tessellator.Instance;
            tessellator.StartDrawingQuadsWithFog();
            p.StartSection("rebuild");

            if ((wrapper.Disposed || wrapper.Chunk.RequiresRendering))
            {
                Counters.Instance.Increment("chunks_to_rebuild");
            }
            if ((wrapper.Disposed || wrapper.Chunk.RequiresRendering) && !forceCachedRendering)
            {
                Counters.Instance.Increment("chunks rebuilded");
                p.StartSection("init");
                // safe chunk reference and dispose wrapper
                Chunk chunkToBeWrapped = wrapper.Chunk;
                wrapper.Dispose();

                // rebuild vertices for cunk
                BlockRenderer blockRenderer = new BlockRenderer();
                PositionBlock startCorner;
                wrapper.Chunk.Position.GetMinCornerBlock(out startCorner);
                int minX = startCorner.X;
                int minY = startCorner.Y;
                int minZ = startCorner.Z;
                int maxX = startCorner.X + 16;
                int maxY = startCorner.Y + 16;
                int maxZ = startCorner.Z + 16;
                PositionBlock blockPos = new PositionBlock(0, 0, 0);
                p.EndStartSection("allblocks");
                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        for (int z = 0; z < 16; z++)
                        {
                            blockPos.X = x;
                            blockPos.Y = y;
                            blockPos.Z = z;
                            blockRenderer.RenderBlock(blockPos, chunkToBeWrapped);
                        }
                    }
                }
                p.EndStartSection("wrapper");

                // create new wrapper object for cunk with updated vertices
                wrapper = new DrawBufferWrapper(chunkToBeWrapped);
                wrapper.DrawBuffer = tessellator.GetDrawBuffer();
                wrapper.VertexCount = tessellator.VertexCount;
                wrapper.Chunk.RenderingDone();
                rerenderingOccured = true;
                p.EndSection();

            }
            p.EndStartSection("draw");

            // draw chunk if drawbuffer has been calculated
            if (wrapper.DrawBuffer != null)
            {
                tessellator.Draw(wrapper.DrawBuffer, wrapper.VertexCount);

                // debug-rendering
                if (GameSettings.ChunkDebuggingEnabled)
                {
                    Tessellator t = Tessellator.Instance;
                    t.StartDrawingLines();
                    PositionBlock startCorner;
                    wrapper.Chunk.Position.GetMinCornerBlock(out startCorner);
                    float x = startCorner.X;
                    float y = 64;
                    float z = startCorner.Z;
                    debug = wrapper.DrawBuffer.Disposed ? 0 : 1;
                    Vector4 c = new Vector4(1, 1, 1, 1);
                    t.AddVertexWithColor(new Vector4(x + 1f, y + 0f, z + 1f, 1.0f), c);
                    t.AddVertexWithColor(new Vector4(x + 15f, y + 0f, z + 1f, 1.0f), c);
                    t.AddVertexWithColor(new Vector4(x + 15f, y + 0f, z + 1f, 1.0f), c);
                    t.AddVertexWithColor(new Vector4(x + 15f, y + 0f, z + 15f, 1.0f), c);
                    t.AddVertexWithColor(new Vector4(x + 15f, y + 0f, z + 15f, 1.0f), c);
                    t.AddVertexWithColor(new Vector4(x + 1f, y + 0f, z + 15f, 1.0f), c);
                    t.AddVertexWithColor(new Vector4(x + 1f, y + 0f, z + 15f, 1.0f), c);
                    t.AddVertexWithColor(new Vector4(x + 1f, y + 0f, z + 1f, 1.0f), c);
                    t.AddVertexWithColor(new Vector4(x + 7f, y + 0f, z + 7f, 1.0f), c);
                    t.AddVertexWithColor(new Vector4(x + 7f, 0f, z + 7f, 1.0f), c);
                    Tessellator.Instance.Draw();
                }
            }
            p.EndSection();

            return rerenderingOccured;
        }

        private void RenewLease()
        {
            stopwatch.Restart();
        }

        internal void Dispose()
        {
            if (!wrapper.Disposed)
            {
                wrapper.Dispose();
            }
        }

        public bool Expired 
        {
            get
            {
                return stopwatch.ElapsedMilliseconds>timeout;
            }
        }
    }
}
