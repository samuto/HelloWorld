using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.Business;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Sun : Entity
    {
        public Sun()
            : base(new Vector4(1, 1, 0, 1))
        {
            float size = 8f;
            AABB = new AxisAlignedBoundingBox(new Vector3(-size, -2f * size, -size), new Vector3(size, 0, size));
        }

        internal override void Update()
        {
            DayWatch watch = DayWatch.Now;
            PrevPosition = Position;
            Position = watch.SunPosition * 180f;
            float amount = watch.SunHeight;
            Color4 c;
            if (watch.IsDay)
            {
                if (amount < 0.4)
                    c = Color4.Lerp(new Color4(1, 1, 1, 0), new Color4(1, 1, 0.2f, 0), 1 - (amount / 0.4f));
                else
                    c = Color4.Lerp(new Color4(1, 1, 1, 0.8f), new Color4(1, 1, 1, 0), 1 - (amount - 0.4f) / 0.6f);
            }
            else
                c = new Color4(1, 1, 0.2f, 0);
            Color = c.ToVector4();

            Position.X += World.Instance.Player.Position.X;
            Position.Y += World.Instance.Player.Position.Y;
            Position.Z += World.Instance.Player.Position.Z;
        }

    }
}
