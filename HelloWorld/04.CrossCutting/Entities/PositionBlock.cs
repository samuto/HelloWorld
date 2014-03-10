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

        internal PositionBlock Add(int x, int y, int z)
        {
            X += x;
            Y += y;
            Z += z;
            return this;
        }
    }
}
