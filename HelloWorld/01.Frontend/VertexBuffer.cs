using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication7.Frontend
{
    class VertexBuffer
    {
        public SlimDX.Direct3D11.Buffer Vertices;
        public int VertexCount = 0;
        public bool Disposed = false;

        public VertexBuffer()
        {
        }

        internal void Dispose()
        {
            if(Vertices == null)
                return;
            if (!Vertices.Disposed)
                Vertices.Dispose();
            Vertices = null;
            VertexCount = 0;
        }

        internal static void Dispose(ref VertexBuffer buffer)
        {
            if (buffer != null)
            {
                buffer.Dispose();
                buffer = null;
            }
        }
    }
}
