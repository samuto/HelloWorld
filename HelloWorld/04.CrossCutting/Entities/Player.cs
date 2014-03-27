using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.Business.Geometry;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Player : Entity
    {
        public int SelectedBlockId = 0;
        public Inventory Inventory = new Inventory();
        
        public Player()
            : base(new Vector4(1, 0, 0, 1))
        {
            AABB = new AxisAlignedBoundingBox(new Vector3(-0.4f, 0f, -0.4f), new Vector3(0.4f, 1.7f, 0.4f));
            EyePosition = new Vector3(0, AABB.Max.Y-0.1f, 0);
            Speed = 0.15f;

            Inventory.Slots[0].Content.ReplaceWith(BlockRepository.Wood.Id, 64);
            Inventory.Slots[1].Content.ReplaceWith(BlockRepository.Stone.Id, 64);
        }

    }
}
