using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Frontend;
using WindowsFormsApplication7.Business;
using SlimDX;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.Business.Geometry;

namespace WindowsFormsApplication7.CrossCutting.Entities.Blocks
{
    class Block : Entity
    {
        public Vector4[] BlockColors = new Vector4[6]{
            new Vector4(1,1,1,1),
            new Vector4(1,1,1,1),
            new Vector4(1,1,1,1),
            new Vector4(1,1,1,1),
            new Vector4(1,1,1,1),
            new Vector4(0.6f,0.6f,0.6f,1),
        };

        public bool HasStages;
        public int DropId;
        public float DropProbability = 1f;
        public bool IsTransparent = false;
        public int MaxStage = 0;

        public Block()
        {
            this.Id = BlockRepository.NextId();
            this.DropId = Id;
            BlockRepository.Blocks[Id] = this;
        }

        // 
        // .. methods
        //

        internal new static Block FromId(int blockId)
        {
            return BlockRepository.Blocks[blockId];
        }

        protected void DropStack(EntityStack stack, Entities.PositionBlock pos)
        {
            if (stack.IsEmpty)
                return;
            stack.Position = new Vector3(pos.X + 0.5f, pos.Y + 0.5f, pos.Z + 0.5f);
            stack.Yaw = (float)(MathLibrary.GlobalRandom.NextDouble() * Math.PI);
            stack.Pitch = (float)(MathLibrary.GlobalRandom.NextDouble() * Math.PI);
            stack.Velocity = new Vector3(
                (float)0.05f * (float)(MathLibrary.GlobalRandom.NextDouble() * 2.0 - 1.0),
                (float)0.25f,
                (float)0.05f * (float)(MathLibrary.GlobalRandom.NextDouble() * 2.0 - 1.0));
            World.Instance.SpawnStack(stack);
        }

        internal virtual Entity CreateEntity()
        {
            return null;
        }

        // Setters/getters
        internal Block SetFullUpdate()
        {
            EntityType = EntityTypeEnum.BlockFullUpdate;
            return this;
        }

        internal Block SetRandomUpdates()
        {
            EntityType = EntityTypeEnum.BlockRandomUpdate;
            return this;
        }


        internal Block SetMaxStage(int maxStage)
        {
            HasStages = true;
            MaxStage = maxStage;
            return this;
        }


        internal Block SetEnergyToTransform(int energy)
        {
            EnergyToTransform = energy;
            return this;
        }

        internal Block SetBlockColor(System.Drawing.Color color)
        {
            Vector4 c = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, 1f);
            BlockColors[0] = c;
            BlockColors[1] = c;
            BlockColors[2] = c;
            BlockColors[3] = c;
            BlockColors[4] = c;
            BlockColors[5] = c;
            return this;
        }

        internal Block SetTopColor(System.Drawing.Color color)
        {
            Vector4 c = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, 1f);
            BlockColors[4] = c;
            return this;
        }

        internal Block SetTopBottomColor(System.Drawing.Color color)
        {
            Vector4 c = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, 1f);
            BlockColors[4] = c;
            BlockColors[5] = c;
            return this;
        }

        internal AxisAlignedBoundingBox BoundingBox
        {
            get
            {
                return new AxisAlignedBoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            }
        }

        internal Block ResetOpaque()
        {
            this.IsOpaque = false;
            return this;
        }

        internal Block SetDensity(int density)
        {
            this.Density = density;
            return this;
        }

        internal Block SetDropId(int id, float chance = 1f)
        {
            DropProbability = chance;
            DropId = id;
            return this;
        }


        internal Block SetDropProbability(float chance)
        {
            DropProbability = chance;
            return this;
        }


        //
        // Events / default behaviour
        //
        internal virtual int[] OnDroppedIds()
        {
            if (DropProbability == 1f)
                return new int[1] { DropId };
            else if (MathLibrary.GlobalRandom.NextDouble() < DropProbability)
                return new int[1] { DropId };
            return new int[0];
        }

        internal virtual bool OnActivate(PositionBlock position)
        {
            return false;
        }

        internal virtual void OnDestroy(PositionBlock pos)
        {
            int[] droppedIds = OnDroppedIds();
            if (droppedIds.Length > 0)
            {
                foreach (int id in droppedIds)
                {
                    EntityStack stack = new EntityStack(id, 1);
                    DropStack(stack, pos);
                }
            }
        }

        internal Block SetTransparent()
        {
            IsTransparent = true;
            return this;
        }

        internal virtual bool FaceVisibleByNeighbor(int neighborBlockId)
        {
            return Block.FromId(neighborBlockId).IsTransparent; 
        }

        internal void UpdateBlock(Chunk chunk, PositionBlock pos)
        {
            Entity entity = this.CreateEntity();
            if (entity != null)
            {
                entity.Parent = chunk;
                entity.BlockPosition = pos;
                entity.OnUpdate();
            }

        }

       
    }
}
