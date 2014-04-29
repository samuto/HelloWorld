using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.Business.Geometry;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Moon : EntityPlayer
    {
        public Moon()
            : base(new Vector4(0.8f, 0.8f, 0.8f, 1f))
        {
            float size = 5f;
            AABB = new AxisAlignedBoundingBox(new Vector3(-size, -2f * size, -size), new Vector3(size, 0, size));
        }

        internal override void OnUpdate()
        {
            DayWatch watch = DayWatch.Now;
            PrevPosition = Position;
            Position = watch.MoonPosition * 180f;
            Color4 c = new Color4(1f, 0.8f, 0.8f, 0.8f);
            Color = c.ToVector4();

            Position.X += World.Instance.Player.Position.X;
            Position.Y += 0;
            Position.Z += World.Instance.Player.Position.Z;
        }

    }
}
