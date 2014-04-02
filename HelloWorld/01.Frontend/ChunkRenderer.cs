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
        public const int timeout = 5*1000;
        public int debug = 0;
        private VertexBuffer vertexBuffer;
        private Stopwatch stopwatch = new Stopwatch();
        private Chunk chunk;

        public ChunkRenderer(Chunk chunk)
        {
            this.chunk = chunk;
            stopwatch.Start();
            vertexBuffer = new VertexBuffer();
        }

        internal static bool InsideViewFrustum(Chunk chunk)
        {
            if (!Camera.Instance.InsideViewFrustum(chunk.GetBoundingBox()))
            {
                Counters.Instance.Increment("chunk !frustum");
                return false;
            }
            return true;
        }

        internal bool Render(bool forceCachedRendering)
        {
            // check if this is inside frustum
            RenewLease();
            Profiler p = Profiler.Instance;

            bool rebuildOccured = false;
            Tessellator tessellator = Tessellator.Instance;
            tessellator.StartDrawingTiledQuads();
            p.StartSection("rebuild");

            if ((vertexBuffer.Disposed || chunk.RequiresRendering) && !forceCachedRendering)
            {
                p.StartSection("init");
                // safe chunk reference and dispose wrapper
                 vertexBuffer.Dispose();
                chunk.OnVertexBufferDisposed();

                // rebuild vertices for cunk
                BlockRenderer blockRenderer = new BlockRenderer();
                PositionBlock startCorner;
                chunk.Position.GetMinCornerBlock(out startCorner);
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
                            blockRenderer.RenderBlock(blockPos, chunk);
                        }
                    }
                }
                p.EndStartSection("wrapper");

                // create new wrapper object for cunk with updated vertices
                vertexBuffer = tessellator.GetVertexBuffer();
                chunk.RenderingDone();
                rebuildOccured = true;
                p.EndSection();

            }
            p.EndStartSection("draw");

            // draw chunk if drawbuffer has been calculated
            if (vertexBuffer.Vertices != null)
            {
                tessellator.Draw(vertexBuffer.Vertices, vertexBuffer.VertexCount);

                // debug-rendering
                if (GameSettings.ChunkDebuggingEnabled)
                {
                    Tessellator t = Tessellator.Instance;
                    t.StartDrawingLines();
                    PositionBlock startCorner;
                    chunk.Position.GetMinCornerBlock(out startCorner);
                    float x = startCorner.X;
                    float y = 64;
                    float z = startCorner.Z;
                    debug = vertexBuffer.Vertices.Disposed ? 0 : 1;
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

            return rebuildOccured;
        }

        private void RenewLease()
        {
            stopwatch.Restart();
        }

        internal void Dispose()
        {
            if (!vertexBuffer.Disposed)
            {
                vertexBuffer.Dispose();
                chunk.OnVertexBufferDisposed();
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
