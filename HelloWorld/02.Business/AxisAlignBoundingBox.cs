using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace WindowsFormsApplication7.Business
{
    class AxisAlignedBoundingBox
    {
        public Vector3 Min = new Vector3();
        public Vector3 Max = new Vector3();

        public AxisAlignedBoundingBox(Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
        }

        internal Vector3 GetMinFromPosition(Vector3 position)
        {
            return Vector3.Add(Min, position);
        }

        internal Vector3 GetMaxFromPosition(Vector3 position)
        {
            return Vector3.Add(Max, position);
        }

        internal AxisAlignedBoundingBox Clone()
        {
            return new AxisAlignedBoundingBox(new Vector3(Min.X, Min.Y, Min.Z), new Vector3(Max.X, Max.Y, Max.Z));
        }


        internal void AlignToWorldGrid()
        {
            Min.X = MathLibrary.FloorToWorldGrid(Min.X)-1;
            Min.Y = MathLibrary.FloorToWorldGrid(Min.Y)-1;
            Min.Z = MathLibrary.FloorToWorldGrid(Min.Z)-1;
            Max.X = MathLibrary.FloorToWorldGrid(Max.X)+1;
            Max.Y = MathLibrary.FloorToWorldGrid(Max.Y)+1;
            Max.Z = MathLibrary.FloorToWorldGrid(Max.Z)+1;
        }

        internal bool OverLaps(AxisAlignedBoundingBox alienAABB)
        {
            if (Min.X >= alienAABB.Max.X) return false;
            if (Min.Y >= alienAABB.Max.Y) return false;
            if (Min.Z >= alienAABB.Max.Z) return false;
            if (Max.X <= alienAABB.Min.X) return false;
            if (Max.Y <= alienAABB.Min.Y) return false;
            if (Max.Z <= alienAABB.Min.Z) return false;
            return true;
        }

        internal void Translate(Vector3 vector)
        {
            Min = Vector3.Add(Min, vector);
            Max = Vector3.Add(Max, vector);
        }
    }
}
