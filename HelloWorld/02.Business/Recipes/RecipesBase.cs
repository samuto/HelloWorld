using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.Business.Recipes
{
    class RecipesBase
    {
        protected Dictionary<string, Recepe> recepies = new Dictionary<string, Recepe>();

        protected void AddRecipe(Recepe recepe)
        {
            recepies.Add(recepe.Key, recepe);
        }

        internal Recepe Find(Slot input)
        {
            return Find(new Slot[] { input, new Slot(), new Slot(), new Slot(), new Slot(), new Slot(), new Slot(), new Slot(), new Slot() });
        }

        internal Recepe Find(Slot[] Grid)
        {
            string key = Recepe.GetKeyFromGrid(Grid);
            if (recepies.ContainsKey(key))
                return recepies[key];
            return null;
        }
    }
}
