using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.Business;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Player : Entity
    {
        public int SelectedBlockId = 0;

        public Player()
            : base(new Vector4(1, 0, 0, 1))
        {
            AABB = new AxisAlignedBoundingBox(new Vector3(-0.4f, 0f, -0.4f), new Vector3(0.4f, 1.7f, 0.4f));
            EyePosition = new Vector3(0, AABB.Max.Y-0.1f, 0);
            Speed = 0.15f;
        }

    }
}
