using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D11;
using WindowsFormsApplication7.Frontend;
using WindowsFormsApplication7.Business;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D11.Device;

namespace WindowsFormsApplication7._01.Frontend.Effects
{
    class FXSimple : FXBase
    {
        public static FXSimple Instance = new FXSimple();

        public FXSimple()
            : base("simple.fx")
        {
        }

        protected override int SetLayout(Device device)
        {
            layout = new InputLayout(device, effect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature, new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0) 
            });
            return 16 * 2;
        }

        internal override void WriteVertex(SlimDX.DataStream stream, ref Tessellator.VS_IN vsin)
        {
            stream.Write(vsin.Vertex);
            stream.Write(vsin.Color);
        }

        internal override void Apply(SlimDX.Direct3D11.Buffer vertices)
        {
            Device device = t.Device;
            device.ImmediateContext.InputAssembler.InputLayout = layout;
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, stride, 0));

            effect.GetVariableByName("gWorld").AsMatrix().SetMatrix(Camera.Instance.World);
            effect.GetVariableByName("gView").AsMatrix().SetMatrix(Camera.Instance.View);
            effect.GetVariableByName("gProj").AsMatrix().SetMatrix(Camera.Instance.Projection);
            effect.GetTechniqueByIndex(TechniqueIndex).GetPassByIndex(0).Apply(device.ImmediateContext);
        }
    }
}
