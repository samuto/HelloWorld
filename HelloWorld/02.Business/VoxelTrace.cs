using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.CrossCutting.Entities.Blocks;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.Business
{
    class VoxelTrace
    {
        public bool Hit = false;
        public Vector4 BuildPosition = new Vector4();
        public Vector4 ImpactPosition = new Vector4();
        private float raylengthSquared = 5 * 5;
        public Block ImpactBlock;    

        public void Update(Vector3 point, Vector3 direction)
        {
            var voxels = TraceLine(point, direction);
            Hit = false;
            for (int i = 1; i < voxels.Count; i++)
            {
                var v = voxels[i];
                int impactBlockId = World.Instance.GetBlock(PositionBlock.FromVector(v));
                ImpactBlock = Block.FromId(impactBlockId);
                if (impactBlockId != 0 && impactBlockId != BlockRepository.Water.Id)
                {
                    ImpactPosition = voxels[i];
                    BuildPosition = voxels[i-1];
                    Hit = true;
                    return;
                }
            }
        }

        private List<Vector4> TraceLine(Vector3 point, Vector3 direction)
        {
            List<Vector4> points = new List<Vector4>();

            // init
            int stepX = (int)Math.Sign(direction.X);
            int stepY = (int)Math.Sign(direction.Y);
            int stepZ = (int)Math.Sign(direction.Z);

            Vector3 tracePos = new Vector3(point.X, point.Y, point.Z);
            Vector3 currentBlock = new Vector3(AlignToGrid(point.X), AlignToGrid(point.Y), AlignToGrid(point.Z));


            while (true)
            {
                // do we need to stop?
                float dx = tracePos.X - point.X;
                float dy = tracePos.Y - point.Y;
                float dz = tracePos.Z - point.Z;
                if (dx * dx + dy * dy + dz * dz > raylengthSquared)
                {
                    break;
                }
                points.Add(new Vector4(currentBlock.X, currentBlock.Y, currentBlock.Z, 0));

                // find next block
                float nextEdgeX = NextEdge(tracePos.X, stepX);
                float nextEdgeY = NextEdge(tracePos.Y, stepY);
                float nextEdgeZ = NextEdge(tracePos.Z, stepZ);
                float tx = (nextEdgeX - tracePos.X) / (direction.X);
                float ty = (nextEdgeY - tracePos.Y) / (direction.Y);
                float tz = (nextEdgeZ - tracePos.Z) / (direction.Z);
                float t = Math.Min(tx, Math.Min(ty, tz));
                tracePos.X = (tracePos.X + t * direction.X);
                tracePos.Y = (tracePos.Y + t * direction.Y);
                tracePos.Z = (tracePos.Z + t * direction.Z);

                // trace block
                bool crossYZ = IsCloseToGrid(tracePos.X);
                bool crossXZ = IsCloseToGrid(tracePos.Y);
                bool crossXY = IsCloseToGrid(tracePos.Z);

                /*
                if (crossX && crossY)
                {
                    points.Add(new Vector3(currentBlock.X + stepX, currentBlock.Y, currentBlock.Z));
                    points.Add(new Vector3(currentBlock.X, currentBlock.Y + stepY, currentBlock.Z));
                }
                */
                if (crossYZ)
                {
                    tracePos.X = (float)Math.Round(tracePos.X);
                    currentBlock.X += stepX;
                }
                if (crossXZ)
                {
                    tracePos.Y = (float)Math.Round(tracePos.Y);
                    currentBlock.Y += stepY;
                }
                if (crossXY)
                {
                    tracePos.Z = (float)Math.Round(tracePos.Z);
                    currentBlock.Z += stepZ;
                }

            }
            return points;
        }


        private static bool IsCloseToGrid(float v)
        {
            double absV = Math.Abs(v);
            float howclose = (float)Math.Abs(Math.Round(absV) - absV);
            return howclose < 0.0001f;
        }

        private static float NextEdge(float v, int direction)
        {
            float left = AlignToGrid(v);
            float right = AlignToGrid(v + 1);
            if (direction < 0)
                return (left == v) ? left - 1 : left;
            else
                return right;

        }

        private static int AlignToGrid(float v)
        {
            int i = (int)v;
            return i > v ? i - 1 : i;
        }

    }
}
