using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.Business.Repository
{
    class RecepeRepository
    {
        private static Dictionary<string, Recepe> recepies = new Dictionary<string, Recepe>();

        static RecepeRepository()
        {
            AddToRepository(new Recepe().Input("1", BlockRepository.Wood.Id).Output(BlockRepository.Plank.Id, 4));
            AddToRepository(new Recepe().Input("1,1", BlockRepository.Plank.Id).Output(ItemRepository.Stick.Id, 4));
            AddToRepository(new Recepe().Input("111,020,020", BlockRepository.CobbleStone.Id, ItemRepository.Stick.Id).Output(ItemRepository.StonePickAxe.Id, 1));
        }

        private static void AddToRepository(Recepe recepe)
        {
            recepies.Add(recepe.Key, recepe);
        }

        internal static Recepe FindRecepe(Slot[] Grid)
        {
            string key = Recepe.GetKeyFromGrid(Grid);
            if(recepies.ContainsKey(key))
                return recepies[key];
            return null;
        }
    }
}
