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
using WindowsFormsApplication7.Business;

namespace WindowsFormsApplication7.Frontend
{
    class Tessellator
    {
        public static Tessellator Instance = new Tessellator();
        public int VertexCount = 0;
        public string ActiveTexture = "bitmap1";
        private string activeEffect = "texture";
        public int activeEffectTechnique = 0;
        private DataStream stream;
        private Device device;
        private int vertexAddCount = 0;
        private PrimitiveTopology mode = PrimitiveTopology.TriangleList;
        private TextureList textureList;
        private EffectList effectList;
        private VS_IN vertex0;
        private VS_IN vertex2;
        private Vector2[] uvVertices = new Vector2[]{
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(1,1),
            new Vector2(0,1),
        };
        private int uvIndex = 0;
        private class VS_IN
        {
            public Vector4 Vertex;
            public Vector4 Color;
            public Vector2 Uv;
        }

        public Tessellator()
        {
        }

        public void Initialize(int size, Device device)
        {
            this.device = device;
            textureList = new TextureList(device);
            effectList = new EffectList(device);
            stream = new DataStream(size, true, true);

            textureList.Load("bitmap1");
            textureList.Load("bitmap2");
            textureList.Load("sky");
            textureList.Load("ascii");

            effectList.Load("simple");
            effectList.Load("texture");
        }

        public void Draw()
        {
            if (VertexCount > 0)
            {
                SlimDX.Direct3D11.Buffer vertices = GetDrawBuffer();
                DrawBuffer(vertices);
                vertices.Dispose();
            }
            Reset();
        }

        public void Draw(SlimDX.Direct3D11.Buffer vertices, int count)
        {
            VertexCount = count;
            DrawBuffer(vertices);
            Reset();
        }

        public SlimDX.Direct3D11.Buffer GetDrawBuffer()
        {
            stream.Position = 0;
            if (VertexCount == 0)
                return null;
            SlimDX.Direct3D11.Buffer vertices = new SlimDX.Direct3D11.Buffer(device, stream, new BufferDescription()
                {
                    BindFlags = BindFlags.VertexBuffer,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None,
                    SizeInBytes = VertexCount * effectList.GetStride(activeEffect),
                    Usage = ResourceUsage.Default
                });
            return vertices;
        }


        private void DrawBuffer(SlimDX.Direct3D11.Buffer vertices)
        {
            if (VertexCount == 0)
                return;
            Effect effect = effectList.ApplyEffect(activeEffect, activeEffectTechnique, vertices, textureList.GetShaderResourceView(ActiveTexture));
            device.ImmediateContext.InputAssembler.PrimitiveTopology = mode;
            device.ImmediateContext.Draw(VertexCount, 0);

        }

        private void Reset()
        {
            // reset vertex info
            VertexCount = 0;
            vertexAddCount = 0;
            stream.Position = 0;
            uvIndex = 0;
        }


        internal void AddVertexWithColor(Vector4 vertex, Vector4 color)
        {
            int stride = effectList.GetStride(activeEffect);

            vertexAddCount++;
            Vector2 texture = uvVertices[uvIndex++];
            uvIndex = uvIndex % 4;
            VS_IN vsin = new VS_IN() { Vertex = vertex, Color = color, Uv = texture };
            VertexCount++;
            stream.Write(vsin.Vertex);
            stream.Write(vsin.Color);
            stream.Write(vsin.Uv);
            if (mode == PrimitiveTopology.TriangleList)
            {
                if (vertexAddCount % 4 == 1)
                {
                    vertex0 = vsin;
                }

                if (vertexAddCount % 4 == 3)
                {
                    vertex2 = vsin;
                }

                if (vertexAddCount % 4 == 0)
                {
                    VertexCount += 2;
                    stream.Write(vertex0.Vertex);
                    stream.Write(vertex0.Color);
                    stream.Write(vertex0.Uv);
                    stream.Write(vertex2.Vertex);
                    stream.Write(vertex2.Color);
                    stream.Write(vertex2.Uv);
                }
            }

            if ((VertexCount * stride) >= stream.Length - 32 * stride && vertexAddCount % 4 == 0)
            {
                Draw();
            }
        }

        public void StartDrawingQuadsWithFog()
        {
            SetTextureQuad(new Vector2(0, 0), 1f, 1f);
            StartDrawing("bitmap1", "texture", 0, PrimitiveTopology.TriangleList);
        }

        internal void StartDrawingLines()
        {
            StartDrawing("bitmap1", "texture", 3, PrimitiveTopology.LineList);
        }

        public void StartDrawingQuadsNoFog(string activeTexture)
        {
            StartDrawing(activeTexture, "texture", 1, PrimitiveTopology.TriangleList);
        }

        public void StartDrawingQuadsWithAlphaBlend(string activeTexture)
        {
            StartDrawing(activeTexture, "texture", 2, PrimitiveTopology.TriangleList);
        }

        private void StartDrawing(string activeTexture, string activeEffect, int technique, PrimitiveTopology mode)
        {
            ActiveTexture = activeTexture;
            this.activeEffect = activeEffect;
            activeEffectTechnique = technique;
            this.mode = mode;
            Reset();
        }

        public void Dispose()
        {
            effectList.Dispose();
            textureList.Dispose();
            stream.Dispose();
        }

        internal void SetTextureQuad(Vector2 corner, float width, float height)
        {
            uvIndex = 0;
            uvVertices[1].X = corner.X;
            uvVertices[1].Y = corner.Y;

            uvVertices[2].X = corner.X+width;
            uvVertices[2].Y = corner.Y;

            uvVertices[3].X = corner.X+width;
            uvVertices[3].Y = corner.Y+height;

            uvVertices[0].X = corner.X;
            uvVertices[0].Y = corner.Y+height;
        }
    }
}
