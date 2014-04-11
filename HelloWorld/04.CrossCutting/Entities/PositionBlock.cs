using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    struct PositionBlock
    {
        public int X;
        public int Y;
        public int Z;

        public PositionBlock(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        internal static PositionBlock FromVector(SlimDX.Vector4 vector4)
        {
            return new PositionBlock((int)vector4.X, (int)vector4.Y, (int)vector4.Z);
        }

        internal bool SameAs(PositionBlock otherPos)
        {
            return X == otherPos.X && Z == otherPos.Z && Y == otherPos.Y;
        }
    }
}
