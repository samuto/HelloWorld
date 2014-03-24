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

namespace WindowsFormsApplication7.Frontend
{
    class BlockRenderer
    {
        private World world = World.Instance;

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
            float vx = globalPosition.X;
            float vy = globalPosition.Y;
            float vz = globalPosition.Z;

            Block block = BlockRepository.Blocks[blockId];
            int x = positionBlock.X;
            int y = positionBlock.Y;
            int z = positionBlock.Z;

            Vector4 c1, c2, c3, c4, c5, c6;
            Vector4[] blockColors = block.BlockColors;
            if (blockColors == null)
            {
                c1 = new Vector4(1f, 1f, 1f, 1f);
                c2 = c1;
                c3 = c1;
                c4 = c1;
                c5 = c1;
                c6 = c1;
            }
            else
            {  
                c1 = blockColors[0];
                c2 = blockColors[1];
                c3 = blockColors[2];
                c4 = blockColors[3];
                c5 = blockColors[4];
                c6 = blockColors[5];
            }

            float reduction = 0.3f;

            Tessellator tessellator = Tessellator.Instance;
            if (chunk.SafeGetLocalBlock(x, y, z + 1) == 0)
            {
                // front
                tessellator.ArrayIndex = BlockTextures.Instance.SideIndex(blockId);
                bool a1 = chunk.SafeGetLocalBlock(x, y - 1, z + 1) == 0 ? false : true;
                bool a2 = chunk.SafeGetLocalBlock(x - 1, y - 1, z + 1) == 0 ? false : true;
                bool a3 = chunk.SafeGetLocalBlock(x - 1, y, z + 1) == 0 ? false : true;
                bool a4 = chunk.SafeGetLocalBlock(x - 1, y + 1, z + 1) == 0 ? false : true;
                bool a5 = chunk.SafeGetLocalBlock(x, y + 1, z + 1) == 0 ? false : true;
                bool a6 = chunk.SafeGetLocalBlock(x + 1, y + 1, z + 1) == 0 ? false : true;
                bool a7 = chunk.SafeGetLocalBlock(x + 1, y, z + 1) == 0 ? false : true;
                bool a8 = chunk.SafeGetLocalBlock(x + 1, y - 1, z + 1) == 0 ? false : true;

                float s1, s2, s3, s4;
                s1 = a1 || a2 || a3 ? reduction : 1f;
                s2 = a3 || a4 || a5 ? reduction : 1f;
                s3 = a5 || a6 || a7 ? reduction : 1f;
                s4 = a7 || a8 || a1 ? reduction : 1f;

                Vector3 normal = new Vector3(0,0,1);
                tessellator.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 1f, 1.0f), c1 * s1, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 1f, 1.0f), c1 * s2, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 1f, 1.0f), c1 * s3, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 1f, 1.0f), c1 * s4, normal);
            }

            if (chunk.SafeGetLocalBlock(x, y, z - 1) == 0)
            {
                // back
                tessellator.ArrayIndex = BlockTextures.Instance.SideIndex(blockId);
                bool a1 = chunk.SafeGetLocalBlock(x, y - 1, z - 1) == 0 ? false : true;
                bool a2 = chunk.SafeGetLocalBlock(x + 1, y - 1, z - 1) == 0 ? false : true;
                bool a3 = chunk.SafeGetLocalBlock(x + 1, y, z - 1) == 0 ? false : true;
                bool a4 = chunk.SafeGetLocalBlock(x + 1, y + 1, z - 1) == 0 ? false : true;
                bool a5 = chunk.SafeGetLocalBlock(x, y + 1, z - 1) == 0 ? false : true;
                bool a6 = chunk.SafeGetLocalBlock(x - 1, y + 1, z - 1) == 0 ? false : true;
                bool a7 = chunk.SafeGetLocalBlock(x - 1, y, z - 1) == 0 ? false : true;
                bool a8 = chunk.SafeGetLocalBlock(x - 1, y - 1, z - 1) == 0 ? false : true;

                float s1, s2, s3, s4;
                s1 = a1 || a2 || a3 ? reduction : 1f;
                s2 = a3 || a4 || a5 ? reduction : 1f;
                s3 = a5 || a6 || a7 ? reduction : 1f;
                s4 = a7 || a8 || a1 ? reduction : 1f;

                Vector3 normal = new Vector3(0, 0, -1);
                tessellator.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 0f, 1.0f), c2 * s1, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 1, vy + 1f, vz + 0f, 1.0f), c2 * s2, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 0f, 1.0f), c2 * s3, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 0f, 1.0f), c2 * s4, normal);
            }

            if (chunk.SafeGetLocalBlock(x - 1, y, z) == 0)
            {
                //left
                tessellator.ArrayIndex = BlockTextures.Instance.SideIndex(blockId);
                bool a1 = chunk.SafeGetLocalBlock(x - 1, y - 1, z) == 0 ? false : true;
                bool a2 = chunk.SafeGetLocalBlock(x - 1, y - 1, z - 1) == 0 ? false : true;
                bool a3 = chunk.SafeGetLocalBlock(x - 1, y, z - 1) == 0 ? false : true;
                bool a4 = chunk.SafeGetLocalBlock(x - 1, y + 1, z - 1) == 0 ? false : true;
                bool a5 = chunk.SafeGetLocalBlock(x - 1, y + 1, z) == 0 ? false : true;
                bool a6 = chunk.SafeGetLocalBlock(x - 1, y + 1, z + 1) == 0 ? false : true;
                bool a7 = chunk.SafeGetLocalBlock(x - 1, y, z + 1) == 0 ? false : true;
                bool a8 = chunk.SafeGetLocalBlock(x - 1, y - 1, z + 1) == 0 ? false : true;

                float s1, s2, s3, s4;
                s1 = a1 || a2 || a3 ? reduction : 1f;
                s2 = a3 || a4 || a5 ? reduction : 1f;
                s3 = a5 || a6 || a7 ? reduction : 1f;
                s4 = a7 || a8 || a1 ? reduction : 1f;

                Vector3 normal = new Vector3(-1, 0, 0);
                tessellator.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 0f, 1.0f), c3 * s1, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 0f, 1.0f), c3 * s2, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 1f, 1.0f), c3 * s3, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 1f, 1.0f), c3 * s4, normal);
            }

            if (chunk.SafeGetLocalBlock(x + 1, y, z) == 0)
            {
                //right
                tessellator.ArrayIndex = BlockTextures.Instance.SideIndex(blockId);
                bool a1 = chunk.SafeGetLocalBlock(x + 1, y - 1, z) == 0 ? false : true;
                bool a2 = chunk.SafeGetLocalBlock(x + 1, y - 1, z + 1) == 0 ? false : true;
                bool a3 = chunk.SafeGetLocalBlock(x + 1, y, z + 1) == 0 ? false : true;
                bool a4 = chunk.SafeGetLocalBlock(x + 1, y + 1, z + 1) == 0 ? false : true;
                bool a5 = chunk.SafeGetLocalBlock(x + 1, y + 1, z) == 0 ? false : true;
                bool a6 = chunk.SafeGetLocalBlock(x + 1, y + 1, z - 1) == 0 ? false : true;
                bool a7 = chunk.SafeGetLocalBlock(x + 1, y, z - 1) == 0 ? false : true;
                bool a8 = chunk.SafeGetLocalBlock(x + 1, y - 1, z - 1) == 0 ? false : true;

                float s1, s2, s3, s4;
                s1 = a1 || a2 || a3 ? reduction : 1f;
                s2 = a3 || a4 || a5 ? reduction : 1f;
                s3 = a5 || a6 || a7 ? reduction : 1f;
                s4 = a7 || a8 || a1 ? reduction : 1f;

                Vector3 normal = new Vector3(1, 0, 0);
                tessellator.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 1f, 1.0f), c4 * s1, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 1f, 1.0f), c4 * s2, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 0f, 1.0f), c4 * s3, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 0f, 1.0f), c4 * s4, normal);
            }

            if (chunk.SafeGetLocalBlock(x, y + 1, z) == 0)
            {
                //top
                tessellator.ArrayIndex = BlockTextures.Instance.TopIndex(blockId);
                bool a1 = chunk.SafeGetLocalBlock(x, y + 1, z + 1) == 0 ? false : true;
                bool a2 = chunk.SafeGetLocalBlock(x - 1, y + 1, z + 1) == 0 ? false : true;
                bool a3 = chunk.SafeGetLocalBlock(x - 1, y + 1, z) == 0 ? false : true;
                bool a4 = chunk.SafeGetLocalBlock(x - 1, y + 1, z - 1) == 0 ? false : true;
                bool a5 = chunk.SafeGetLocalBlock(x, y + 1, z - 1) == 0 ? false : true;
                bool a6 = chunk.SafeGetLocalBlock(x + 1, y + 1, z - 1) == 0 ? false : true;
                bool a7 = chunk.SafeGetLocalBlock(x + 1, y + 1, z) == 0 ? false : true;
                bool a8 = chunk.SafeGetLocalBlock(x + 1, y + 1, z + 1) == 0 ? false : true;


                float s1, s2, s3, s4;
                reduction = 0.5f;
                s1 = a1 || a2 || a3 ? reduction : 1f;
                s2 = a3 || a4 || a5 ? reduction : 1f;
                s3 = a5 || a6 || a7 ? reduction : 1f;
                s4 = a7 || a8 || a1 ? reduction : 1f;

                Vector3 normal = new Vector3(0, 1, 0);
                tessellator.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 1f, 1.0f), c5 * s1, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 0f, 1.0f), c5 * s2, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 0f, 1.0f), c5 * s3, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 1f, 1.0f), c5 * s4, normal);
            }

            if (chunk.SafeGetLocalBlock(x, y - 1, z) == 0)
            {
                //bottom
                tessellator.ArrayIndex = BlockTextures.Instance.BottomIndex(blockId);
                bool a1 = chunk.SafeGetLocalBlock(x, y - 1, z - 1) == 0 ? false : true;
                bool a2 = chunk.SafeGetLocalBlock(x - 1, y - 1, z - 1) == 0 ? false : true;
                bool a3 = chunk.SafeGetLocalBlock(x - 1, y - 1, z) == 0 ? false : true;
                bool a4 = chunk.SafeGetLocalBlock(x - 1, y - 1, z + 1) == 0 ? false : true;
                bool a5 = chunk.SafeGetLocalBlock(x, y - 1, z + 1) == 0 ? false : true;
                bool a6 = chunk.SafeGetLocalBlock(x + 1, y - 1, z + 1) == 0 ? false : true;
                bool a7 = chunk.SafeGetLocalBlock(x + 1, y - 1, z) == 0 ? false : true;
                bool a8 = chunk.SafeGetLocalBlock(x + 1, y - 1, z - 1) == 0 ? false : true;


                float s1, s2, s3, s4;
                s1 = a1 || a2 || a3 ? reduction : 1f;
                s2 = a3 || a4 || a5 ? reduction : 1f;
                s3 = a5 || a6 || a7 ? reduction : 1f;
                s4 = a7 || a8 || a1 ? reduction : 1f;

                Vector3 normal = new Vector3(0, -1, 0);
                tessellator.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 0f, 1.0f), c6 * s1, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 1f, 1.0f), c6 * s2, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 1f, 1.0f), c6 * s3, normal);
                tessellator.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 0f, 1.0f), c6 * s4, normal);
            }

            tessellator.ArrayIndex = -1;
        }
    }
}
