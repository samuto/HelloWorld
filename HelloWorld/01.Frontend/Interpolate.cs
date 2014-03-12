using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.CrossCutting.Entities;

namespace WindowsFormsApplication7._01.Frontend
{
    class Interpolate
    {
        internal static SlimDX.Vector3 Position(Entity entity, float partialTicks)
        {
            float p1 = partialTicks;
            float p2 = (1-partialTicks);
            return new Vector3(
                entity.Position.X * p1 + entity.PrevPosition.X * p2,
                entity.Position.Y * p1 + entity.PrevPosition.Y * p2,
                entity.Position.Z * p1 + entity.PrevPosition.Z * p2
                );
        }



        internal static Vector3 EyePosition(Entity attachedEntity, float partialTicks)
        {
            return Vector3.Add(attachedEntity.EyePosition, Interpolate.Position(attachedEntity, partialTicks));
        }
    }
}
