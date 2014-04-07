using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using SlimDX;
using WindowsFormsApplication7.Business.Geometry;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    struct PositionChunk
    {
        public int X;
        public int Y;
        public int Z;

        public PositionChunk(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        internal static PositionChunk CreateFrom(Vector3 pos)
        {
            PositionChunk newChunk = new PositionChunk();

            newChunk.X = MathLibrary.FloorToWorldGrid(pos.X / 16f);
            newChunk.Y = MathLibrary.FloorToWorldGrid(pos.Y / 16f);
            newChunk.Z = MathLibrary.FloorToWorldGrid(pos.Z / 16f);

            return newChunk;
        }


        internal static PositionChunk CreateFrom(PositionBlock positionBlock)
        {
            return CreateFrom(new Vector3(positionBlock.X, positionBlock.Y, positionBlock.Z));
        }

        internal void ConvertToLocalPosition(ref PositionBlock positionBlock)
        {
            positionBlock.X = positionBlock.X - X * 16;
            positionBlock.Y = positionBlock.Y - Y * 16;
            positionBlock.Z = positionBlock.Z - Z * 16;
        }

        public object Key
        {
            get
            {
                return X + "," + Y + "," + Z;
            }
        }

        internal void GetMinCornerBlock(out PositionBlock positionBlock)
        {
            positionBlock.X = X * 16;
            positionBlock.Y = Y * 16;
            positionBlock.Z = Z * 16;
        }

        internal void GetGlobalPositionBlock(out PositionBlock positionBlock, int x, int y, int z)
        {
            positionBlock.X = X * 16 + x;
            positionBlock.Y = Y * 16 + y;
            positionBlock.Z = Z * 16 + z;
        }

        internal bool SameAs(PositionChunk posChunk)
        {
            return posChunk.X == X && posChunk.Y == Y && posChunk.Z == Z;
        }
    }
}
