using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication7.Business.Geometry
{
    static class MathLibrary
    {
        public static Random GlobalRandom = new Random(1234);
        public const float Math2Pi = (float)Math.PI * 2f;

        public static int FloorToWorldGrid(float posF)
        {
            int pos = (int)posF;
            return pos > posF ? pos - 1 : pos;
        }

        internal static float RandomSign()
        {
            return GlobalRandom.NextDouble() < 0.5 ? -1 : 1;
        }
    }
}
