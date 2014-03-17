using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace WindowsFormsApplication7.Business
{
    /// <summary>
    /// Represents a bounding frustum in three dimensional space.
    /// </summary>
    [Serializable]
    public class BoundingFrustum
    {
        Plane near;
        Plane far;
        Plane top;
        Plane bottom;
        Plane left;
        Plane right;

        Matrix matrix;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlimMath.BoundingFrustum"/> class.
        /// </summary>
        /// <param name="value">The <see cref="SlimMath.Matrix"/> to extract the planes from.</param>
        public BoundingFrustum(Matrix value)
        {
            SetMatrix(ref value);
        }

        /// <summary>
        /// Sets the matrix that represents this instance of <see cref="SlimMath.BoundingFrustum"/>.
        /// </summary>
        /// <param name="value">The <see cref="SlimMath.Matrix"/> to extract the planes from.</param>
        public void SetMatrix(ref Matrix value)
        {
            this.matrix = value;

            //Near
            near.Normal.X = value.M13;
            near.Normal.Y = value.M23;
            near.Normal.Z = value.M33;
            near.D = value.M43;

            //Far
            far.Normal.X = value.M14 - value.M13;
            far.Normal.Y = value.M24 - value.M23;
            far.Normal.Z = value.M34 - value.M33;
            far.D = value.M44 - value.M43;

            //Top
            top.Normal.X = value.M14 - value.M12;
            top.Normal.Y = value.M24 - value.M22;
            top.Normal.Z = value.M34 - value.M32;
            top.D = value.M44 - value.M42;

            //Bottom
            bottom.Normal.X = value.M14 + value.M12;
            bottom.Normal.Y = value.M24 + value.M22;
            bottom.Normal.Z = value.M34 + value.M32;
            bottom.D = value.M44 + value.M42;

            //Left
            left.Normal.X = value.M14 + value.M11;
            left.Normal.Y = value.M24 + value.M21;
            left.Normal.Z = value.M34 + value.M31;
            left.D = value.M44 + value.M41;

            //Right
            right.Normal.X = value.M14 - value.M11;
            right.Normal.Y = value.M24 - value.M21;
            right.Normal.Z = value.M34 - value.M31;
            right.D = value.M44 - value.M41;
        }

        internal bool Contains(BoundingBox boundingBox)
        {
            return BoundingBox.Intersects(boundingBox, near) != PlaneIntersectionType.Back &&
                BoundingBox.Intersects(boundingBox, far) != PlaneIntersectionType.Back &&
                BoundingBox.Intersects(boundingBox, left) != PlaneIntersectionType.Back &&
                BoundingBox.Intersects(boundingBox, right) != PlaneIntersectionType.Back &&
                BoundingBox.Intersects(boundingBox, top) != PlaneIntersectionType.Back &&
                BoundingBox.Intersects(boundingBox, bottom) != PlaneIntersectionType.Back;
        }
    }

}



