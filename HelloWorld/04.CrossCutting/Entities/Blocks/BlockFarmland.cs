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
    class BlockFarmland : Block
    {
        
        internal override int[] OnDroppedIds()
        {
            return new int[] { BlockRepository.Dirt.Id};
        }

        internal override Entity CreateEntity()
        {
            return new Farmland();
        }

    }
}
