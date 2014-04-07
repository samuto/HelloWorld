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
    class EntityStack : Entity
    {
        private int count;
        public int Id;

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
        }

        internal bool Compatible(EntityStack otherStack)
        {
            return otherStack.Id == Id || this.Id == 0 || otherStack.Id == 0;
        }

        internal bool IsBlock
        {
            get
            {
                return Id > 0 && Id < ItemRepository.ItemIdOffset;
            }
        }


       

        internal bool IsItem
        {
            get
            {
                return Id >= ItemRepository.ItemIdOffset;
            }
        }

        internal Block AsBlock
        {
            get
            {
                if (!IsBlock) 
                    return null;
                return BlockRepository.Blocks[Id];
            }
        }

        internal Item AsItem
        {
            get
            {
                if (!IsItem)
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

    }
}
