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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="partialStep">1.0 if this is previous tick position, 0.0 if this is current tick position</param>
        /// <returns></returns>
        internal static SlimDX.Vector3 Position(Entity entity, float partialStep)
        {
            float p1 = partialStep;
            float p2 = (1-partialStep);
            return new Vector3(
                entity.Position.X * p1 + entity.PrevPosition.X * p2,
                entity.Position.Y * p1 + entity.PrevPosition.Y * p2,
                entity.Position.Z * p1 + entity.PrevPosition.Z * p2
                );
        }

        internal static Vector3 EyePosition(Entity attachedEntity, float partialStep)
        {
            return Vector3.Add(attachedEntity.EyePosition, Interpolate.Position(attachedEntity, partialStep));
        }

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
