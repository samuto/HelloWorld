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
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.Frontend
{
    class TileTextures
    {
        public static TileTextures Instance = new TileTextures();
        private Dictionary<int, int> topBlockTextures = new Dictionary<int, int>();
        private Dictionary<int, int> sideBlockTextures = new Dictionary<int, int>();
        private Dictionary<int, int> bottomBlockTextures = new Dictionary<int, int>();
        private Dictionary<string, int> indexMap = new Dictionary<string, int>();
        private Dictionary<int, VertexBuffer> blockVertexBuffers = new Dictionary<int, VertexBuffer>();
        private Dictionary<int, VertexBuffer> itemVertexBuffers = new Dictionary<int, VertexBuffer>();
        private VertexBuffer[] destroyBlocks = new VertexBuffer[10];
        public ShaderResourceView View;

        internal void Initialize()
        {
            LoadAllTileTextures();

            // define blocks
            DefineBlock(BlockRepository.Grass.Id, "grass_top", "grass_side", "dirt");
            DefineBlock(BlockRepository.Dirt.Id, "dirt");
            DefineBlock(BlockRepository.Stone.Id, "stone");
            DefineBlock(BlockRepository.Wood.Id, "log_oak_top", "log_oak", "log_oak_top");
            DefineBlock(BlockRepository.Leaf.Id, "leaves_oak_opaque");
            DefineBlock(BlockRepository.Brick.Id, "brick");
            DefineBlock(BlockRepository.Sand.Id, "sand");
            DefineBlock(BlockRepository.BedRock.Id, "bedrock");
            DefineBlock(BlockRepository.Diamond.Id, "diamond_ore");
            DefineBlock(BlockRepository.Plank.Id, "planks_oak");
            DefineBlock(BlockRepository.CobbleStone.Id, "cobblestone");

            // Define destroy blocks...
            DefineDestroyBlocks();

            // define items
            BuildItemVertexBuffer(ItemRepository.Stick.Id, "stick");
            BuildItemVertexBuffer(ItemRepository.StoneAxe.Id, "stone_axe");
            BuildItemVertexBuffer(ItemRepository.StoneHoe.Id, "stone_hoe");
            BuildItemVertexBuffer(ItemRepository.StonePickAxe.Id, "stone_pickaxe");
            BuildItemVertexBuffer(ItemRepository.StoneShovel.Id, "stone_shovel");
            BuildItemVertexBuffer(ItemRepository.StoneSword.Id, "stone_sword");
        }

        private void DefineDestroyBlocks()
        {
            float alpha = 0.0f;
            Vector4[] BlockColors = new Vector4[] { 
                new Vector4(1,1,1,alpha),
                new Vector4(1,1,1,alpha),
                new Vector4(1,1,1,alpha),
                new Vector4(1,1,1,alpha),
                new Vector4(1,1,1,alpha),
                new Vector4(1,1,1,alpha)};
            for (int i = 0; i < 10; i++)
            {
                int tileIndex = indexMap["destroy_stage_" + i];
                destroyBlocks[i] = GenerateBlockVertices(
                tileIndex,
                tileIndex,
                tileIndex,
                tileIndex,
                tileIndex,
                tileIndex,
                BlockColors);
            }
        }

        private void BuildItemVertexBuffer(int itemId, string name)
        {
            Tessellator t = Tessellator.Instance;
            t.StartDrawingTiledQuads();
            Vector4 c1 = new Vector4(1, 1, 1, 1);
            t.ArrayIndex = indexMap[name];
            Vector3 normal = new Vector3(0, 0, 1);
            float s = 1f;
            t.AddVertexWithColor(new Vector4(0f, 0f, 0, 1.0f), c1, normal);
            t.AddVertexWithColor(new Vector4(0f, s, 0, 1.0f), c1, normal);
            t.AddVertexWithColor(new Vector4(s, s, 0, 1.0f), c1, normal);
            t.AddVertexWithColor(new Vector4(s, 0f, 0, 1.0f), c1, normal);
            itemVertexBuffers.Add(itemId, t.GetVertexBuffer());
        }

        private void DefineBlock(int blockid, string allSides)
        {
            DefineBlock(blockid, allSides, allSides, allSides);
        }

        private void BuildBlockVertexBuffer(int blockId)
        {
            int tileIndexFront = TileTextures.Instance.SideIndex(blockId);
            int tileIndexTop = TileTextures.Instance.TopIndex(blockId);
            int tileIndexBottom = TileTextures.Instance.BottomIndex(blockId);

            blockVertexBuffers.Add(blockId, GenerateBlockVertices(
                tileIndexFront,
                tileIndexFront,
                tileIndexFront,
                tileIndexFront,
                tileIndexTop,
                tileIndexBottom,
                BlockRepository.Blocks[blockId].BlockColors));
        }

        private VertexBuffer GenerateBlockVertices(
            int tileIndexFront,
            int tileIndexBack,
            int tileIndexLeft,
            int tileIndexRight,
            int tileIndexTop,
            int tileIndexBottom,
            Vector4[] blockColors)
        {
            Tessellator t = Tessellator.Instance;
            t.StartDrawingTiledQuads();
            Vector4 c1, c2, c3, c4, c5, c6;
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
            t.ArrayIndex = tileIndexFront;
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
            t.ArrayIndex = tileIndexTop;
            normal = new Vector3(0, 1, 0);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 1f, 1.0f), c5 * s1, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 1f, vz + 0f, 1.0f), c5 * s2, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 0f, 1.0f), c5 * s3, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 1f, vz + 1f, 1.0f), c5 * s4, normal);
            t.ArrayIndex = tileIndexBottom;
            normal = new Vector3(0, -1, 0);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 0f, 1.0f), c6 * s1, normal);
            t.AddVertexWithColor(new Vector4(vx + 0f, vy + 0f, vz + 1f, 1.0f), c6 * s2, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 1f, 1.0f), c6 * s3, normal);
            t.AddVertexWithColor(new Vector4(vx + 1f, vy + 0f, vz + 0f, 1.0f), c6 * s4, normal);
            return t.GetVertexBuffer();
        }

        private void DefineBlock(int blockid, string top, string side, string bottom)
        {
            topBlockTextures[blockid] = indexMap[top];
            sideBlockTextures[blockid] = indexMap[side];
            bottomBlockTextures[blockid] = indexMap[bottom];
            BuildBlockVertexBuffer(blockid);

            BuildItemVertexBuffer(blockid, side);
        }

        private void LoadAllTileTextures()
        {
            Device device = Tessellator.Instance.Device;
            List<Texture2D> textures = new List<Texture2D>();
            List<string> allFiles = Directory.GetFiles("01.Frontend/Textures/Blocks/", "*.png").ToList();
            allFiles.AddRange(Directory.GetFiles("01.Frontend/Textures/Items/", "*.png"));

            ImageLoadInformation info = new ImageLoadInformation()
            {
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                FilterFlags = FilterFlags.None,
                Format = SlimDX.DXGI.Format.R8G8B8A8_UNorm,
                MipFilterFlags = FilterFlags.Point,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Default,
                MipLevels = 2
            };

            foreach (string filename in allFiles)
            {
                textures.Add(Texture2D.FromFile(device, filename, info));
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

        public void Dispose()
        {
            foreach (VertexBuffer vertexBuffer in blockVertexBuffers.Values)
            {
                vertexBuffer.Dispose();
            }
            blockVertexBuffers.Clear();
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

        internal VertexBuffer GetBlockVertexBuffer(int blockId)
        {
            return blockVertexBuffers[blockId];
        }

        internal VertexBuffer GetItemVertexBuffer(int itemId)
        {
            return itemVertexBuffers[itemId];
        }

        internal VertexBuffer GetDestroyBlockVertexBuffer(float breakPercentage)
        {
            int index = (int)(breakPercentage / 10f);
            if (index < 0)
                index = 0;
            else if (index > 9)
                index = 9;
            return destroyBlocks[index];
        }
    }
}
