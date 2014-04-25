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
    class EntityPlayer : EntityMoveable
    {
        public float Speed = 1f;
        public Vector3 EyePosition = new Vector3(0, 0, 0);

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
            if (TheGame.Instance.Mode == TheGame.GameMode.InGame)
            {
                UpdateViewAngles();
            }
            Vector3.Add(ref Velocity, ref accGravity, out Velocity);

            Vector3 response = Position;
            if (collisionSystem != null)
                response = collisionSystem.Resolve(this);
            else
            {
                response = Position + Velocity;
                Velocity *= 0.5f;
            }
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

        
    }
}

