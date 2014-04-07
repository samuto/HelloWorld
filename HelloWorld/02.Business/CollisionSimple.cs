using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.Business.Geometry;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.CrossCutting.Entities.Blocks;

namespace WindowsFormsApplication7.Business
{
    class CollisionSimple : ICollisionSystem
    {
        private Entity entity;

        public CollisionSimple(Entity entity)
        {
            this.entity = entity;
        }

        public Vector3 Resolve(CrossCutting.Entities.Entity entity)
        {
            World theWorld = World.Instance;
            Vector3 pos = entity.Position;
            // already collided? - then bubble up
            int x = MathLibrary.FloorToWorldGrid(pos.X);
            int y = MathLibrary.FloorToWorldGrid(pos.Y);
            int z = MathLibrary.FloorToWorldGrid(pos.Z);
            if (Block.FromId(theWorld.GetBlock(x, y, z)).IsOpaque)
            {
                // allready collided
                entity.Velocity = new Vector3();
                if (!Block.FromId(theWorld.GetBlock(x - 1, y, z)).IsOpaque)
                    return pos + new Vector3(-1, 0, 0);
                if (!Block.FromId(theWorld.GetBlock(x + 1, y, z)).IsOpaque)
                    return pos + new Vector3(1, 0, 0);
                if (!Block.FromId(theWorld.GetBlock(x, y, z-1)).IsOpaque)
                    return pos + new Vector3(0, 0, -1);
                if (!Block.FromId(theWorld.GetBlock(x, y, z+1)).IsOpaque)
                    return pos + new Vector3(0, 0, 1);
                if (!Block.FromId(theWorld.GetBlock(x, y+1, z)).IsOpaque)
                    return pos + new Vector3(0, 1, 0);
                if (!Block.FromId(theWorld.GetBlock(x, y-1, z)).IsOpaque)
                    return pos + new Vector3(0, -1, 0); 
                // could not be resolved .. entity should be killed!
                throw new NotImplementedException("Kill entity in this case!");
            }
            
            Vector3 v = entity.Velocity;
            Vector3 destpos = pos + v;
            x = MathLibrary.FloorToWorldGrid(destpos.X);
            y = MathLibrary.FloorToWorldGrid(destpos.Y);
            z = MathLibrary.FloorToWorldGrid(destpos.Z);
            Block block = Block.FromId(theWorld.GetBlock(x, y, z));
            if (block.IsOpaque)
            {
                entity.Velocity = -entity.Velocity / 2f;
                v = new Vector3();
            }
            return pos+v;
        }

    }
}
