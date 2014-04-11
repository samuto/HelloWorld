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

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class EntityPlayer : Entity
    {
        public float Speed = 1f;
        public Vector4 Color = new Vector4(1, 1, 1, 1);
        public Vector3 PrevPosition = new Vector3(0, 0, 0);
        public Vector3 Position = new Vector3(0, 0, 0);
        public AxisAlignedBoundingBox AABB = new AxisAlignedBoundingBox(new Vector3(-0.5f, 0f, -0.5f), new Vector3(0.5f, 1f, 0.5f));
        public Vector3 EyePosition = new Vector3(0, 0, 0);
        public float Yaw = 0;
        public float Pitch = 0;
        public float PrevYaw = 0;
        public float PrevPitch = 0;
        public Vector3 accGravity = new Vector3(0, -0.02f, 0);
        public Vector3 Velocity = new Vector3(0, 0, 0);
        public Vector3 Direction = new Vector3();
        public Vector3 PrevDirection = new Vector3();
        protected bool moveLeft = false;
        protected bool moveRight = false;
        protected bool moveForward = false;
        protected bool moveBackward = false;
        protected bool moveJump = false;
        protected bool moveUp = false;
        protected bool moveDown = false;
        public bool onGround = false;
        protected bool changeYaw = false;
        protected bool changePitch = false;
        protected ICollisionSystem collisionSystem = null;

        private const float Math2Pi = (float)Math.PI * 2f;


        public EntityPlayer()
        {
        }

        public EntityPlayer(Vector4 color)
        {
            this.Color = color;
        }

        public float GetWidth()
        {
            return AABB.Max.X - AABB.Min.X;
        }

        public float GetHeight()
        {
            return AABB.Max.Y - AABB.Min.Y;
        }


        internal override void OnUpdate()
        {
            UpdateDirection();
            if (TheGame.Instance.IsEntityControlled(this) && TheGame.Instance.Mode == TheGame.GameMode.InGame)
            {
                UpdateViewAngles();
            }
            Vector3.Add(ref Velocity, ref accGravity, out Velocity);

            Vector3 response = Position;
            if (collisionSystem != null)
                response = collisionSystem.Resolve(this);
            
            PrevPosition = Position;
            Position.X = response.X;
            Position.Y = response.Y;
            Position.Z = response.Z;


            moveLeft = false;
            moveRight = false;
            moveForward = false;
            moveBackward = false;

            moveJump = false;
            moveUp = false;
            moveDown = false;
            
        }



        

       


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
            if (Yaw > Math2Pi)
                Yaw -= Math2Pi;
            if (Yaw < 0f)
                Yaw += Math2Pi;
            Pitch += deltaPitch;
            float limit = Math2Pi / 4f - 0.1f;
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

