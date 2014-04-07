using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Frontend;
using WindowsFormsApplication7.Business;
using SlimDX;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.Business.Geometry;

namespace WindowsFormsApplication7.CrossCutting.Entities.Blocks
{
    class Block
    {
        public int Id;
        private bool isOpaque = true;

        public Vector4[] BlockColors = new Vector4[6]{
            new Vector4(1,1,1,1),
            new Vector4(1,1,1,1),
            new Vector4(1,1,1,1),
            new Vector4(1,1,1,1),
            new Vector4(1,1,1,1),
            new Vector4(0.6f,0.6f,0.6f,1),
        };
        private int density = 0;

        public Block(int blockId)
        {
            this.Id = blockId;
            BlockRepository.Blocks[Id] = this;
        }

        internal Block BlockColor(System.Drawing.Color color)
        {
            Vector4 c = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, 1f);
            BlockColors[0] = c;
            BlockColors[1] = c;
            BlockColors[2] = c;
            BlockColors[3] = c;
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

        internal static Block FromId(int blockId)
        {
            return BlockRepository.Blocks[blockId];
        }

        public bool IsOpaque
        {
            get
            {
                return isOpaque;
            }
        }


        internal Block Opaque(bool opaque)
        {
            this.isOpaque = opaque;
            return this;
        }

        internal int Density
        {
            get
            {
                return density;
            }
        }

        internal Block SetDensity(int density)
        {
            this.density = density;
            return this;
        }

        internal virtual int[] DroppedIds()
        {
            return new int[1] { Id };
        }
    }
}
