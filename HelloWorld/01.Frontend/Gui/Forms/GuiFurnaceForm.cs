using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities;
using SlimDX;
using WindowsFormsApplication7.Business.Geometry;
using WindowsFormsApplication7._01.Frontend.Gui.Controls;
using WindowsFormsApplication7.Frontend.Gui.Controls;

namespace WindowsFormsApplication7.Frontend.Gui.Forms
{
    class GuiFurnaceForm : GuiInventoryForm
    {
        private Furnace furnace;

        private GuiPanel guiFuel;
        private GuiPanel guiInput;
        private GuiPanel guiProduct;
        
        public GuiFurnaceForm(Furnace furnace)
        {
            this.furnace = furnace;
            
        }

        internal override void OnLoad()
        {
            // init code here..
            guiFuel = CreateAndBindGuiSlot(furnace.Fuel, 3, 5);
            guiInput = CreateAndBindGuiSlot(furnace.Input, 3, 7);
            guiProduct = CreateAndBindGuiSlot(furnace.Product, 5, 6);
            furnace.Progress += new EventHandler(furnace_Progress);
            furnace.Changed += new EventHandler(furnace_Changed);
            base.OnLoad();
        }

        void furnace_Changed(object sender, EventArgs e)
        {
            BindControl((GuiStackControl)guiFuel.Controls[0]);
            BindControl((GuiStackControl)guiInput.Controls[0]);
            BindControl((GuiStackControl)guiProduct.Controls[0]);
        }

        void furnace_Progress(object sender, EventArgs e)
        {
            
        }

        protected override void OnPickUp(Controls.GuiPanel guiSlot, Controls.GuiStackControl guiStack, Slot slot)
        {
            
        }

        protected override bool OnBeforeTransfer(Controls.GuiPanel guiSlot, Controls.GuiStackControl guiStack, Slot slot)
        {
            if (slot == furnace.Fuel)
            {
                // we can only use blocks as fuel if they have HOF greater than 0
                return stackInHand.AsEntity.HeatOfCombustion > 0;
            }
            else if (slot == furnace.Product)
            {
                return false;
            }
            return true;
        }

        protected override void OnAfterTransfer(Controls.GuiPanel guiSlot, Controls.GuiStackControl guiStack, Slot slot)
        {

        }

        internal override void OnClose()
        {
            furnace.Save();
            base.OnClose();
        }
    }
}
