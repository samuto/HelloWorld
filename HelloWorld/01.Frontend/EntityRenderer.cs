using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities;
using SlimDX;
using SlimDX.Direct3D11;
using WindowsFormsApplication7._01.Frontend;

namespace WindowsFormsApplication7.Frontend
{
    class EntityRenderer
    {
        internal void Render(float partialTicks)
        {
            RenderEntity(World.Instance.FlyingCamera, partialTicks);
            RenderEntity(World.Instance.Player, partialTicks);
        }

        private void RenderEntity(Entity entity, float partialTicks)
        {
            Tessellator t = Tessellator.Instance;
            t.StartDrawingQuadsWithFog();
            Vector3 position = Interpolate.Position(entity, partialTicks);
            Vector4 c = entity.Color;
            Vector3 min = entity.AABB.GetMinFromPosition(position);
            Vector3 max = entity.AABB.GetMaxFromPosition(position);
            min = entity.AABB.Min;
            max = entity.AABB.Max;
            Vector4[] v = new Vector4[] {
            new Vector4(min.X, min.Y, min.Z,1),
            new Vector4(max.X, min.Y, min.Z,1),
            new Vector4(max.X, min.Y, max.Z,1),
            new Vector4(min.X, min.Y, max.Z,1),
            new Vector4(min.X, max.Y, min.Z,1),
            new Vector4(max.X, max.Y, min.Z,1),
            new Vector4(max.X, max.Y, max.Z,1),
            new Vector4(min.X, max.Y, max.Z,1),
            };

            // left
            t.AddVertexWithColor(v[0], c * 0.9f);
            t.AddVertexWithColor(v[4], c * 0.9f);
            t.AddVertexWithColor(v[7], c * 0.9f);
            t.AddVertexWithColor(v[3], c * 0.9f);

            //front
            t.AddVertexWithColor(v[3], c * 0f);
            t.AddVertexWithColor(v[7], c * 0f);
            t.AddVertexWithColor(v[6], c * 0.8f);
            t.AddVertexWithColor(v[2], c * 0.8f);

            //right
            t.AddVertexWithColor(v[2], c * 0.7f);
            t.AddVertexWithColor(v[6], c * 0.7f);
            t.AddVertexWithColor(v[5], c * 0.7f);
            t.AddVertexWithColor(v[1], c * 0.7f);

            //back
            t.AddVertexWithColor(v[1], c * 0.6f);
            t.AddVertexWithColor(v[5], c * 0.6f);
            t.AddVertexWithColor(v[4], c * 0.6f);
            t.AddVertexWithColor(v[0], c * 0.6f);

            //top
            t.AddVertexWithColor(v[4], c);
            t.AddVertexWithColor(v[5], c);
            t.AddVertexWithColor(v[6], c);
            t.AddVertexWithColor(v[7], c);

            //bottom
            t.AddVertexWithColor(v[0], c * 0.4f);
            t.AddVertexWithColor(v[3], c * 0.4f);
            t.AddVertexWithColor(v[2], c * 0.4f);
            t.AddVertexWithColor(v[1], c * 0.4f);

            Camera.Instance.World = Matrix.Multiply(Matrix.RotationYawPitchRoll(entity.Yaw, 0, 0), Matrix.Translation(position));
            t.Draw();
            Camera.Instance.World = Matrix.Identity;

            
        }
    }
}
