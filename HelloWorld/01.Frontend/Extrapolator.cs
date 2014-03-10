using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.CrossCutting.Entities;

namespace WindowsFormsApplication7._01.Frontend
{
    class Extrapolator
    {
        internal static SlimDX.Vector3 AdjustByEntity(Vector3 inputVector, Entity entity, float partialTicks)
        {
            Vector3 partialMovement = Vector3.Multiply(entity.Velocity, partialTicks);
            return inputVector = Vector3.Add(inputVector, partialMovement);
        }

        internal static float GetEntityPitch(Entity entity, float partialTicks)
        {
            return entity.Pitch + entity.PitchVelocity * partialTicks;

        }

        internal static float GetEntityYaw(Entity entity, float partialTicks)
        {
            return entity.Yaw + entity.YawVelocity * partialTicks;

        }
    }
}
