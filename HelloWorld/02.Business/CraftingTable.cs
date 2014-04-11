using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.Business.Recipes;

namespace WindowsFormsApplication7.Business
{
    class CraftingTable
    {
        public Slot[] Grid = new Slot[9];
        public Slot Product = new Slot();

        public CraftingTable()
        {
            for (int i = 0; i < Grid.Length; i++)
            {
                Grid[i] = new Slot();
            }
        }

        public void TestRecipe()
        {
            Product.Content.Clear();
            Recepe recepe = CraftingTableRecipes.Instance.Find(Grid);
            if (recepe == null)
                return;
            Product.Content.ReplaceWith(recepe.Product);
        }

        public EntityStack CraftRecipe()
        {
            TestRecipe();
            EntityStack createdProduct = Product.Content;
            if (createdProduct == null) return null;
            
            // Craft it!
            Product.Content.Clear();
            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].Content != null)
                {
                    Grid[i].Content.Remove(1);
                }
            }
            
            return createdProduct;
        }
    }
}
