using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.Business.Geometry;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.CrossCutting;
using WindowsFormsApplication7.CrossCutting.Entities.Blocks;

namespace WindowsFormsApplication7.Business
{
    class CollisionComplex : ICollisionSystem
    {
        private Entity entity;
        private Log logger = Log.Instance;
        
        public CollisionComplex(Entity entity)
        {
            this.entity = entity;
        }

        public Vector3 Resolve(CrossCutting.Entities.Entity entity)
        {
            List<AxisAlignedBoundingBox> collidingObjects;
            Vector3 response = entity.Position;
            Vector3 v = entity.Velocity;
            AxisAlignedBoundingBox AABB = entity.AABB;

            
            Vector3 testZ = entity.Position;
            Vector3 testX = entity.Position;

            HandleZ(ref testZ, ref v);
            HandleX(ref testX, ref v);

            bool zFirst = Math.Abs(response.Z - testZ.Z) > Math.Abs(response.X - testX.X);

            if (zFirst)
            {
                response = testZ;
                HandleX(ref response, ref v);
            }
            else
            {
                response = testX;
                HandleZ(ref response, ref v);
            }

            //HandleY
            entity.onGround = false;
            collidingObjects = GetCollidingObjects(new Vector3(response.X, response.Y + v.Y, response.Z));
            if (collidingObjects.Count > 0)
            {
                if (v.Y < 0)
                {
                    response.Y = collidingObjects.Max(o => o.Max.Y) - AABB.Min.Y;
                    entity.onGround = true;
                }
                v.Y = 0;
            }
            else
                response.Y += v.Y;

            entity.Velocity = v;
            return response;

        }

        private void HandleX(ref Vector3 response, ref Vector3 v)
        {
            List<AxisAlignedBoundingBox> collidingObjects;
            
            // HandleX
            collidingObjects = GetCollidingObjects(new Vector3(response.X + v.X, response.Y, response.Z));
            if (collidingObjects.Count > 0)
                v.X = 0;
            else
                response.X += v.X;
        }

        private void HandleZ(ref Vector3 response, ref Vector3 v)
        {
            List<AxisAlignedBoundingBox> collidingObjects;
            
            // HandleZ
            collidingObjects = GetCollidingObjects(new Vector3(response.X, response.Y, response.Z + v.Z));
            if (collidingObjects.Count > 0)
                v.Z = 0;
            else
                response.Z += v.Z;
        }

        private List<AxisAlignedBoundingBox> GetCollidingObjects(Vector3 newPosition)
        {
            List<AxisAlignedBoundingBox> collidingObjects = new List<AxisAlignedBoundingBox>();

            World theWorld = World.Instance;
            AxisAlignedBoundingBox thisAABB = entity.AABB;
            thisAABB.Translate(newPosition);

            AxisAlignedBoundingBox sweepAABB = thisAABB;
            //sweepAABB.Min.Y--;
            sweepAABB.AlignToWorldGrid();
            for (int x = (int)sweepAABB.Min.X; x <= (int)sweepAABB.Max.X; x++)
            {
                for (int y = (int)sweepAABB.Min.Y; y <= (int)sweepAABB.Max.Y; y++)
                {
                    for (int z = (int)sweepAABB.Min.Z; z <= (int)sweepAABB.Max.Z; z++)
                    {
                        int blockId = theWorld.GetBlock(x, y, z);
                        Block alien = BlockRepository.Blocks[blockId];
                        if (!alien.IsOpaque)
                            continue;
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
    }
}
