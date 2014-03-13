using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using SlimDX;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Entity
    {
        public float Speed = 1f;
        public Vector4 Color = new Vector4(1, 1, 1, 1);
        public Vector3 PrevPosition = new Vector3(0, 0, 0);
        public Vector3 Position = new Vector3(0, 0, 0);
        public AxisAlignedBoundingBox AABB = new AxisAlignedBoundingBox(new Vector3(-0.5f, 0f, -0.5f), new Vector3(0.5f, 1f, 0.5f));
        public Vector3 EyePosition = new Vector3(0, 0, 0);
        public float Yaw = 0;
        public float Pitch = 0;
        public Vector3 accGravity = new Vector3(0, -0.1f, 0);
        public Vector3 Velocity = new Vector3(0, 0, 0);

        protected bool moveLeft = false;
        protected bool moveRight = false;
        protected bool moveForward = false;
        protected bool moveBackward = false;
        protected bool moveJump = false;
        protected bool moveUp = false;
        protected bool moveDown = false;
        protected bool onGround = false;
        protected bool changeYaw = false;
        protected bool changePitch = false;

        private const float Math2Pi = (float)Math.PI * 2f;


        public Entity(Vector4 color)
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

      
        internal virtual void Update()
        {
            CalculateVelocity();
            Vector3.Add(ref Velocity, ref accGravity, out Velocity);

            List<AxisAlignedBoundingBox> collidingObjects;
            Vector3 response = Position;
            float halfWidth = GetWidth() / 2f;

            collidingObjects = GetCollidingObjects(new Vector3(response.X, response.Y, response.Z + Velocity.Z));
            if (collidingObjects.Count > 0)
            {
                if (Velocity.Z < 0)
                    response.Z = collidingObjects.Max(o => o.Max.Z) + halfWidth;
                else if (Velocity.Z > 0)
                    response.Z = collidingObjects.Min(o => o.Min.Z) - halfWidth;
            }
            else
                response.Z += Velocity.Z;

            collidingObjects = GetCollidingObjects(new Vector3(response.X + Velocity.X, response.Y, response.Z));
            if (collidingObjects.Count > 0)
            {
                if (Velocity.X < 0)
                    response.X = collidingObjects.Max(o => o.Max.X) + halfWidth;
                else if (Velocity.X > 0)
                    response.X = collidingObjects.Min(o => o.Min.X) - halfWidth;
            }
            else
                response.X += Velocity.X;

            onGround = false;
            collidingObjects = GetCollidingObjects(new Vector3(response.X, response.Y + Velocity.Y, response.Z));
            if (collidingObjects.Count > 0)
            {
                if (Velocity.Y < 0)
                {
                    response.Y = collidingObjects.Max(o => o.Max.Y);
                    onGround = true;
                }
                else if (Velocity.Y > 0)
                {
                    response.Y = collidingObjects.Min(o => o.Min.Y) - GetHeight();
                    Velocity.Y = 0;
                }
            }
            else
                response.Y += Velocity.Y;

            PrevPosition = Position;
            Position.X = response.X;
            Position.Y = response.Y;
            Position.Z = response.Z;

            if (onGround)
            {
                moveLeft = false;
                moveRight = false;
                moveForward = false;
                moveBackward = false;
            }
            moveJump = false;
            moveUp = false;
            moveDown = false;
        }

        private List<AxisAlignedBoundingBox> GetCollidingObjects(Vector3 newPosition)
        {
            List<AxisAlignedBoundingBox> collidingObjects = new List<AxisAlignedBoundingBox>();

            World theWorld = World.Instance;
            AxisAlignedBoundingBox thisAABB = AABB.Clone();
            thisAABB.Translate(newPosition);

            AxisAlignedBoundingBox sweepAABB = thisAABB.Clone();
            sweepAABB.Min.Y--;
            sweepAABB.AlignToWorldGrid();
            for (int x = (int)sweepAABB.Min.X; x <= (int)sweepAABB.Max.X; x++)
            {
                for (int y = (int)sweepAABB.Min.Y; y <= (int)sweepAABB.Max.Y; y++)
                {
                    for (int z = (int)sweepAABB.Min.Z; z <= (int)sweepAABB.Max.Z; z++)
                    {
                        int blockId = theWorld.GetBlock(x, y, z);
                        if (blockId == 0)
                            continue;
                        Block alien = BlockRepository.Blocks[blockId];
                        AxisAlignedBoundingBox collidingObject = alien.GetBoundingBox();
                        collidingObject.Translate(new Vector3(x, y, z));
                        if (thisAABB.OverLaps(collidingObject))
                        {
                            collidingObjects.Add(collidingObject);
                        }
                    }
                }
            }
            return collidingObjects;
        }

        protected void CalculateVelocity()
        {
            if (onGround)
            {
                Vector3 forward = GetDirection();
                forward.Y = 0;
                Vector3 left = new Vector3(forward.Z, 0, -forward.X);
                Vector3 direction = new Vector3();
                if (moveLeft)
                    Vector3.Add(ref direction, ref left, out direction);
                if (moveRight)
                    Vector3.Subtract(ref direction, ref left, out direction);
                if (moveForward)
                    Vector3.Add(ref direction, ref forward, out direction);
                if (moveBackward)
                    Vector3.Subtract(ref direction, ref forward, out direction);
                direction.Normalize();
                Vector3.Multiply(ref direction, Speed, out direction);
                Velocity.X = direction.X;
                Velocity.Z = direction.Z;
                Velocity.Y = 0;
            }
           


            if (moveUp)
                Velocity.Y += Speed;
            if (moveDown)
                Velocity.Y -= Speed;

            if (moveJump && onGround)
            {
                Velocity.Y = 0.6f;
                Velocity.X *= 0.7f;
                Velocity.Z *= 0.7f;
            }
        }


        internal Vector3 GetDirection()
        {
            Matrix m = Matrix.RotationYawPitchRoll(Yaw, Pitch, 0);
            Vector4 v = new Vector4(0, 0, 1, 1);
            v = Vector4.Transform(v, m);
            Vector3 v3 = new Vector3(v.X, v.Y, v.Z);
            return v3;
        }

        internal void SetViewAngles(float deltaYaw, float deltaPitch)
        {
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
    }
}

