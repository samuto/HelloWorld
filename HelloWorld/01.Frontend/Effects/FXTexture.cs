using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Frontend;
using WindowsFormsApplication7.Business;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using SlimDX.D3DCompiler;
using Device = SlimDX.Direct3D11.Device;
namespace WindowsFormsApplication7._01.Frontend.Effects
{
    class FXTexture : FXBase
    {
        public static FXTexture Instance = new FXTexture();
        public float GlobalScale = 1f;

        public FXTexture()
            : base("texture.fx")
        {
        }

        protected override int SetLayout(Device device)
        {
            layout = new InputLayout(device, effect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature, new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 32, 0) 
            });
            return 2 * 16 + 8;
        }

        internal override void WriteVertex(DataStream stream, ref Tessellator.VS_IN vsin)
        {
            stream.Write(vsin.Vertex);
            stream.Write(vsin.Color);
            stream.Write(vsin.Uv);
        }

        internal override void Apply(SlimDX.Direct3D11.Buffer vertices)
        {
            Device device = Tessellator.Instance.Device;
            device.ImmediateContext.InputAssembler.InputLayout = layout;
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, stride, 0));
            effect.GetVariableByName("gWorld").AsMatrix().SetMatrix(Camera.Instance.World);
            effect.GetVariableByName("gView").AsMatrix().SetMatrix(Camera.Instance.View);
            effect.GetVariableByName("gProj").AsMatrix().SetMatrix(Camera.Instance.Projection);
            effect.GetVariableByName("intexture").AsResource().SetResource(Tessellator.Instance.ActiveTexture);
            effect.GetVariableByName("eye").AsVector().Set(Camera.Instance.EyePosition);
            effect.GetVariableByName("fogNear").AsScalar().Set((float)GameSettings.ViewRadius - 32f);
            effect.GetVariableByName("fogFar").AsScalar().Set((float)GameSettings.ViewRadius);
            effect.GetVariableByName("fogColor").AsVector().Set(GlobalRenderer.Instance.BackgroundColor);
            effect.GetTechniqueByIndex(TechniqueIndex).GetPassByIndex(0).Apply(device.ImmediateContext);
        }
    }
}
