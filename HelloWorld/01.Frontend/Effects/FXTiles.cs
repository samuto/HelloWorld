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
using WindowsFormsApplication7.Frontend;
using WindowsFormsApplication7.Business;

namespace WindowsFormsApplication7._01.Frontend.Effects
{
    class FXTiles : FXBase
    {
        public static FXTiles Instance = new FXTiles();

        public FXTiles()
            : base("Tiles.fx")
        {
            
        }

        protected override int SetLayout(Device device)
        {
            layout = new InputLayout(device, effect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature, new[] {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0), //16
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0), //16
                new InputElement("NORMAL", 0, Format.R32G32B32_Float, 32, 0), //12
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 44, 0),  //8
                new InputElement("PSIZE", 0, Format.R32_Float, 52, 0),  //4
            });
            return 56;
        }

        internal override void WriteVertex(DataStream stream, ref Tessellator.VS_IN vsin)
        {
            stream.Write(vsin.Vertex);
            stream.Write(vsin.Color);
            stream.Write(vsin.Normal);
            stream.Write(vsin.Uv);
            stream.Write(vsin.TextureIndex);
        }

        internal override void Apply(SlimDX.Direct3D11.Buffer vertices)
        {
            Device device = Tessellator.Instance.Device;
            device.ImmediateContext.InputAssembler.InputLayout = layout;
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, stride, 0));

            effect.GetVariableByName("gWorld").AsMatrix().SetMatrix(Camera.Instance.World);
            effect.GetVariableByName("gView").AsMatrix().SetMatrix(Camera.Instance.View);
            effect.GetVariableByName("gProj").AsMatrix().SetMatrix(Camera.Instance.Projection);
            effect.GetVariableByName("textureArray").AsResource().SetResource(BlockTextures.Instance.View);
            float timeOfDay = World.Instance.TimeOfDay;
            Vector3 lightDirection = new Vector3(0, 0, 0);
            DayWatch watch = DayWatch.Now;
            if (watch.IsDay)
            {
                lightDirection = -watch.SunPosition;
            }
            effect.GetVariableByName("lightDirection").AsVector().Set(lightDirection);
            effect.GetTechniqueByIndex(TechniqueIndex).GetPassByIndex(0).Apply(device.ImmediateContext);
        }
    }
}
