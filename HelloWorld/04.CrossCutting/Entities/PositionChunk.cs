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
        public readonly int X;
        public readonly int Y;
        public readonly int Z;
        public readonly object Key;

        public PositionChunk(int x, int y, int z)
        {
            if (y < 0) throw new Exception("NO!");
            if (y >= Chunk.MaxSizeY / 16) throw new Exception("NO!!!");

            this.X = x;
            this.Y = y;
            this.Z = z;
            Key = X + "," + Y + "," + Z; ;
        }

        internal static PositionChunk CreateFrom(Vector3 pos)
        {
            PositionChunk newChunk = new PositionChunk(
                MathLibrary.FloorToWorldGrid(pos.X / 16f),
                MathLibrary.FloorToWorldGrid(pos.Y / 16f),
                MathLibrary.FloorToWorldGrid(pos.Z / 16f));

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
