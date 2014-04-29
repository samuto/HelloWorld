using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.Business.Geometry;
using SlimDX;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities.Items;
using WindowsFormsApplication7.CrossCutting.Entities.Blocks;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class EntityStack : EntityPlayer
    {
        private int count;
        public new int Id;
        private float dYaw = (float)(MathLibrary.GlobalRandom.NextDouble() - 0.5);
        private float dPitch = (float)(MathLibrary.GlobalRandom.NextDouble() - 0.5);

        public int Count
        {
            get
            {
                return count;
            }
        }

        public EntityStack(int id, int count)
        {
            this.Id = id;
            this.count = count;
            AABB = new AxisAlignedBoundingBox(new Vector3(-0.25f, -0.25f, -0.25f), new Vector3(0.25f, 0.25f, 0.25f));
            collisionSystem = new CollisionSimple(this);
            EntityType = EntityTypeEnum.EntityStackFullUpdate;
        }

        internal bool Compatible(EntityStack otherStack)
        {
            return otherStack.Id == Id || this.Id == 0 || otherStack.Id == 0;
        }

       
        internal Block AsBlock
        {
            get
            {
                if (!Entity.IsBlock(Id)) 
                    return null;
                return BlockRepository.Blocks[Id];
            }
        }

        internal Entity AsEntity
        {
            get
            {
                return Entity.FromId(Id);
            }
        }

        internal Item AsItem
        {
            get
            {
                if (!Entity.IsItem(Id))
                    return null;
                return ItemRepository.Items[Id];
            }
        }

        internal bool IsEmpty
        {
            get
            {
                return Count == 0;
            }
        }

        internal void TransferEntities(EntityStack destinationStack, int transferCount)
        {
            if (!Compatible(destinationStack))
                return;
            destinationStack.Id = this.Id;
            if (destinationStack.Count + transferCount > 64)
            {
                transferCount = 64 - destinationStack.Count;
            }
            destinationStack.Add(transferCount);
            Remove(transferCount);
        }

        internal void Add(int addCount)
        {
            count += addCount;
        }

        internal void Remove(int consumeCount)
        {
            count -= consumeCount;
            if (Count <= 0)
            {
                count = 0;
                Id = 0;
            }
        }

        internal bool TransferAll(EntityStack destinationStack)
        {
            
            if (destinationStack.Count + Count > 64)
                return false;
            TransferEntities(destinationStack, Count);
            return true;
        }

        internal void ReplaceWith(int newId, int newCount)
        {
            Id = newId;
            count = newCount;
        }

        internal void Clear()
        {
            ReplaceWith(0, 0);
        }

        internal void ReplaceWith(EntityStack newStack)
        {
            Id = newStack.Id;
            count = newStack.Count;
        }

        internal void ReplaceWithEmptyCompatibleStack(EntityStack stackInHand)
        {
            ReplaceWith(stackInHand.Id, 0);
        }

        internal static EntityStack CreateEmptyStack()
        {
            return new EntityStack(0, 0);
        }

        internal void Swap(EntityStack swapStack)
        {
            int tempCount = this.count;
            int tempId = this.Id;
            this.count = swapStack.count;
            this.Id = swapStack.Id;
            swapStack.count = tempCount;
            swapStack.Id = tempId;
        }

       internal override void OnUpdate()
        {
            PrevYaw = Yaw;
            Yaw += dYaw*0.02f;
            PrevPitch = Pitch;
            Pitch += dPitch * 0.02f;
            if (Yaw > Math.PI * 2.0)
                Yaw -= (float)Math.PI * 2f;
            if (Pitch > Math.PI * 2.0)
                Pitch -= (float)Math.PI * 2f;
            base.OnUpdate();
        }
    }
}
