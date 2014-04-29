using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities;
using SlimDX;
using SlimDX.Direct3D11;
using WindowsFormsApplication7._01.Frontend;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.Business.Geometry;

namespace WindowsFormsApplication7.Frontend
{
    class PlayerRenderer : EntityRenderer
    {
        public PlayerRenderer(Entity entity)
            : base(entity)
        {
        }

        internal override void Render(float partialStep)
        {
            t.ResetTransformation();
            Vector3 position = Interpolate.Vector(entity.PrevPosition, entity.Position, partialStep);

            Player p = World.Instance.Player;
            Vector3 pos = position + p.EyePosition;
            int blockId = World.Instance.GetBlock((int)MathLibrary.FloorToWorldGrid(pos.X), (int)MathLibrary.FloorToWorldGrid(pos.Y), (int)MathLibrary.FloorToWorldGrid(pos.Z));
            if (blockId == BlockRepository.Water.Id)
            {
                t.StartDrawingColoredQuads();
                Camera.Instance.World = Matrix.Identity;
                Camera.Instance.World = Matrix.Multiply(Camera.Instance.World, Matrix.Translation(new Vector3(-0.5f, -0.5f, -1.2f)));
                Camera.Instance.World = Matrix.Multiply(Camera.Instance.World, Matrix.RotationYawPitchRoll(p.Yaw - (float)Math.PI, -p.Pitch, 0));
                Camera.Instance.World = Matrix.Multiply(Camera.Instance.World, Matrix.Translation(pos));

                Vector3 normal = new Vector3(0, 0, 1);
                Vector4 c1 = new Vector4(0.2f, 0.2f, 0.4f, 0.5f);
                t.AddVertexWithColor(new Vector4(0f, 0f, 1f, 1.0f), c1, normal);
                t.AddVertexWithColor(new Vector4(0f, 1f, 1f, 1.0f), c1, normal);
                t.AddVertexWithColor(new Vector4(1f, 1f, 1f, 1.0f), c1, normal);
                t.AddVertexWithColor(new Vector4(1f, 0f, 1f, 1.0f), c1, normal);
                t.Draw();
            }

            if (World.Instance.Player.DestroyProgress > 0)
            {
                t.StartDrawingTiledQuads2();
                t.Translate.X = World.Instance.Player.BlockAttackPosition.X + 0.5f;
                t.Translate.Y = World.Instance.Player.BlockAttackPosition.Y + 0.5f;
                t.Translate.Z = World.Instance.Player.BlockAttackPosition.Z + 0.5f;
                float s = 1.005f;
                t.Scale = new Vector3(s, s, s);
                t.Draw(TileTextures.Instance.GetDestroyBlockVertexBuffer(World.Instance.Player.DestroyProgress));
            }
        }

    }
}
