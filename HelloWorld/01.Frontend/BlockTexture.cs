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
using System.IO;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities;

namespace WindowsFormsApplication7.Frontend
{
    class BlockTextures
    {
        public static BlockTextures Instance = new BlockTextures();
        private Dictionary<int, int> topBlockTextures = new Dictionary<int, int>();
        private Dictionary<int, int> sideBlockTextures = new Dictionary<int, int>();
        private Dictionary<int, int> bottomBlockTextures = new Dictionary<int, int>();
        private Dictionary<string, int> indexMap = new Dictionary<string, int>();
        private Dictionary<int, VertexBuffer> blockVertexBuffers = new Dictionary<int, VertexBuffer>();
        public ShaderResourceView View;
        
        internal void Initialize()
        {
            LoadAllBlockTexture();
            DefineBlock(BlockRepository.Grass.Id, "grass", "grass_side", "dirt");
            DefineBlock(BlockRepository.Dirt.Id, "dirt");
            DefineBlock(BlockRepository.Stone.Id, "stone");
            DefineBlock(BlockRepository.Wood.Id, "wood_top", "wood_side", "wood_top");
            DefineBlock(BlockRepository.Leaf.Id, "leaf");
            DefineBlock(BlockRepository.Brick.Id, "brick");
            DefineBlock(BlockRepository.Sand.Id, "sand");
            DefineBlock(BlockRepository.BedRock.Id, "bedrock");
            DefineBlock(BlockRepository.Diamond.Id, "diamond");
        }

        private void DefineBlock(int blockid, string allSides)
        {
            DefineBlock(blockid, allSides, allSides, allSides);
        }

        private void BuildVertexBuffer(int blockId)
        {
            Tessellator t = Tessellator.Instance;
            t.StartDrawingTiledQuads();
            Vector4 c1, c2, c3, c4, c5, c6;
            Block block = BlockRepository.Blocks[blockId];
            Vector4[] blockColors = block.BlockColors;
            float vx = -0.5f;
            float vy = -0.5f;
            float vz = -0.5f;
            c1 = blockColors[0];
            c2 = blockColors[1];
            c3 = blockColors[2];
            c4 = blockColors[3];
            c5 = blockColors[4];
            c6 = blockColors[5];
            float s1 = 1f;
            float s2 = 1f;
            float s3 = 1f;
            float s4 = 1f;
            t.ArrayIndex = BlockTextures.Instance.SideIndex(blockId);
            Vector3 normal = new Vector3(0, 0, 1);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 1f, 1.0f), c1 * s1, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 1f, 1.0f), c1 * s2, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 1f, 1.0f), c1 * s3, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 1f, 1.0f), c1 * s4, normal);
             normal = new Vector3(0, 0, -1);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 0f, 1.0f), c2 * s1, normal);
            t.AddVertexWithColor(new Vector4(vx + 1, vy + 1f, vz + 0f, 1.0f), c2 * s2, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 0f, 1.0f), c2 * s3, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 0f, 1.0f), c2 * s4, normal);
            normal = new Vector3(-1, 0, 0);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 0f, 1.0f), c3 * s1, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 0f, 1.0f), c3 * s2, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 1f, 1.0f), c3 * s3, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 1f, 1.0f), c3 * s4, normal);
            normal = new Vector3(1, 0, 0);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 1f, 1.0f), c4 * s1, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 1f, 1.0f), c4 * s2, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 0f, 1.0f), c4 * s3, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 0f, 1.0f), c4 * s4, normal);
            t.ArrayIndex = BlockTextures.Instance.TopIndex(blockId);
            normal = new Vector3(0, 1, 0);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 1f, 1.0f), c5 * s1, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 0f, 1.0f), c5 * s2, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 0f, 1.0f), c5 * s3, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 1f, 1.0f), c5 * s4, normal);
            t.ArrayIndex = BlockTextures.Instance.BottomIndex(blockId);
            normal = new Vector3(0, -1, 0);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 0f, 1.0f), c6 * s1, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 1f, 1.0f), c6 * s2, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 1f, 1.0f), c6 * s3, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 0f, 1.0f), c6 * s4, normal);
            blockVertexBuffers.Add(blockId, t.GetVertexBuffer());
        }

        public void Dispose()
        {
            foreach (VertexBuffer vertexBuffer in blockVertexBuffers.Values)
            {
                vertexBuffer.Dispose();
            }
            blockVertexBuffers.Clear();
        }


        private void DefineBlock(int blockid, string top, string side, string bottom)
        {
            topBlockTextures[blockid] = indexMap[top];
            sideBlockTextures[blockid] = indexMap[side];
            bottomBlockTextures[blockid] = indexMap[bottom];
            BuildVertexBuffer(blockid);
        }

        private void LoadAllBlockTexture()
        {
            Device device = Tessellator.Instance.Device;
            List<Texture2D> textures = new List<Texture2D>();
            string[] allFiles = Directory.GetFiles("01.Frontend/Textures/Blocks/", "*.png");
            foreach (string filename in allFiles)
            {
                textures.Add(Texture2D.FromFile(device, filename));
            }

            var textureArrayDescription = textures[0].Description;
            textureArrayDescription.ArraySize = textures.Count;
            var textureArray = new Texture2D(device, textureArrayDescription);
            var mipLevels = textureArrayDescription.MipLevels;
            for (int j = 0; j < textures.Count; j++)
            {
                indexMap.Add(Path.GetFileNameWithoutExtension(allFiles[j]), j);
                for (var i = 0; i < mipLevels; i++)
                {
                    // for both textures
                    device.ImmediateContext.CopySubresourceRegion(textures[j], i, textureArray, mipLevels * j + i, 0, 0, 0);
                }
            }
            View = new ShaderResourceView(device, textureArray);
        }

        internal int TopIndex(int blockId)
        {
            return topBlockTextures[blockId];
        }

        internal int SideIndex(int blockId)
        {
            return sideBlockTextures[blockId];
        }

        internal int BottomIndex(int blockId)
        {
            return bottomBlockTextures[blockId];
        }


        internal VertexBuffer GetVertexBuffer(int blockId)
        {
            return blockVertexBuffers[blockId];
        }
    }
}
