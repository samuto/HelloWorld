using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7._01.Frontend;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.Business.Geometry;

namespace WindowsFormsApplication7.Frontend
{
    class Camera
    {
        public static Camera Instance = new Camera();
        public Matrix World;
        public Matrix View;
        public Matrix Projection;
        public Vector3 EyePosition;
        public Vector3 Direction;
        public bool Enable3d;
        private Entity attachedEntity;
        private BoundingFrustum frustum;

        private Camera()
        {
            World = Matrix.Identity;
        }

        public void AttachTo(Entity entity)
        {
            attachedEntity = entity;
        }

        internal void Update(float partialStep)
        {
            
            if (Enable3d)
            {
                Direction = Interpolate.Vector(attachedEntity.PrevDirection, attachedEntity.Direction, partialStep);
                EyePosition = Interpolate.EyePosition(attachedEntity, partialStep);
                Vector3 target = Vector3.Add(EyePosition, Direction * 5f);
                View = Matrix.LookAtRH(EyePosition, target, new Vector3(0, 1, 0));
                Projection = Matrix.PerspectiveFovRH(45.0f, (float)TheGame.Instance.Width / (float)TheGame.Instance.Height, 0.05f, 200f);
                frustum = new BoundingFrustum(Matrix.Multiply(View, Projection));
            }
            else
            {
                Vector3 eye = new Vector3(TheGame.Instance.Width / 2, TheGame.Instance.Height / 2, 0);
                Vector3 target = new Vector3(TheGame.Instance.Width / 2, TheGame.Instance.Height / 2, -5);
                Camera.Instance.View = Matrix.LookAtRH(eye, target, new Vector3(0, 1, 0));
                Camera.Instance.Projection = Matrix.OrthoRH((float)TheGame.Instance.Width, (float)TheGame.Instance.Height, 0, 100);
            }
        }

        internal bool InsideViewFrustum(BoundingBox boundingBox)
        {
            return frustum.Contains(boundingBox);
        }
    }
}
