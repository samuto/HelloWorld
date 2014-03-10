using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using SlimDX;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    struct PositionChunk
    {
        public int X;
        public int Z;

        public PositionChunk(int x, int z)
        {
            this.X = x;
            this.Z = z;
        }

        internal static PositionChunk CreateFrom(Vector3 pos)
        {
            PositionChunk newChunk = new PositionChunk();

            newChunk.X = MathLibrary.FloorToWorldGrid(pos.X / 16f);
            newChunk.Z = MathLibrary.FloorToWorldGrid(pos.Z / 16f);

            return newChunk;
        }

        internal PositionChunk AddScalar(int scalar)
        {
            X += scalar;
            Z += scalar;
            return this;
        }

        

        internal static PositionChunk CreateFrom(PositionBlock positionBlock)
        {
            PositionChunk newChunk = new PositionChunk();

            newChunk.X = MathLibrary.FloorToWorldGrid(positionBlock.X / 16f);
            newChunk.Z = MathLibrary.FloorToWorldGrid(positionBlock.Z / 16f);

            return newChunk;
        }

        internal PositionBlock GetLocalPositionBlock(PositionBlock positionBlock)
        {
            return new PositionBlock(
                positionBlock.X - X * 16,
                positionBlock.Y,
                positionBlock.Z - Z * 16
            );
        }

        public object Key
        {
            get
            {
                return X+","+Z;
            }
        }

        internal PositionBlock GetMinCornerBlock()
        {
            return new PositionBlock(X * 16, 0, Z * 16);
        }

        internal PositionBlock GetGlobalPositionBlock(int x, int y, int z)
        {
            return new PositionBlock(X * 16 + x, y, Z * 16 + z);
        }
    }
}
