using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class FlyingCamera : Entity
    {
        public FlyingCamera()
            : base(new Vector4(0.5f,0.5f,0.5f,1f))
        {
        }

        internal override void Update()
        {
            UpdateDirection();
            UpdateViewAngles();
            Velocity = new Vector3();
            Vector3 direction = Vector3.Multiply(Direction, Speed);
            if (moveForward)
                Velocity = direction;
            if(moveBackward)
                Velocity = -direction;
            if (moveLeft)
                Velocity = -new Vector3(-direction.Z, 0, direction.X);
            if (moveRight)
                Velocity = new Vector3(-direction.Z, 0, direction.X);
            Velocity.Normalize();
            PrevPosition = Position;
            Position.X += Velocity.X;
            Position.Y += Velocity.Y;
            Position.Z += Velocity.Z;
            
            // clear input
            moveRight = moveLeft = moveForward = moveBackward = false;
        }


    }
}
