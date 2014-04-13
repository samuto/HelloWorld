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
    class BlockWater : Block
    {
        internal override bool FaceVisibleByNeighbor(int neighborBlockId)
        {
            if (neighborBlockId == BlockRepository.Water.Id)
                return false;
            return true;
        }

        internal override Entity CreateEntity()
        {
            return new Water();
        }

        internal override int[] OnDroppedIds()
        {
            return new int[0];
        }
       
    }
}
