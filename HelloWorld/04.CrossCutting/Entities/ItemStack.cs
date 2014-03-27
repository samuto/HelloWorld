using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class ItemStack
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

        public ItemStack(int blockId, int count)
        {
            this.Id = blockId;
            this.count = count;
        }

        internal bool Compatible(ItemStack otherStack)
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

        internal void TransferItems(ItemStack destinationStack, int transferCount)
        {
            if (!Compatible(destinationStack))
                return;
            destinationStack.Id = this.Id;
            if (destinationStack.Count + transferCount > 64)
            {
                transferCount = 64 - destinationStack.Count;
            }
            destinationStack.AddItems(transferCount);
            RemoveItems(transferCount);
        }

        internal void AddItems(int addCount)
        {
            count += addCount;
        }

        internal void RemoveItems(int consumeCount)
        {
            count -= consumeCount;
            if (Count <= 0)
            {
                count = 0;
                Id = 0;
            }
        }

        internal bool TransferAll(ItemStack destinationStack)
        {
            if (destinationStack.Count + Count > 64)
                return false;
            TransferItems(destinationStack, Count);
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

        internal void ReplaceWith(ItemStack itemStack)
        {
            Id = itemStack.Id;
            count = itemStack.Count;
        }

        internal void ReplaceWithEmptyCompatibleStack(ItemStack stackInHand)
        {
            ReplaceWith(stackInHand.Id, 0);
        }

        internal static ItemStack CreateEmptyStack()
        {
            return new ItemStack(0, 0);
        }
    }
}
