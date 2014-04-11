using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using SlimDX.D3DCompiler;
using Device = SlimDX.Direct3D11.Device;
using System.Drawing;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.CrossCutting.Entities.Blocks;

namespace WindowsFormsApplication7.Frontend
{
    class BlockRenderer
    {
        private World world = World.Instance;
        private Tessellator t = Tessellator.Instance;
            
        public BlockRenderer()
        {
        }

        internal void RenderBlock(PositionBlock positionBlock, Chunk chunk)
        {
            int blockId = chunk.SafeGetLocalBlock(positionBlock.X, positionBlock.Y, positionBlock.Z);
            if (blockId == 0)
            {
                return;
            }
            PositionBlock globalPosition;
            chunk.Position.GetGlobalPositionBlock(out globalPosition, positionBlock.X, positionBlock.Y, positionBlock.Z);

            Block block = BlockRepository.Blocks[blockId];
            if (block.Id == BlockRepository.TallGrass.Id)
                RenderCross(block, globalPosition, positionBlock, chunk);
            else if (block.Id == BlockRepository.Wheat.Id)
                RenderHash(block, globalPosition, positionBlock, chunk);
            else
                RenderCube(block, globalPosition, positionBlock, chunk);
        }

        private void RenderHash(Block block, PositionBlock globalPosition, PositionBlock positionBlock, Chunk chunk)
        {
            Vector4 c1, c2, c3, c4, c5, c6;
            int blockId = block.Id;
            Vector4[] blockColors = block.BlockColors;
            c1 = blockColors[0];
            c2 = blockColors[1];
            c3 = blockColors[2];
            c4 = blockColors[3];
            c5 = blockColors[4];
            c6 = blockColors[5];
            float vx = globalPosition.X;
            float vy = globalPosition.Y;
            float vz = globalPosition.Z;
            int x = positionBlock.X;
            int y = positionBlock.Y;
            int z = positionBlock.Z;
            if (block.HasStages)
            {
                int stage = chunk.MetaDataGetInt("stage", positionBlock);
                t.ArrayIndex = TileTextures.Instance.GetStage(blockId, stage);
            }
            else
                t.ArrayIndex = TileTextures.Instance.FrontIndex(blockId);
            float margin = 0.2f;
            Vector3 normal = new Vector3(0, 0, 1);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 1f - margin, 1.0f), c1, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 1f - margin, 1.0f), c1, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 1f - margin, 1.0f), c1, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 1f - margin, 1.0f), c1, normal);
            normal = new Vector3(0, 0, 1);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 0f + margin, 1.0f), c1, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 0f + margin, 1.0f), c1, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 0f + margin, 1.0f), c1, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 0f + margin, 1.0f), c1, normal);
            normal = new Vector3(0, 0, -1);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 1f - margin, 1.0f), c2, normal);
            t.AddVertexWithColor(new Vector4(vx + 1, vy + 1f, vz + 1f - margin, 1.0f), c2, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 1f - margin, 1.0f), c2, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 1f - margin, 1.0f), c2, normal);
            normal = new Vector3(0, 0, -1);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 0f + margin, 1.0f), c2, normal);
            t.AddVertexWithColor(new Vector4(vx + 1, vy + 1f, vz + 0f + margin, 1.0f), c2, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 0f + margin, 1.0f), c2, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 0f + margin, 1.0f), c2, normal);
            normal = new Vector3(-1, 0, 0);
            t.AddVertexWithColor(new Vector4(vx + margin, vy + 0f, vz + 0f, 1.0f), c3, normal);
            t.AddVertexWithColor(new Vector4(vx + margin, vy + 1f, vz + 0f, 1.0f), c3, normal);
            t.AddVertexWithColor(new Vector4(vx + margin, vy + 1f, vz + 1f, 1.0f), c3, normal);
            t.AddVertexWithColor(new Vector4(vx + margin, vy + 0f, vz + 1f, 1.0f), c3, normal);
            normal = new Vector3(-1, 0, 0);
            t.AddVertexWithColor(new Vector4(vx + 1f - margin, vy + 0f, vz + 0f, 1.0f), c3, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f - margin, vy + 1f, vz + 0f, 1.0f), c3, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f - margin, vy + 1f, vz + 1f, 1.0f), c3, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f - margin, vy + 0f, vz + 1f, 1.0f), c3, normal);

            normal = new Vector3(1, 0, 0);
            t.AddVertexWithColor(new Vector4(vx + margin, vy + 0f, vz + 1f, 1.0f), c4, normal);
            t.AddVertexWithColor(new Vector4(vx + margin, vy + 1f, vz + 1f, 1.0f), c4, normal);
            t.AddVertexWithColor(new Vector4(vx + margin, vy + 1f, vz + 0f, 1.0f), c4, normal);
            t.AddVertexWithColor(new Vector4(vx + margin, vy + 0f, vz + 0f, 1.0f), c4, normal);
            normal = new Vector3(1, 0, 0);
            t.AddVertexWithColor(new Vector4(vx + 1f - margin, vy + 0f, vz + 1f, 1.0f), c4, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f - margin, vy + 1f, vz + 1f, 1.0f), c4, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f - margin, vy + 1f, vz + 0f, 1.0f), c4, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f - margin, vy + 0f, vz + 0f, 1.0f), c4, normal);
            t.ArrayIndex = -1;
        }

        private void RenderCross(Block block, PositionBlock globalPosition, PositionBlock positionBlock, Chunk chunk)
        {
            Vector4 c1, c2, c3, c4, c5, c6;
            int blockId = block.Id;
            Vector4[] blockColors = block.BlockColors;
            c1 = blockColors[0];
            c2 = blockColors[1];
            c3 = blockColors[2];
            c4 = blockColors[3];
            c5 = blockColors[4];
            c6 = blockColors[5];
            float vx = globalPosition.X;
            float vy = globalPosition.Y;
            float vz = globalPosition.Z;
            int x = positionBlock.X;
            int y = positionBlock.Y;
            int z = positionBlock.Z;
            t.ArrayIndex = TileTextures.Instance.FrontIndex(blockId);
            Vector3 normal = new Vector3(-0.7f, 0, 0.7f);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 0.0f, 1.0f), c1, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 0.0f, 1.0f), c1, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 1f, 1.0f), c1, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 1f, 1.0f), c1, normal);
            normal = new Vector3(0.7f, 0, -0.7f);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 1f, 1.0f), c2, normal);
            t.AddVertexWithColor(new Vector4(vx + 1, vy + 1f, vz + 1f, 1.0f), c2, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 0f, 1.0f), c2, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 0f, 1.0f), c2, normal);
            normal = new Vector3(0.7f, 0, 0.7f);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 1f, 1.0f), c3, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 1f, 1.0f), c3, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 0f, 1.0f), c3, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 0f, 1.0f), c3, normal);
            normal = new Vector3(-0.7f, 0, -0.7f);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 0f, 1.0f), c4, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 0f, 1.0f), c4, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 1f, 1.0f), c4, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 1f, 1.0f), c4, normal);
            t.ArrayIndex = -1;
        }

        private void RenderCube(Block block, PositionBlock globalPosition, PositionBlock positionBlock, Chunk chunk)
        {
            Vector4 c1, c2, c3, c4, c5, c6;
            int blockId = block.Id;
            Vector4[] blockColors = block.BlockColors;
            c1 = blockColors[0];
            c2 = blockColors[1];
            c3 = blockColors[2];
            c4 = blockColors[3];
            c5 = blockColors[4];
            c6 = blockColors[5];
            float vx = globalPosition.X;
            float vy = globalPosition.Y;
            float vz = globalPosition.Z;
            int x = positionBlock.X;
            int y = positionBlock.Y;
            int z = positionBlock.Z;
            float reduction = 0.3f;
            bool renderFront = !IsOpaque(chunk.SafeGetLocalBlock(x, y, z + 1));
            bool renderBack = !IsOpaque(chunk.SafeGetLocalBlock(x, y, z - 1));
            bool renderLeft = !IsOpaque(chunk.SafeGetLocalBlock(x - 1, y, z));
            bool renderRight = !IsOpaque(chunk.SafeGetLocalBlock(x + 1, y, z));
            bool renderTop = !IsOpaque(chunk.SafeGetLocalBlock(x, y + 1, z));
            bool renderBottom = !IsOpaque(chunk.SafeGetLocalBlock(x, y - 1, z));
            if (renderFront)
            {
                // front
                t.ArrayIndex = TileTextures.Instance.FrontIndex(blockId);
                bool a1 = IsOpaque(chunk.SafeGetLocalBlock(x, y - 1, z + 1));
                bool a2 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y - 1, z + 1));
                bool a3 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y, z + 1));
                bool a4 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y + 1, z + 1));
                bool a5 = IsOpaque(chunk.SafeGetLocalBlock(x, y + 1, z + 1));
                bool a6 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y + 1, z + 1));
                bool a7 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y, z + 1));
                bool a8 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y - 1, z + 1));

                float s1, s2, s3, s4;
                s1 = a1 || a2 || a3 ? reduction : 1f;
                s2 = a3 || a4 || a5 ? reduction : 1f;
                s3 = a5 || a6 || a7 ? reduction : 1f;
                s4 = a7 || a8 || a1 ? reduction : 1f;

                Vector3 normal = new Vector3(0, 0, 1);
                t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 1f, 1.0f), AdjustColor(c1, s1), normal);
                t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 1f, 1.0f), AdjustColor(c1, s2), normal);
                t.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 1f, 1.0f), AdjustColor(c1, s3), normal);
                t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 1f, 1.0f), AdjustColor(c1, s4), normal);
            }

            if (renderBack)
            {
                // back
                t.ArrayIndex = TileTextures.Instance.SideIndex(blockId);
                bool a1 = IsOpaque(chunk.SafeGetLocalBlock(x, y - 1, z - 1));
                bool a2 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y - 1, z - 1));
                bool a3 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y, z - 1));
                bool a4 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y + 1, z - 1));
                bool a5 = IsOpaque(chunk.SafeGetLocalBlock(x, y + 1, z - 1));
                bool a6 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y + 1, z - 1));
                bool a7 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y, z - 1));
                bool a8 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y - 1, z - 1));

                float s1, s2, s3, s4;
                s1 = a1 || a2 || a3 ? reduction : 1f;
                s2 = a3 || a4 || a5 ? reduction : 1f;
                s3 = a5 || a6 || a7 ? reduction : 1f;
                s4 = a7 || a8 || a1 ? reduction : 1f;

                Vector3 normal = new Vector3(0, 0, -1);
                t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 0f, 1.0f), AdjustColor(c2, s1), normal);
                t.AddVertexWithColor(new Vector4(vx + 1, vy + 1f, vz + 0f, 1.0f), AdjustColor(c2, s2), normal);
                t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 0f, 1.0f), AdjustColor(c2, s3), normal);
                t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 0f, 1.0f), AdjustColor(c2, s4), normal);
            }

            if (renderLeft)
            {
                //left
                t.ArrayIndex = TileTextures.Instance.SideIndex(blockId);
                bool a1 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y - 1, z));
                bool a2 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y - 1, z - 1));
                bool a3 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y, z - 1));
                bool a4 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y + 1, z - 1));
                bool a5 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y + 1, z));
                bool a6 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y + 1, z + 1));
                bool a7 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y, z + 1));
                bool a8 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y - 1, z + 1));

                float s1, s2, s3, s4;
                s1 = a1 || a2 || a3 ? reduction : 1f;
                s2 = a3 || a4 || a5 ? reduction : 1f;
                s3 = a5 || a6 || a7 ? reduction : 1f;
                s4 = a7 || a8 || a1 ? reduction : 1f;

                Vector3 normal = new Vector3(-1, 0, 0);
                t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 0f, 1.0f), AdjustColor(c3, s1), normal);
                t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 0f, 1.0f), AdjustColor(c3, s2), normal);
                t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 1f, 1.0f), AdjustColor(c3, s3), normal);
                t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 1f, 1.0f), AdjustColor(c3, s4), normal);
            }

            if (renderRight)
            {
                //right
                t.ArrayIndex = TileTextures.Instance.SideIndex(blockId);
                bool a1 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y - 1, z));
                bool a2 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y - 1, z + 1));
                bool a3 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y, z + 1));
                bool a4 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y + 1, z + 1));
                bool a5 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y + 1, z));
                bool a6 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y + 1, z - 1));
                bool a7 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y, z - 1));
                bool a8 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y - 1, z - 1));

                float s1, s2, s3, s4;
                s1 = a1 || a2 || a3 ? reduction : 1f;
                s2 = a3 || a4 || a5 ? reduction : 1f;
                s3 = a5 || a6 || a7 ? reduction : 1f;
                s4 = a7 || a8 || a1 ? reduction : 1f;

                Vector3 normal = new Vector3(1, 0, 0);
                t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 1f, 1.0f), AdjustColor(c4, s1), normal);
                t.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 1f, 1.0f), AdjustColor(c4, s2), normal);
                t.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 0f, 1.0f), AdjustColor(c4, s3), normal);
                t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 0f, 1.0f), AdjustColor(c4, s4), normal);
            }

            if (renderTop)
            {
                //top
                t.ArrayIndex = TileTextures.Instance.TopIndex(blockId);
                bool a1 = IsOpaque(chunk.SafeGetLocalBlock(x, y + 1, z + 1));
                bool a2 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y + 1, z + 1));
                bool a3 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y + 1, z));
                bool a4 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y + 1, z - 1));
                bool a5 = IsOpaque(chunk.SafeGetLocalBlock(x, y + 1, z - 1));
                bool a6 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y + 1, z - 1));
                bool a7 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y + 1, z));
                bool a8 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y + 1, z + 1));


                float s1, s2, s3, s4;
                reduction = 0.5f;
                s1 = a1 || a2 || a3 ? reduction : 1f;
                s2 = a3 || a4 || a5 ? reduction : 1f;
                s3 = a5 || a6 || a7 ? reduction : 1f;
                s4 = a7 || a8 || a1 ? reduction : 1f;

                Vector3 normal = new Vector3(0, 1, 0);
                t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 1f, 1.0f), AdjustColor(c5, s1), normal);
                t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 0f, 1.0f), AdjustColor(c5, s2), normal);
                t.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 0f, 1.0f), AdjustColor(c5, s3), normal);
                t.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 1f, 1.0f), AdjustColor(c5, s4), normal);
            }

            if (renderBottom)
            {
                //bottom
                t.ArrayIndex = TileTextures.Instance.BottomIndex(blockId);
                bool a1 = IsOpaque(chunk.SafeGetLocalBlock(x, y - 1, z - 1));
                bool a2 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y - 1, z - 1));
                bool a3 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y - 1, z));
                bool a4 = IsOpaque(chunk.SafeGetLocalBlock(x - 1, y - 1, z + 1));
                bool a5 = IsOpaque(chunk.SafeGetLocalBlock(x, y - 1, z + 1));
                bool a6 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y - 1, z + 1));
                bool a7 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y - 1, z));
                bool a8 = IsOpaque(chunk.SafeGetLocalBlock(x + 1, y - 1, z - 1));


                float s1, s2, s3, s4;
                s1 = a1 || a2 || a3 ? reduction : 1f;
                s2 = a3 || a4 || a5 ? reduction : 1f;
                s3 = a5 || a6 || a7 ? reduction : 1f;
                s4 = a7 || a8 || a1 ? reduction : 1f;

                Vector3 normal = new Vector3(0, -1, 0);
                t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 0f, 1.0f), AdjustColor(c6, s1), normal);
                t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 1f, 1.0f), AdjustColor(c6, s2), normal);
                t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 1f, 1.0f), AdjustColor(c6, s3), normal);
                t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 0f, 1.0f), AdjustColor(c6, s4), normal);
            }
            t.ArrayIndex = -1;
        }

        private bool IsOpaque(int blockId)
        {
            return Block.FromId(blockId).IsOpaque;
        }



        private Vector4 AdjustColor(Vector4 c1, float s1)
        {
            return new Vector4(c1.X * s1, c1.Y * s1, c1.Z * s1, 1);
        }
    }
}
