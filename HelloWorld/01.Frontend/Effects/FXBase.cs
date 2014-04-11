using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Frontend;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using SlimDX.D3DCompiler;
using Device = SlimDX.Direct3D11.Device;

namespace WindowsFormsApplication7._01.Frontend.Effects
{
    abstract class FXBase
    {
        public int TechniqueIndex;

        protected Effect effect;
        protected ShaderBytecode bytecode;
        protected InputLayout layout;
        protected int stride;
        protected Tessellator t = Tessellator.Instance;

        public FXBase(string effectFile)
        {
            try
            {
                Load(effectFile);
            }
            catch
            {
                throw;
            }
        }

        private void Load(string effectFile)
        {
            Device device = t.Device;
            bytecode = ShaderBytecode.CompileFromFile("01.frontend/shaders/" + effectFile, "fx_5_0", ShaderFlags.None, EffectFlags.None);
            effect = new Effect(device, bytecode);
            stride = SetLayout(device);
        }

        internal void Dispose()
        {
            bytecode.Dispose();
            effect.Dispose();
            layout.Dispose();
        }

        internal int GetStride()
        {
            return stride;
        }

        protected abstract int SetLayout(Device device);
        internal abstract void WriteVertex(DataStream stream, ref Tessellator.VS_IN vsin);
        internal abstract void Apply(SlimDX.Direct3D11.Buffer vertices);
    }
}
