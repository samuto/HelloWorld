using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities;
using SlimDX;
using WindowsFormsApplication7.Business.Geometry;
using WindowsFormsApplication7.Frontend.Gui.Controls;

namespace WindowsFormsApplication7.Frontend.Gui.Forms
{
    class GuiCraftingForm : GuiInventoryForm
    {
        private GuiPanel[] guiCraftingSlots = new GuiPanel[9];
        private GuiPanel guiProduct;
        private CraftingTable craftingTable = new CraftingTable();

        public GuiCraftingForm()
        {
            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i < 9; i++)
            {
                int x = i % 3;
                int y = i / 3;

                guiCraftingSlots[i] = CreateAndBindGuiSlot(craftingTable.Grid[y * 3 + x], x, 7 - y);
            }
            guiProduct = CreateAndBindGuiSlot(craftingTable.Product, 4, 6);
        }

        protected override bool OnPickUp(Controls.GuiPanel guiSlot, Controls.GuiStackControl guiStack, Slot slot)
        {
            if (slot == craftingTable.Product)
            {
                PickUpProduct();
                return true;
            }
            else
            {
                BindAll();
                return false;
            }
        }

        protected override bool OnBeforeTransfer(Controls.GuiPanel guiSlot, Controls.GuiStackControl guiStack, Slot slot)
        {
            if (slot == craftingTable.Product)
            {
                PickUpProduct();
                return true;
            }
            return false;
        }

        private void PickUpProduct()
        {
            if (craftingTable.Product.IsEmpty)
                return;
            if (craftingTable.Product.Content.TransferAll(stackInHand))
                craftingTable.CraftRecipe();
            BindAll();
        }

        protected override void OnAfterTransfer(Controls.GuiPanel guiSlot, Controls.GuiStackControl guiStack, Slot slot)
        {
            BindAll();
        }

        protected override void OnAfterPickUp(Controls.GuiPanel guiSlot, Controls.GuiStackControl guiStack, Slot slot)
        {
            BindAll();
        }

        private void BindAll()
        {
            craftingTable.TestRecipe();
            foreach (GuiPanel guiSlot in guiCraftingSlots)
            {
                BindControl((GuiStackControl)guiSlot.Controls[0]);
            }
            BindControl((GuiStackControl)guiProduct.Controls[0]);
            BindControl(guiStackInHand);

        }
    }
}