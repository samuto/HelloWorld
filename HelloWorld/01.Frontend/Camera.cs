using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7._01.Frontend;

namespace WindowsFormsApplication7.Frontend
{
    class Camera
    {
        public static Camera Instance = new Camera();
        public Matrix World;
        public Matrix View;
        public Matrix Projection;
        public Vector3 EyePosition;
        public bool Enable3d;
        private Entity attachedEntity;


        private Camera()
        {
            World = Matrix.Identity;
        }

        public void AttachTo(Entity entity)
        {
            attachedEntity = entity;
        }

        internal void Update(float partialTicks)
        {
            
            if (Enable3d)
            {
                Vector3 direction = attachedEntity.GetDirection();
                Vector3 eye = Interpolate.EyePosition(attachedEntity, partialTicks);
                Vector3 target = Vector3.Add(eye, direction);

                Camera.Instance.View = Matrix.LookAtRH(eye, target, new Vector3(0, 1, 0));
                Camera.Instance.Projection = Matrix.PerspectiveFovRH(45.0f, (float)TheGame.Instance.Width / (float)TheGame.Instance.Height, 0.05f, 100f);
            }
            else
            {
                Vector3 eye = new Vector3(TheGame.Instance.Width / 2, TheGame.Instance.Height / 2, 0);
                Vector3 target = new Vector3(TheGame.Instance.Width / 2, TheGame.Instance.Height / 2, -5);
                Camera.Instance.View = Matrix.LookAtRH(eye, target, new Vector3(0, 1, 0));
                Camera.Instance.Projection = Matrix.OrthoRH((float)TheGame.Instance.Width, (float)TheGame.Instance.Height, 0, 10);
            }
        }

       
    }
}
