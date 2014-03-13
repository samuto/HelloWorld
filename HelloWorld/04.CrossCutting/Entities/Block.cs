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
            BlockColorMap.BlockColors[Id] = new SlimDX.Vector4[6]{
                c * 0.8f,
                c * 0.7f,
                c * 0.6f,
                c * 0.5f,
                c * 1f,
                c * 0.4f};
            return this;
        }

        internal Block TopColor(System.Drawing.Color color)
        {
            Vector4 c = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, 1f);
            BlockColorMap.BlockColors[Id][4] = c;
            return this;
        }

        internal Block TopBottomColor(System.Drawing.Color color)
        {
            Vector4 c = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, 1f);
            BlockColorMap.BlockColors[Id][4] = c;
            BlockColorMap.BlockColors[Id][5] = c;
            return this;
        }

        internal AxisAlignedBoundingBox GetBoundingBox()
        {
            return new AxisAlignedBoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        }
    }
}
