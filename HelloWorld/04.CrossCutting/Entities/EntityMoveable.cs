using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using SlimDX;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.Business.Geometry;
using SlimDX.DirectInput;
using WindowsFormsApplication7.Frontend.Gui;
using WindowsFormsApplication7.Frontend;
using WindowsFormsApplication7.Business.Profiling;
using WindowsFormsApplication7.CrossCutting.Entities.Items;
using WindowsFormsApplication7.CrossCutting.Entities.Blocks;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class EntityMoveable : Entity
    {
        public static Vector3 defaultGravity = new Vector3(0, -0.02f, 0);
        public Vector3 accGravity = defaultGravity;
        public Vector3 Velocity = new Vector3(0, 0, 0);
        public Vector3 Direction = new Vector3();
        public Vector3 PrevDirection = new Vector3();
        public bool onGround = false;
        protected bool moveLeft = false;
        protected bool moveRight = false;
        protected bool moveForward = false;
        protected bool moveBackward = false;
        protected bool moveJump = false;
        protected bool moveUp = false;
        protected bool moveDown = false;
        protected ICollisionSystem collisionSystem = null;
        public bool CollisionEnabled = true;

        internal void UpdateDirection()
        {
            Matrix m = Matrix.RotationYawPitchRoll(Yaw, Pitch, 0);
            Vector4 v = new Vector4(0, 0, 1, 1);
            v = Vector4.Transform(v, m);
            Vector3 v3 = new Vector3(v.X, v.Y, v.Z);
            PrevDirection = Direction;
            Direction = v3;
        }

        internal void UpdateViewAngles()
        {
            float deltaYaw = -Input.Instance.CurrentInput.MouseState.X * GameSettings.MouseSensitivity * 0.002f;
            float deltaPitch = Input.Instance.CurrentInput.MouseState.Y * GameSettings.MouseSensitivity * 0.002f;
            PrevYaw = Yaw;
            PrevPitch = Pitch;
            Yaw += deltaYaw;
            if (Yaw > MathLibrary.Math2Pi)
                Yaw -= MathLibrary.Math2Pi;
            if (Yaw < 0f)
                Yaw += MathLibrary.Math2Pi;
            Pitch += deltaPitch;
            float limit = MathLibrary.Math2Pi / 4f - 0.01f;
            if (Pitch > limit)
                Pitch = limit;
            if (Pitch < -limit)
                Pitch = -limit;
        }

        internal void MoveDown()
        {
            moveDown = true;
        }

        internal void MoveUp()
        {
            moveUp = true;
        }

        internal void Jump()
        {
            moveJump = true;
        }

        internal void MoveForward()
        {
            moveForward = true;
        }

        internal void MoveBackward()
        {
            moveBackward = true;
        }

        internal void MoveLeft()
        {
            moveLeft = true;
        }

        internal void MoveRight()
        {
            moveRight = true;
        }

        public bool IsMoving
        {
            get
            {
                return onGround && (PrevPosition - Position).LengthSquared() > 0;
            }
        }
    }
}