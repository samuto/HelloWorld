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
    class BlockCraftingTable : Block
    {
        public BlockCraftingTable(int blockId) : base(blockId) { }

        internal override bool OnActivate(PositionBlock position)
        {
            TheGame.Instance.OpenGui(new GuiCraftingForm());
            return true;
        }
    }
}
