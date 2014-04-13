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
    class GuiPlayerInventoryForm : GuiInventoryForm
    {
        private GuiPanel[] guiCraftingSlots = new GuiPanel[4];
        private GuiPanel guiProduct;
        private CraftingTable craftingTable = new CraftingTable();

        public GuiPlayerInventoryForm()
        {
            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i < 4; i++)
            {
                int x = i % 2;
                int y = i / 2;

                guiCraftingSlots[i] = CreateAndBindGuiSlot(craftingTable.Grid[y * 3 + x], x, 6 - y);
            }
            guiProduct = CreateAndBindGuiSlot(craftingTable.Product, 3, 5);
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

        internal override void OnClose()
        {
            foreach (Slot slot in craftingTable.Grid)
            {
                if(!slot.Content.IsEmpty)
                    player.ThrowStack(slot.Content);
            }
            base.OnClose();
            
        }
    }
}