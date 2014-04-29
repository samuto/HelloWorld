using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.CrossCutting.Entities;

namespace WindowsFormsApplication7.Frontend
{
    class Interpolate
    {
        internal static Vector2 Vector(Vector2 current, Vector2 last, float partialStep)
        {
            float p1 = partialStep;
            float p2 = (1 - partialStep);
            return new Vector2(
                current.X * p1 + last.X * p2,
                current.Y * p1 + last.Y * p2);
        }

        internal static float Scalar(float current, float last, float partialStep)
        {
            float p1 = partialStep;
            float p2 = (1 - partialStep);
            return current * p1 + last * p2;
        }

        internal static Vector3 Vector(Vector3 prev, Vector3 now, float partialStep)
        {
            float p1 = partialStep;
            float p2 = (1 - partialStep);
            return new Vector3(
                now.X * p1 + prev.X * p2,
                now.Y * p1 + prev.Y * p2,
                now.Z * p1 + prev.Z * p2
                );
        }
    }
}
