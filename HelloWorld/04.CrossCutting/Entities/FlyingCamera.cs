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
            : base(new Vector4(1, 1, 0, 1))
        {
            accGravity = new Vector3(0, 0, 0);
        }

        internal override void Update()
        {
            Velocity = new Vector3();
            Vector3 direction = Vector3.Multiply(GetDirection(), Speed);
            if (moveForward)
                Velocity = direction;
            if(moveBackward)
                Velocity = -direction;
            if (moveLeft)
                Velocity = -new Vector3(-direction.Z, 0, direction.X);
            if (moveRight)
                Velocity = new Vector3(-direction.Z, 0, direction.X);
            Velocity.Normalize();
            Position.X += Velocity.X;
            Position.Y += Velocity.Y;
            Position.Z += Velocity.Z;
            
            // clear input
            moveRight = moveLeft = moveForward = moveBackward = false;
        }


    }
}
