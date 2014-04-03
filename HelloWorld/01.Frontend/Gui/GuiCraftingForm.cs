using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities;
using SlimDX;

namespace WindowsFormsApplication7.Frontend.Gui
{
    class GuiCraftingForm : GuiForm
    {
        private const float slotSize = itemSize+2;
        private const float itemSize = 16;
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
            guiStackInHand.Size = new SlimDX.Vector2(itemSize, itemSize);
            guiStackInHand.Tag = stackInHand;
            guiStackInHand.AttachToCursor();
            guiStackInHand.OnRender += new EventHandler<EventArgs>(guiStack_OnRender);
            BindControl(guiStackInHand);

            // setup crafting table
            GuiPanel panel;
            for (int i = 0; i < 9; i++)
            {
                int x = i % 3;
                int y = 2 - i / 3;
                panel = CreateAndBindGuiSlot(craftingTable.Grid[i], x, y + 5);
                panel.OnClick += new EventHandler<EventArgs>(craftingTableSlot_OnClick);
                CreateAndBindGuiStack(craftingTable.Grid[i].Content, panel);
                craftingSlots.Add(panel);
            }
            // setup crafting table product
            guiCraftingProduct = CreateAndBindGuiSlot(craftingTable.Product, 4, 6);
            guiCraftingProduct.OnClick += new EventHandler<EventArgs>(guiCraftingProduct_OnClick);
            CreateAndBindGuiStack(craftingTable.Product.Content, guiCraftingProduct);

            // setup inventory
            for (int i = 0; i < inventory.Slots.Length; i++)
            {
                int x = i % 9;
                int y = i / 9;
                if (y > 0) y++; 
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
                Location = new SlimDX.Vector2((x + 1) * slotSize, (y + 1) * slotSize),
                Size = new SlimDX.Vector2(slotSize, slotSize),
                Text = "",
                Tag = slot

            };
            AddControl(panel);
            return panel;
        }

        private void CreateAndBindGuiStack(ItemStack stack, GuiPanel parent)
        {
            GuiMovableControl guiStack = new GuiMovableControl();
            guiStack.Size = new SlimDX.Vector2(itemSize, itemSize);
            guiStack.Tag = stack;
            parent.AddControl(guiStack);
            guiStack.CenterInParent();
            guiStack.OnRender += new EventHandler<EventArgs>(guiStack_OnRender);
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

        void guiStack_OnRender(object sender, EventArgs e)
        {
            GuiMovableControl control = (GuiMovableControl)sender;
            Vector3 pos = new Vector3(
                control.Location.X+control.ParentLocation.X, 
                control.Location.Y+control.ParentLocation.Y, 
                0);
            Tessellator t = Tessellator.Instance;
            t.StartDrawingTiledQuads();
            Camera.Instance.World = Matrix.Multiply(Camera.Instance.World, Matrix.Scaling(new Vector3(itemSize, itemSize, itemSize)));
            Camera.Instance.World = Matrix.Multiply(Camera.Instance.World, Matrix.Translation(pos));
            ItemStack stack = (ItemStack)control.Tag;
            t.Draw(TileTextures.Instance.GetItemVertexBuffer(stack.Id));
            
            FontRenderer f = FontRenderer.Instance;
            t.StartDrawingAlphaTexturedQuads("ascii");
            string text = stack.Count.ToString();
            Vector2 textSize = f.TextSize(text);
            f.RenderText(text, 
                control.Location.X + control.ParentLocation.X,
                control.Location.Y + control.ParentLocation.Y);
            t.Draw();
        }


        void guiCraftingProduct_OnClick(object sender, EventArgs e)
        {
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
                int beforeCount = stackInHand.Count;
                stackInHand.TransferItems(selectedSlot.Content, transferCount);
                bool stackInHandUnchanged = beforeCount == stackInHand.Count;
                if (stackInHandUnchanged && leftMouse)
                {
                    stackInHand.Swap(selectedSlot.Content);
                }
                BindControl(guiStackInHand);
                BindControl(guiSelectedStack);
            }
        }
    }
}
