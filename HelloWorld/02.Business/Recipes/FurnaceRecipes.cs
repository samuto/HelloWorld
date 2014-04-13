using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.Business.Recipes
{
    class FurnaceRecipes : RecipesBase
    {
        public static FurnaceRecipes Instance = new FurnaceRecipes();

        public FurnaceRecipes()
        {
            AddRecipe(new Recepe().Input("1", BlockRepository.CobbleStone.Id).Output(BlockRepository.Stone.Id, 1));
            AddRecipe(new Recepe().Input("1", BlockRepository.Sand.Id).Output(BlockRepository.Glass.Id, 1));
        }
    }
}
