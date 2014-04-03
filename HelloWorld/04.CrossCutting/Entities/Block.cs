using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Frontend;
using WindowsFormsApplication7.Business;
using SlimDX;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.Business.Geometry;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Block
    {
        public int Id;

        public Vector4[] BlockColors = new Vector4[6]{
            new Vector4(1,1,1,1),
            new Vector4(1,1,1,1),
            new Vector4(1,1,1,1),
            new Vector4(1,1,1,1),
            new Vector4(1,1,1,1),
            new Vector4(0.6f,0.6f,0.6f,1),
        };
        public const int PunchesToBreak = 100;

        public Block(int id)
        {
            this.Id = id;
            BlockRepository.Blocks[Id] = this;
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

        internal Block SetAlpha(float alpha)
        {
            BlockColors[0].W = alpha;
            BlockColors[1].W = alpha;
            BlockColors[2].W = alpha;
            BlockColors[3].W = alpha;
            BlockColors[4].W = alpha;
            BlockColors[5].W = alpha;
            return this;
        }
    }
}
