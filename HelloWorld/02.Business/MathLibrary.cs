using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication7.Business
{
    static class MathLibrary
    {
        public static Random Random = new Random(1234);

        public static int FloorToWorldGrid(float posF)
        {
            int pos = (int)posF;
            return pos > posF ? pos - 1 : pos;
        }
    }
}
