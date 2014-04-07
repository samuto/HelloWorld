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
using WindowsFormsApplication7.CrossCutting.Entities.Blocks;

namespace WindowsFormsApplication7.Frontend
{
    class ChunkRenderer
    {
        public const int timeout = 5 * 1000;
        public int debug = 0;
        // opaque vertex buffer
        private VertexBuffer pass1VertexBuffer;
        private VertexBuffer pass2VertexBuffer;
        private Stopwatch stopwatch = new Stopwatch();
        private Chunk chunk;

        public ChunkRenderer(Chunk chunk)
        {
            this.chunk = chunk;
            stopwatch.Start();
            pass1VertexBuffer = new VertexBuffer();
            pass2VertexBuffer = new VertexBuffer();
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

        internal void RenderPass2()
        {
            // draw chunk if drawbuffer has been calculated
            Tessellator t = Tessellator.Instance;
            if (pass2VertexBuffer.Vertices != null)
            {
                t.StartDrawingTiledQuadsPass2();
                t.Draw(pass2VertexBuffer.Vertices, pass2VertexBuffer.VertexCount);
            }
        }

        internal bool Render(bool forceCachedRendering)
        {
            // check if this is inside frustum
            RenewLease();
            bool rebuildOccured = false;
            Tessellator t = Tessellator.Instance;


            if ((pass1VertexBuffer.Disposed || chunk.IsDirty) && !forceCachedRendering)
            {
                // pass1
                pass1VertexBuffer.Dispose();
                pass2VertexBuffer.Dispose();
                chunk.IsDirty = true;

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
                List<PositionBlock> pass2Blocks = new List<PositionBlock>();
                t.StartDrawingTiledQuads();
                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        for (int z = 0; z < 16; z++)
                        {
                            blockPos.X = x;
                            blockPos.Y = y;
                            blockPos.Z = z;
                            if (Block.FromId(chunk.SafeGetLocalBlock(x, y, z)).IsOpaque)
                                blockRenderer.RenderBlock(blockPos, chunk);
                            else
                                pass2Blocks.Add(blockPos);
                        }
                    }
                }
                pass1VertexBuffer = t.GetVertexBuffer();

                // generate vertex buffer for pass2

                t.StartDrawingTiledQuads();
                foreach (PositionBlock pass2BlockPos in pass2Blocks)
                {
                    blockRenderer.RenderBlock(pass2BlockPos, chunk);
                }
                pass2VertexBuffer = t.GetVertexBuffer();

                chunk.IsDirty = false;
                rebuildOccured = true;
            }

            // draw chunk if drawbuffer has been calculated
            if (pass1VertexBuffer.Vertices != null)
            {
                t.StartDrawingTiledQuads();
                t.Draw(pass1VertexBuffer.Vertices, pass1VertexBuffer.VertexCount);
            }
            // draw entities in chunk
            foreach (EntityStack stack in chunk.StackEntities)
            {
                if (stack.IsBlock)
                {
                    t.StartDrawingTiledQuads();
                    t.Translate = stack.Position;
                    t.Scale = new Vector3(0.5f, 0.5f, 0.5f);
                    t.Rotate = new Vector3(stack.Pitch, stack.Yaw, 0);
                    t.Draw(TileTextures.Instance.GetBlockVertexBuffer(stack.Id));
                }
                else if(stack.IsItem)
                {
                    Player p = World.Instance.Player;
                    t.StartDrawingTiledQuadsPass2();
                    t.Translate = stack.Position;
                    t.Scale = new Vector3(0.5f, 0.5f, 0.5f);
                    t.Rotate = new Vector3(-p.Pitch, p.Yaw+(float)Math.PI, 0);
                    t.Draw(TileTextures.Instance.GetItemVertexBuffer(stack.Id));
                }
            }
            t.ResetTransformation();


            
            return rebuildOccured;
        }

        private void RenewLease()
        {
            stopwatch.Restart();
        }

        internal void Dispose()
        {
            if (!pass1VertexBuffer.Disposed)
            {
                pass1VertexBuffer.Dispose();
                chunk.IsDirty = true;
            }
        }

        public bool Expired
        {
            get
            {
                return stopwatch.ElapsedMilliseconds > timeout;
            }
        }
    }
}
