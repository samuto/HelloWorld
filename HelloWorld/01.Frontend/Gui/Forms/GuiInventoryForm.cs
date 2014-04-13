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
    class GuiInventoryForm : GuiForm
    {
        protected const float slotSize = itemSize + 2;
        protected const float itemSize = 16;
        protected Inventory inventory = World.Instance.Player.Inventory;
        protected Player player = World.Instance.Player;

        protected GuiStackControl guiStackInHand;
        protected EntityStack stackInHand = EntityStack.CreateEmptyStack();

        public GuiInventoryForm()
        {
        }

        internal override void OnClose()
        {
            base.OnClose();
            player.ThrowStack(stackInHand);
        }

        internal override void OnLoad()
        {
            // Create gui stack in hand and bind it to local variable
            guiStackInHand = new GuiStackControl();
            guiStackInHand.Size = new SlimDX.Vector2(itemSize, itemSize);
            guiStackInHand.Tag = stackInHand;
            guiStackInHand.AttachToCursor();
            BindControl(guiStackInHand);

            // setup inventory
            GuiPanel panel;
            for (int i = 0; i < inventory.Slots.Length; i++)
            {
                int x = i % 9;
                int y = i / 9;
                if (y > 0) y++;
                panel = CreateAndBindGuiSlot(inventory.Slots[i], x, y);
            }

            // keep stack in hand in the foreground
            AddControl(guiStackInHand);
        }

        protected GuiPanel CreateAndBindGuiSlot(Slot slot, int x, int y)
        {
            GuiPanel panel = new GuiPanel()
            {
                Location = new SlimDX.Vector2((x + 1) * slotSize, (y + 1) * slotSize),
                Size = new SlimDX.Vector2(slotSize, slotSize),
                Text = "",
                Tag = slot

            };
            panel.OnClick += new EventHandler<EventArgs>(slot_OnClick);
            AddControl(panel);

            // add moveable control...
            GuiStackControl guiStack = new GuiStackControl();
            guiStack.Size = new SlimDX.Vector2(itemSize, itemSize);
            guiStack.Tag = slot.Content;
            panel.AddControl(guiStack);
            guiStack.CenterInParent();
            BindControl(guiStack);

            return panel;
        }

        private void BindControl(GuiPanel guiCraftingProduct)
        {
            BindControl((GuiStackControl)guiCraftingProduct.Controls[0]);
        }

        protected void BindControl(GuiStackControl control)
        {
            EntityStack stack = (EntityStack)control.Tag;
            control.Text = stack.Count.ToString();
            control.Visible = stack.Count > 0;
        }

       

        protected virtual bool OnPickUp(GuiPanel guiSlot, GuiStackControl guiStack, Slot slot)
        {
            return false;
        }

        protected virtual void OnAfterPickUp(GuiPanel guiSlot, GuiStackControl guiStack, Slot slot)
        {
            
        }

        protected virtual bool OnBeforeTransfer(GuiPanel guiSlot, GuiStackControl guiStack, Slot slot)
        {
            return false;
        }
        protected virtual void OnAfterTransfer(GuiPanel guiSlot, GuiStackControl guiStack, Slot slot)
        { 
        }

        void slot_OnClick(object sender, EventArgs e)
        {
            bool pickingUp = stackInHand.IsEmpty;
            bool leftMouse = Input.Instance.CurrentInput.MouseState.IsPressed(0);
            bool rightMouse = !leftMouse;

            // setup variables
            GuiPanel guiSelectedSlot = (GuiPanel)sender;
            GuiStackControl guiSelectedStack = (GuiStackControl)guiSelectedSlot.Controls[0];
            Slot selectedSlot = (Slot)guiSelectedSlot.Tag;

            // return if there is nothing to pick up
            if (pickingUp && selectedSlot.IsEmpty)
                return;

            if (pickingUp)
            {
                if (!OnPickUp(guiSelectedSlot, guiSelectedStack, selectedSlot))
                {
                    int transferCount = leftMouse ? selectedSlot.Content.Count : selectedSlot.Content.Count / 2;
                    selectedSlot.Content.TransferEntities(stackInHand, transferCount);
                    BindControl(guiStackInHand);
                    BindControl(guiSelectedStack);
                    OnAfterPickUp(guiSelectedSlot, guiSelectedStack, selectedSlot);
                }

            }
            else
            {

                if (!OnBeforeTransfer(guiSelectedSlot, guiSelectedStack, selectedSlot))
                {
                    int transferCount = leftMouse ? stackInHand.Count : 1;
                    int beforeCount = stackInHand.Count;
                    stackInHand.TransferEntities(selectedSlot.Content, transferCount);
                    bool stackInHandUnchanged = beforeCount == stackInHand.Count;
                    if (stackInHandUnchanged && leftMouse)
                    {
                        stackInHand.Swap(selectedSlot.Content);
                    }
                    BindControl(guiStackInHand);
                    BindControl(guiSelectedStack);
                    OnAfterTransfer(guiSelectedSlot, guiSelectedStack, selectedSlot);
                }
            }
        }

    }
}