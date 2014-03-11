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

namespace WindowsFormsApplication7.Frontend
{
    class GlobalRenderer
    {
        public Color4 BackgroundColor = new Color4(Color.LightBlue);
        public static GlobalRenderer Instance = new GlobalRenderer();
        private ChunkCacheRenderer chunkCacheRenderer = new ChunkCacheRenderer();
        private EntityRenderer entityRenderer = new EntityRenderer();
        private RenderTargetView renderView;
        private DepthStencilView depthView;
        private Texture2D backBuffer;
        private Texture2D depthBuffer;
        private Device device;
        private SwapChain swapChain;

        internal void Render(float partialTicks)
        {
            Profiler p = Profiler.Instance;
            p.StartSection("init");
            device.ImmediateContext.ClearRenderTargetView(renderView, BackgroundColor);
            device.ImmediateContext.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth, 1.0f, 0);


            p.EndStartSection("cached");
            // Render 3d stuff
            Setup3dCamera(partialTicks);

            // 3D: render chunks
            chunkCacheRenderer.Render();

            p.EndStartSection("entities");
            // 3D: render entities
            entityRenderer.Render(partialTicks);

            p.EndStartSection("HUD");
            // Render 2d stuff
            Setup2dCamera();

            // 2D: render Heads up display 
            // ....

            p.EndSection();

           
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

        private void Setup3dCamera(float partialTicks)
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
            Camera.Instance.Update(partialTicks);
        }

        internal void Initialize(SlimDX.Windows.RenderForm form)
        {
            // setup directx
            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out device, out swapChain);
            
            Factory factory = swapChain.GetParent<Factory>();
            factory.SetWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

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
            device.ImmediateContext.Rasterizer.SetViewports(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 0.01f));

            Tessellator.Instance.Initialize(1024 * 1024 * 10, device);

        }

        internal void Dispose()
        {
            depthView.Dispose();
            renderView.Dispose();
            depthBuffer.Dispose();
            backBuffer.Dispose();
            device.Dispose();
            swapChain.Dispose();
        }

        internal void RenderProfiler()
        {
            Profiler p = Profiler.Instance;
            FontRenderer f = FontRenderer.Instance;
            string report = p.Report();
            float y = TheGame.Instance.Height - 20;
            using (StringReader sr = new StringReader(report))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    f.RenderTextShadow(line, 0, y);
                    y -= f.LineHeight;
                }
            }
        }
    }
}
