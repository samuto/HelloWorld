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
                entity.GetRenderer().Render(partialStep);
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
            t.StartDrawingTiledQuadsWTF();
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
            if (profileVertexBuffer != null)
                profileVertexBuffer.Dispose();

            t.ResetTransformation();
            FontRenderer f = FontRenderer.Instance;
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
            y = TheGame.Instance.Height - f.LineHeight * 10f;

            if (World.Instance.PlayerVoxelTrace.Hit)
            {
                string line = string.Format("impact:    x={0}y={1}z={2} ({3})",
                        World.Instance.PlayerVoxelTrace.ImpactPosition.X.ToString().PadRight(5),
                        World.Instance.PlayerVoxelTrace.ImpactPosition.Y.ToString().PadRight(5),
                        World.Instance.PlayerVoxelTrace.ImpactPosition.Z.ToString().PadRight(5),
                        World.Instance.PlayerVoxelTrace.ImpactBlock.Id);
                f.RenderTextShadow(line, 0, y);
                y -= f.LineHeight;
            }
            else
            {
                f.RenderTextShadow("no impact", 0, y);
                y -= f.LineHeight;
            }
            f.RenderTextShadow(string.Format("direction: x={0}z={2}y={1}",
                        World.Instance.Player.Direction.X.ToString("#.##").PadRight(5),
                        World.Instance.Player.Direction.Y.ToString("#.##").PadRight(5),
                        World.Instance.Player.Direction.Z.ToString("#.##").PadRight(5)), 0, y);
            y -= f.LineHeight;
            f.RenderTextShadow(string.Format("position: x={0}z={2}y={1}",
                       World.Instance.Player.Position.X.ToString("#.##").PadRight(5),
                       World.Instance.Player.Position.Y.ToString("#.##").PadRight(5),
                       World.Instance.Player.Position.Z.ToString("#.##").PadRight(5)), 0, y);
            y -= f.LineHeight;
            y -= f.LineHeight;
            string[] lastLines = Log.Instance.Last(30);
            foreach (string line in lastLines)
            {
                f.RenderTextShadow(line, 0, y);
                y -= f.LineHeight;
            }
            f.StopBatch();
            profileVertexBuffer = t.GetVertexBuffer();
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
