using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.Business.Recipes
{
    class CraftingTableRecipes : RecipesBase
    {
        public static CraftingTableRecipes Instance = new CraftingTableRecipes();

        private CraftingTableRecipes()
        {
            AddRecipe(new Recepe().Input("1", BlockRepository.Wood.Id).Output(BlockRepository.Plank.Id, 4));
            AddRecipe(new Recepe().Input("1,1", BlockRepository.Plank.Id).Output(ItemRepository.Stick.Id, 4));
            AddRecipe(new Recepe().Input("11,12,02", BlockRepository.CobbleStone.Id, ItemRepository.Stick.Id).Output(ItemRepository.StoneAxe.Id, 1));
            AddRecipe(new Recepe().Input("11,02,02", BlockRepository.CobbleStone.Id, ItemRepository.Stick.Id).Output(ItemRepository.StoneHoe.Id, 1));
            AddRecipe(new Recepe().Input("111,020,020", BlockRepository.CobbleStone.Id, ItemRepository.Stick.Id).Output(ItemRepository.StonePickAxe.Id, 1));
            AddRecipe(new Recepe().Input("1,2,2", BlockRepository.CobbleStone.Id, ItemRepository.Stick.Id).Output(ItemRepository.StoneShovel.Id, 1));
            AddRecipe(new Recepe().Input("1,1,2", BlockRepository.CobbleStone.Id, ItemRepository.Stick.Id).Output(ItemRepository.StoneSword.Id, 1));
            AddRecipe(new Recepe().Input("111", ItemRepository.Wheat.Id).Output(ItemRepository.Bread.Id, 1));
            AddRecipe(new Recepe().Input("11,11", BlockRepository.Plank.Id).Output(BlockRepository.CraftingTable.Id, 1));
            AddRecipe(new Recepe().Input("111,101,111", BlockRepository.CobbleStone.Id).Output(BlockRepository.FurnaceOff.Id, 1));
        }
    }
}
