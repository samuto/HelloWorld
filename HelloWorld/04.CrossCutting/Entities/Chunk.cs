using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Chunk
    {
        public const int SizeY = 128;
        public bool Expired;
        public PositionChunk Position;
        private byte[] blocks = new byte[16 * SizeY * 16];
        public bool RequiresRendering = true;
        public bool Initialized;

        public Chunk()
        {
        }

        public void SetLocalBlock(int x, int y, int z, int blockId)
        {
            blocks[x * SizeY * 16 + y * 16 + z] = (byte)blockId;
        }

        public byte GetLocalBlock(int x, int y, int z)
        {
            if (y < 0 || y >= SizeY) return 0;
            return blocks[x * SizeY * 16 + y * 16 + z];
        }

        public void SafeSetLocalBlock(PositionBlock positionBlock, int blockId)
        {
            PositionBlock globalPosition = Position.GetGlobalPositionBlock(positionBlock.X, positionBlock.Y, positionBlock.Z);
            if (positionBlock.X < 0 || positionBlock.X >= 16 ||
                positionBlock.Z < 0 || positionBlock.Z >= 16)
            {
                World.Instance.SetBlock(globalPosition.X, globalPosition.Y, globalPosition.Z, blockId);
                return;
            }
            SetLocalBlock(positionBlock.X, positionBlock.Y, positionBlock.Z, blockId);

        }

        internal int SafeGetLocalBlock(PositionBlock positionBlock)
        {
            PositionBlock globalPosition = Position.GetGlobalPositionBlock(positionBlock.X, positionBlock.Y, positionBlock.Z);
            if (positionBlock.X < 0 || positionBlock.X >= 16 ||
                positionBlock.Z < 0 || positionBlock.Z >= 16)
            {
                return World.Instance.GetBlock(globalPosition.X, globalPosition.Y, globalPosition.Z);
            }
            return GetLocalBlock(positionBlock.X, positionBlock.Y, positionBlock.Z);
        }


        public void SetLocalBlock(PositionBlock positionBlock, int blockId)
        {
            SetLocalBlock(positionBlock.X, positionBlock.Y, positionBlock.Z, blockId);
        }

        internal int GetLocalBlock(PositionBlock positionBlock)
        {
            return GetLocalBlock(positionBlock.X, positionBlock.Y, positionBlock.Z);
        }

        internal void Dipose()
        {
        }


        internal bool ReplaceBlock(PositionBlock pos, int oldId, int newId)
        {
            if (oldId == SafeGetLocalBlock(pos))
            {
                SafeSetLocalBlock(pos, newId);
                return true;
            }
            return false;
        }

        internal void Initialize()
        {
            World.Instance.Generate(this);
            Initialized = true;
        }
    }
}
