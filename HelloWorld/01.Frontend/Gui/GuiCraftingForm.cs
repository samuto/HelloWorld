using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities;

namespace WindowsFormsApplication7.Frontend.Gui
{
    class GuiCraftingForm : GuiForm
    {
        private const float slotSize = 50;
        private Inventory inventory;
        private CraftingTable craftingTable;
        private GuiPanel guiCraftingProduct;
        private List<GuiPanel> craftingSlots = new List<GuiPanel>();

        private GuiMovableControl guiStackInHand;
        private ItemStack stackInHand = ItemStack.CreateEmptyStack();

        public GuiCraftingForm()
        {
            inventory = World.Instance.Player.Inventory;
            craftingTable = new CraftingTable();
            Initialize();
        }

        private void Initialize()
        {
            // Create gui stack in hand and bind it to local variable
            guiStackInHand = new GuiMovableControl();
            guiStackInHand.Size = new SlimDX.Vector2(slotSize - 10, slotSize - 10);
            guiStackInHand.Tag = stackInHand;
            guiStackInHand.AttachToCursor();
            BindControl(guiStackInHand);

            // Add some random buttons for testing puposes...
            AddControl(new GuiButton() { Location = new SlimDX.Vector2(10, 20), Size = new SlimDX.Vector2(16, 16), Text = "b1" });
            AddControl(new GuiButton() { Location = new SlimDX.Vector2(30, 20), Size = new SlimDX.Vector2(16, 16), Text = "b2" });
            AddControl(new GuiButton() { Location = new SlimDX.Vector2(50, 20), Size = new SlimDX.Vector2(16, 16), Text = "b3" });

            // setup crafting table
            GuiPanel panel;
            for (int i = 0; i < 9; i++)
            {
                int x = i % 3;
                int y = 2 - i / 3;
                panel = CreateAndBindGuiSlot(craftingTable.Grid[i], x, y+4);
                panel.OnClick += new EventHandler<EventArgs>(craftingTableSlot_OnClick);
                CreateAndBindGuiStack(craftingTable.Grid[i].Content, panel);
                craftingSlots.Add(panel);
            }
            // setup crafting table product
            guiCraftingProduct = CreateAndBindGuiSlot(craftingTable.Product, 5, 5);
            guiCraftingProduct.OnClick += new EventHandler<EventArgs>(guiCraftingProduct_OnClick);
            CreateAndBindGuiStack(craftingTable.Product.Content, guiCraftingProduct);

            // setup inventory
            for (int i = 0; i < inventory.Slots.Length; i++)
            {
                int x = i % 9;
                int y = i / 9;
                panel = CreateAndBindGuiSlot(inventory.Slots[i], x, y);
                panel.OnClick += new EventHandler<EventArgs>(inventorySlot_OnClick);
                CreateAndBindGuiStack(inventory.Slots[i].Content, panel);
            }

            AddControl(guiStackInHand);
        }

        private GuiPanel CreateAndBindGuiSlot(Slot slot, int x, int y)
        {
            GuiPanel panel = new GuiPanel()
            {
                Location = new SlimDX.Vector2((x+1) * slotSize, (y+1) * slotSize),
                Size = new SlimDX.Vector2(slotSize - 5, slotSize - 5),
                Text = "",
                Tag = slot

            };
            AddControl(panel);
            return panel;
        }

        private void CreateAndBindGuiStack(ItemStack stack, GuiPanel parent)
        {
            GuiMovableControl guiStack = new GuiMovableControl();
            guiStack.Size = new SlimDX.Vector2(slotSize - 10, slotSize - 10);
            guiStack.Tag = stack;
            parent.AddControl(guiStack);
            guiStack.CenterInParent();
            BindControl(guiStack);
        }

        private void BindControl(GuiPanel guiCraftingProduct)
        {
            BindControl((GuiMovableControl)guiCraftingProduct.Controls[0]);
        }

        private void BindControl(GuiMovableControl control)
        {
            ItemStack stack = (ItemStack)control.Tag;
            control.Text = stack.Id + "#" + stack.Count.ToString();
            control.Visible = stack.Count > 0;
        }

        void guiCraftingProduct_OnClick(object sender, EventArgs e)
        {
            // TODO: Handle picking products up in hand!
            Slot craftingProductSlot = (Slot)guiCraftingProduct.Tag;
            ItemStack craftingProductStack = craftingProductSlot.Content;
            if (craftingProductStack.IsEmpty)
                return;
            if (!stackInHand.IsEmpty && !craftingProductStack.Compatible(stackInHand))
                return;
            if (stackInHand.IsEmpty)
            {
                stackInHand.ReplaceWithEmptyCompatibleStack(craftingProductStack);
            }
            if (craftingProductStack.TransferAll(stackInHand))
            {
                craftingTable.CraftRecipe();
            }
            craftingTable.TestRecipe();
            BindControl(guiStackInHand);
            BindControl(guiCraftingProduct);
            craftingSlots.ForEach(c => BindControl(c));
        }

        void craftingTableSlot_OnClick(object sender, EventArgs e)
        {
            slot_OnClick(sender, e);
            craftingTable.TestRecipe();
            BindControl(guiCraftingProduct);
        }

        void inventorySlot_OnClick(object sender, EventArgs e)
        {
            slot_OnClick(sender, e);
        }
        
        void slot_OnClick(object sender, EventArgs e)
        {
            bool pickingUp = stackInHand.IsEmpty;
            bool leftMouse = Input.Instance.CurrentInput.MouseState.IsPressed(0);
            bool rightMouse = !leftMouse;

            // setup variables
            GuiPanel guiSelectedSlot = (GuiPanel)sender;
            GuiMovableControl guiSelectedStack = (GuiMovableControl)guiSelectedSlot.Controls[0];
            Slot selectedSlot = (Slot)guiSelectedSlot.Tag;

            // return if there is nothing to pick up
            if (pickingUp && selectedSlot.IsEmpty)
                return;

            if (pickingUp)
            {
                int transferCount = leftMouse ? selectedSlot.Content.Count : selectedSlot.Content.Count / 2;
                selectedSlot.Content.TransferItems(stackInHand, transferCount);
                BindControl(guiStackInHand);
                BindControl(guiSelectedStack);
            }
            else
            {
                int transferCount = leftMouse ? stackInHand.Count : 1;
                stackInHand.TransferItems(selectedSlot.Content, transferCount);
                BindControl(guiStackInHand);
                BindControl(guiSelectedStack);
            }
        }
    }
}
