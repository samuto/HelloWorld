using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Frontend;
using WindowsFormsApplication7.Business;
using SlimDX;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.Business.Geometry;
using WindowsFormsApplication7.Frontend.Gui;
using WindowsFormsApplication7.Frontend.Gui.Forms;

namespace WindowsFormsApplication7.CrossCutting.Entities.Blocks
{
    class BlockFurnace : Block
    {
        public BlockFurnace(int blockId) : base(blockId) { }

        internal override bool OnActivate(PositionBlock position)
        {
            Furnace furnace = (Furnace)World.Instance.GetBlockEntity(position);
            TheGame.Instance.OpenGui(new GuiFurnaceForm(furnace));
            return true;
        }

        internal override void OnDestroy(PositionBlock pos)
        {
            // drop furnace content
            Furnace furnace = (Furnace)World.Instance.GetBlockEntity(pos);
            DropStack(furnace.Product.Content, pos);
            DropStack(furnace.Input.Content, pos);
            DropStack(furnace.Fuel.Content, pos);

            base.OnDestroy(pos);
        }

        internal override Entity CreateEntity()
        {
            return new Furnace();
        }
    }
}
