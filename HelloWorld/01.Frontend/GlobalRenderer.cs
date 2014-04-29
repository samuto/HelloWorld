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
using WindowsFormsApplication7.Frontend;
using WindowsFormsApplication7.Business;
using System.IO;
using WindowsFormsApplication7.Business.Profiling;
using WindowsFormsApplication7.Frontend.Gui;
using WindowsFormsApplication7.CrossCutting;
using WindowsFormsApplication7.CrossCutting.Entities;

namespace WindowsFormsApplication7.Frontend
{
    class GlobalRenderer
    {
        public Color4 BackgroundColor = new Color4(Color.White);
        public static GlobalRenderer Instance = new GlobalRenderer();
        private ChunkCacheRenderer chunkCacheRenderer;
        private RenderTargetView renderView;
        private DepthStencilView depthView;
        private Texture2D backBuffer;
        private Texture2D depthBuffer;
        private Device device;
        private SwapChain swapChain;
        private HeadUpDisplay headUpDisplay;
        private Tessellator t = Tessellator.Instance;
        private Profiler p = Profiler.Instance;

        internal void InitializeRenderer()
        {
            // setup directx
            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(TheGame.Instance.Width, TheGame.Instance.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = TheGame.Instance.FormHandle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput,
            };
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out device, out swapChain);

            Factory factory = swapChain.GetParent<Factory>();
            factory.SetWindowAssociation(TheGame.Instance.FormHandle, WindowAssociationFlags.IgnoreAll);

            backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            renderView = new RenderTargetView(device, backBuffer);


            // setup depth buffer
            Format depthFormat = Format.D32_Float;
            Texture2DDescription depthBufferDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = depthFormat,
                Height = TheGame.Instance.Height,
                Width = TheGame.Instance.Width,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };

            depthBuffer = new Texture2D(device, depthBufferDesc);

            DepthStencilViewDescription dsViewDesc = new DepthStencilViewDescription
            {
                ArraySize = 0,
                Format = depthFormat,
                Dimension = DepthStencilViewDimension.Texture2D,
                MipSlice = 0,
                Flags = 0,
                FirstArraySlice = 0
            };

            depthView = new DepthStencilView(device, depthBuffer, dsViewDesc);

            DepthStencilStateDescription dsStateDesc = new DepthStencilStateDescription()
            {
                IsDepthEnabled = true,
                IsStencilEnabled = false,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Less,
            };

            DepthStencilState depthState = DepthStencilState.FromDescription(device, dsStateDesc);



            // setup render targets
            device.ImmediateContext.OutputMerger.DepthStencilState = depthState;
            device.ImmediateContext.OutputMerger.SetTargets(depthView, renderView);
            device.ImmediateContext.Rasterizer.SetViewports(new Viewport(0, 0, TheGame.Instance.Width, TheGame.Instance.Height, 0.0f, 0.01f));

            t.Initialize(1024 * 1024 * 10, device);
            TileTextures.Instance.Initialize();
        }

        internal void ClearTarget()
        {
            DayWatch watch = DayWatch.Now;
            BackgroundColor = Color4.Lerp(new Color4(0.8f, 0.8f, 1f), new Color4(0f, 0f, 0f), 1f - watch.SunHeight);
            device.ImmediateContext.ClearRenderTargetView(renderView, BackgroundColor);
            device.ImmediateContext.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
        }

        internal void Render(float partialStep)
        {
            // Render 3d stuff
            Setup3dCamera(partialStep);

            // 3D: render chunks
            p.StartSection("chunkspass1");
            chunkCacheRenderer.Render();

            // 3D: render entities
            p.EndStartSection("entities");
            foreach (Entity entity in World.Instance.globalEntities)
            {
                EntityRenderer renderer = GetRenderer(entity);
                if (renderer != null)
                {
                    renderer.Render(partialStep);
                    renderer.Dispose();
                }
            }
            RenderPlayerRayImpact(partialStep);
            p.EndStartSection("chunkspass2");
            chunkCacheRenderer.RenderPass2();


            // Render 2d stuff
            p.EndStartSection("2d");
            Setup2dCamera();
            RenderCrossHair();
            RenderHeadUpDisplay();
            p.EndSection();
        }

        private EntityRenderer GetRenderer(Entity entity)
        {
            Type t = entity.GetType();
            if (t == typeof(Player))
                return new PlayerRenderer(entity);
            else
                return new EntityRenderer(entity);
        }

        private void RenderHeadUpDisplay()
        {
            if (TheGame.Instance.Mode == TheGame.GameMode.InGame)
            {
                headUpDisplay.Render();
            }
        }

        private void RenderCrossHair()
        {
            t.ResetTransformation();
            t.StartDrawingLines();
            Vector4 c = new Vector4(1, 1, 1, 1);
            Vector4 center = new Vector4(TheGame.Instance.Width / 2, TheGame.Instance.Height / 2, 0, 1);
            float size = 15;
            t.AddVertexWithColor(new Vector4(center.X - size, center.Y, 0, 1), c);
            t.AddVertexWithColor(new Vector4(center.X + size, center.Y, 0, 1), c);
            t.AddVertexWithColor(new Vector4(center.X, center.Y - size, 0, 1), c);
            t.AddVertexWithColor(new Vector4(center.X, center.Y + size, 0, 1), c);
            t.Draw();
        }

        private void RenderPlayerRayImpact(float partialStep)
        {
            if (!World.Instance.PlayerVoxelTrace.Hit)
                return;
            t.ResetTransformation();
            //t.StartDrawingTiledQuadsWTF();
            t.StartDrawingTiledQuadsPass2();
            Vector4 buildPos = World.Instance.PlayerVoxelTrace.ImpactPosition;
            t.Translate.X = buildPos.X + 0.5f;
            t.Translate.Y = buildPos.Y + 0.5f;
            t.Translate.Z = buildPos.Z + 0.5f;
            float s = 1.01f;
            t.Scale = new Vector3(s, s, s);
            t.Draw(TileTextures.Instance.GetSelectionBlockVertexBuffer());
            return;
        }

        internal void Commit()
        {
            swapChain.Present(0, PresentFlags.None);
        }


        internal void Setup2dCamera()
        {
            Camera.Instance.Enable3d = false;
            DepthStencilStateDescription dsStateDesc = new DepthStencilStateDescription()
            {
                IsDepthEnabled = false,
                IsStencilEnabled = false,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Less,
            };
            DepthStencilState depthState = DepthStencilState.FromDescription(device, dsStateDesc);
            device.ImmediateContext.OutputMerger.DepthStencilState = depthState;
            Camera.Instance.Update(0);
        }

        private void Setup3dCamera(float partialStep)
        {
            Camera.Instance.Enable3d = true;
            DepthStencilStateDescription dsStateDesc = new DepthStencilStateDescription()
            {
                IsDepthEnabled = true,
                IsStencilEnabled = false,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Less,
            };
            DepthStencilState depthState = DepthStencilState.FromDescription(device, dsStateDesc);
            device.ImmediateContext.OutputMerger.DepthStencilState = depthState;
            Camera.Instance.Update(partialStep);
        }

        internal void ToggleFullScreen()
        {
            bool isFull = swapChain.Description.IsWindowed;
            swapChain.SetFullScreenState(isFull, null);
        }

        internal void InitializeWorld()
        {
            chunkCacheRenderer = new ChunkCacheRenderer();
            headUpDisplay = new HeadUpDisplay();
            Camera.Instance.AttachTo(World.Instance.Player);
        }

        internal void Dispose()
        {
            profileVertexBuffer.Dispose();
            depthView.Dispose();
            renderView.Dispose();
            depthBuffer.Dispose();
            backBuffer.Dispose();
            device.Dispose();
            swapChain.SetFullScreenState(false, null);
            swapChain.Dispose();
        }


        internal void RenderProfiler()
        {
            if (profileVertexBuffer == null)
                return;
            t.ResetTransformation();
            t.StartDrawingAlphaTexturedQuads("ascii");
            t.Draw(profileVertexBuffer);
        }

        private VertexBuffer profileVertexBuffer;
        internal void ProfilerSnapshot()
        {
            VertexBuffer.Dispose(ref profileVertexBuffer);

            t.ResetTransformation();
            FontRenderer f = FontRenderer.Instance;
            Player player = World.Instance.Player;
            f.BeginBatch();
            f.CharScale = 1;
            string report = p.Report();
            float y = TheGame.Instance.Height - FontRenderer.Instance.LineHeight;
            using (StringReader sr = new StringReader(report))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    f.RenderTextShadow(line, 0, y);
                    y -= f.LineHeight;
                }
            }

            // log...
            y -= f.LineHeight;

            if (World.Instance.PlayerVoxelTrace.Hit)
            {
                Vector4 impactpos = World.Instance.PlayerVoxelTrace.ImpactPosition;
                string line = string.Format(GetVectorAsString("impactpos", impactpos));
                f.RenderTextShadow(line, 0, y);
                y -= f.LineHeight;
            }
            else
            {
                f.RenderTextShadow("impactpos", 0, y);
                y -= f.LineHeight;

                Vector3 pos = new Vector3(player.Position.X, player.Position.Y, player.Position.Z);
                if (pos.Y > Chunk.MaxSizeY - 1)
                    pos.Y = Chunk.MaxSizeY - 1;
                else if (pos.Y < 0)
                    pos.Y = 0;
                PositionChunk chunkPos = PositionChunk.CreateFrom(pos);
                Vector3 chunkPos3 = new Vector3(chunkPos.X, chunkPos.Y, chunkPos.Z);
                f.RenderTextShadow(string.Format(GetVectorAsString("chunkpos", chunkPos3)), 0, y);
                y -= f.LineHeight;
                ChunkCache cache = World.Instance.GetCachedChunks();
                Chunk c = cache.GetChunk(chunkPos);
                if (c != null)
                {
                    f.RenderTextShadow(string.Format("chunk.Stage      = {0}", c.Stage.ToString()), 0, y);
                    y -= f.LineHeight;
                    f.RenderTextShadow(string.Format("chunk.col.stage  = {0}", c.Column.Stage.ToString()), 0, y);
                    y -= f.LineHeight;
                    f.RenderTextShadow(string.Format("chunk.col.active = {0}", c.Column.Active.ToString()), 0, y);
                    y -= f.LineHeight;
                    f.RenderTextShadow(string.Format("cache.alleighbors= {0}", cache.AllNeighborColumns(c.Column).Where(cc => cc != null).Count()), 0, y);
                    y -= f.LineHeight;
                    
                    List<Chunk> chunks = new List<Chunk>();
                    for (int i = 0; i < 8; i++)
                    {
                        chunks.Add(cache.GetChunk(new PositionChunk(c.Position.X, i, c.Position.Z)));

                    }
                }
            }
            f.RenderTextShadow(GetVectorAsString("direction", player.Direction), 0, y);

            y -= f.LineHeight;
            f.RenderTextShadow(GetVectorAsString("position", player.Position), 0, y);

            y -= f.LineHeight;
            y -= f.LineHeight;
            string[] lastLines = Log.Instance.Last(70);
            foreach (string line in lastLines)
            {
                f.RenderTextShadow(line, 0, y);
                y -= f.LineHeight;
            }
            f.StopBatch();
            profileVertexBuffer = t.GetVertexBuffer();
        }

        private string GetVectorAsString(string name, Vector4 vector4)
        {
            return GetVectorAsString(name, new Vector3(vector4.X, vector4.Y, vector4.Z));
        }
        private string GetVectorAsString(string name, Vector3 vector3)
        {
            return string.Format("{0}: x={1:+0000.00;-0000.00}  z={2:+0000.00;-0000.00}  y={3:+0000.00;-0000.00}",
                        name.PadRight(10),
                        vector3.X,
                        vector3.Y,
                        vector3.Z);
        }

        internal void RenderGui(float partialStep)
        {
            t.Scale = GuiScaling.Instance.Scale3;
            t.Translate = GuiScaling.Instance.Translate3;
            TheGame.Instance.ActiveGui.Render(partialStep);

            FontRenderer.Instance.CharScale = GuiScaling.Instance.Scale;
            TheGame.Instance.Cursor.Render(partialStep);
            FontRenderer.Instance.CharScale = 1f;
        }




    }
}
