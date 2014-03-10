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
    class EffectList
    {
        private Device device;
        private Dictionary<string, EffectWrapper> effects = new Dictionary<string, EffectWrapper>();

        private class EffectWrapper
        {
            public ShaderBytecode Bytecode;
            public Effect Effect;
            public EffectTechnique Technique;
            public EffectPass Pass;
            public InputLayout Layout;
            public int Stride;

            public void SetTechniqueAndPass(int technique, int pass)
            {
                Technique = Effect.GetTechniqueByIndex(technique);
                Pass = Technique.GetPassByIndex(0);
            }

        }

        public EffectList(Device device)
        {
            this.device = device;
        }

        public void Load(string name)
        {
            EffectWrapper wrapper = new EffectWrapper();
            wrapper.Bytecode = ShaderBytecode.CompileFromFile("01.frontend/shaders/" + name + ".fx", "fx_5_0", ShaderFlags.None, EffectFlags.None);
            wrapper.Effect = new Effect(device, wrapper.Bytecode);
            wrapper.SetTechniqueAndPass(0, 0);
            
            wrapper.Layout = new InputLayout(device, wrapper.Pass.Description.Signature, new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 32, 0) 
            });
            wrapper.Stride = 2 * 16 + 8;
            effects.Add(name, wrapper);

        }

        internal void Dispose()
        {
            foreach (EffectWrapper wrapper in effects.Values)
            {
                wrapper.Bytecode.Dispose();
                wrapper.Effect.Dispose();
                wrapper.Layout.Dispose();
            }

        }

        internal Effect ApplyEffect(string name, int technique, SlimDX.Direct3D11.Buffer vertices, ShaderResourceView view)
        {
            EffectWrapper wrapper = effects[name];
            Effect effect = wrapper.Effect;
            device.ImmediateContext.InputAssembler.InputLayout = wrapper.Layout;
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, wrapper.Stride, 0));

            effect.GetVariableByName("gWorld").AsMatrix().SetMatrix(Camera.Instance.World);
            effect.GetVariableByName("gView").AsMatrix().SetMatrix(Camera.Instance.View);
            effect.GetVariableByName("gProj").AsMatrix().SetMatrix(Camera.Instance.Projection);
            effect.GetVariableByName("intexture").AsResource().SetResource(view);
            effect.GetVariableByName("eye").AsVector().Set(Camera.Instance.EyePosition);
            effect.GetVariableByName("fogNear").AsScalar().Set((float)GameSettings.ViewRadius - 20f);
            effect.GetVariableByName("fogFar").AsScalar().Set((float)GameSettings.ViewRadius);
            effect.GetVariableByName("fogColor").AsVector().Set(GlobalRenderer.Instance.BackgroundColor);
            wrapper.SetTechniqueAndPass(technique, 0);
            wrapper.Pass.Apply(device.ImmediateContext);

            return wrapper.Effect;
        }

        internal int GetStride(string name)
        {
            return effects[name].Stride;
        }
   }
}