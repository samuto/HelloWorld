using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Frontend;
using WindowsFormsApplication7.Business;
using SlimDX;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Block
    {
        public int Id;
        public MaterialEnum Material;
        public Vector4[] BlockColors = new Vector4[6]{
            new Vector4(1,1,1,1),
            new Vector4(1,1,1,1),
            new Vector4(1,1,1,1),
            new Vector4(1,1,1,1),
            new Vector4(1,1,1,1),
            new Vector4(0.6f,0.6f,0.6f,1),
        };

        public Block(int id, MaterialEnum material)
        {
            this.Id = id;
            this.Material = material;
        }

        internal Block AddToRepository()
        {
            BlockRepository.Blocks[Id] = this;
            return this;
        }

        internal Block BlockColor(System.Drawing.Color color)
        {
            Vector4 c = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, 1f);
            BlockColors[0] = c * 0.8f;
            BlockColors[1] = c * 0.8f;
            BlockColors[2] = c * 0.8f;
            BlockColors[3] = c * 0.8f;
            BlockColors[4] = c;
            BlockColors[5] = c;
            return this;
        }

        internal Block TopColor(System.Drawing.Color color)
        {
            Vector4 c = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, 1f);
            BlockColors[4] = c;
            return this;
        }

        internal Block TopBottomColor(System.Drawing.Color color)
        {
            Vector4 c = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, 1f);
            BlockColors[4] = c;
            BlockColors[5] = c;
            return this;
        }

        internal AxisAlignedBoundingBox GetBoundingBox()
        {
            return new AxisAlignedBoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        }
    }
}
