using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;
using WindowsFormsApplication7.Business.Geometry;
using SlimDX.DirectInput;
using WindowsFormsApplication7.Frontend.Gui;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Player : Entity
    {
        public int SelectedSlotId = 0;
        public Inventory Inventory = new Inventory();
        public float BreakCompletePercentage;
        public Vector4 prevBreakPosition;
        public Vector4 BreakPosition;
        private int resting = 0;

        public Player()
            : base(new Vector4(1, 0, 0, 1))
        {
            AABB = new AxisAlignedBoundingBox(new Vector3(-0.4f, 0f, -0.4f), new Vector3(0.4f, 1.7f, 0.4f));
            EyePosition = new Vector3(0, AABB.Max.Y - 0.1f, 0);
            Speed = 0.15f;

            Inventory.Slots[0].Content.ReplaceWith(BlockRepository.Wood.Id, 64);
            Inventory.Slots[1].Content.ReplaceWith(BlockRepository.CobbleStone.Id, 64);
        }

        public bool IsSelectedItemABlock()
        {
            return Inventory.Slots[SelectedSlotId].Content.IsBlock;
        }

        public ItemStack SelectedItemStack
        {
            get
            {
                return Inventory.Slots[SelectedSlotId].Content;
            }
        }

        internal override void Update()
        {
            HandleInput();
            base.Update();
        }

        private void HandleInput()
        {
            // handle keyboard
            Player player = World.Instance.Player;
            KeyboardState prevKeyboardState = Input.Instance.CurrentInput.KeyboardState;
            KeyboardState keyboardState = Input.Instance.LastInput.KeyboardState;
            bool inGuiMode = TheGame.Instance.Mode == TheGame.GameMode.Gui;
            if (prevKeyboardState.IsPressed(Key.E) && !inGuiMode)
            {
                TheGame.Instance.OpenGui(new GuiCraftingForm());
            }
            else if (prevKeyboardState.IsPressed(Key.D1))
            {
                player.SelectedSlotId = 0;
            }
            else if (prevKeyboardState.IsPressed(Key.D2))
            {
                player.SelectedSlotId = 1;
            }
            else if (prevKeyboardState.IsPressed(Key.D3))
            {
                player.SelectedSlotId = 2;
            }
            else if (prevKeyboardState.IsPressed(Key.D4))
            {
                player.SelectedSlotId = 3;
            }
            else if (prevKeyboardState.IsPressed(Key.D5))
            {
                player.SelectedSlotId = 4;
            }
            else if (prevKeyboardState.IsPressed(Key.D6))
            {
                player.SelectedSlotId = 5;
            }
            else if (prevKeyboardState.IsPressed(Key.D7))
            {
                player.SelectedSlotId = 6;
            }
            else if (prevKeyboardState.IsPressed(Key.D8))
            {
                player.SelectedSlotId = 7;
            }
            else if (prevKeyboardState.IsPressed(Key.D9))
            {
                player.SelectedSlotId = 8;
            }
            if (inGuiMode)
                return;
            if (!World.Instance.PlayerVoxelTrace.Hit)
                return;
            if (resting > 0)
            {
                resting--;
                return;
            }
            resting = 0;

            // handle mouse
            MouseState mouseState = Input.Instance.CurrentInput.MouseState;
            MouseState prevState = Input.Instance.LastInput.MouseState;
            bool mouseLeft = mouseState.IsPressed(0);
            bool mouseRight = mouseState.IsPressed(1);
            bool prevMouseLeft = prevState.IsPressed(0);
            bool mouseHold = mouseLeft && prevMouseLeft;

            // are we punching blocks?
            if (mouseHold && mouseLeft)
            {
                BreakPosition = World.Instance.PlayerVoxelTrace.BuildPosition;
                bool sameBlock = prevBreakPosition == BreakPosition;
                prevBreakPosition = BreakPosition;
                if (sameBlock)
                {
                    int itemIdToDestroy = World.Instance.GetBlock((int)BreakPosition.X, (int)BreakPosition.Y, (int)BreakPosition.Z);
                    float efficiency = ToolMatrix.GetEfficiency(player.SelectedItemStack.Id, itemIdToDestroy);
                    BreakCompletePercentage += (100f / (float)Block.PunchesToBreak) * efficiency;
                }
                else
                    BreakCompletePercentage = 0f;
                if (BreakCompletePercentage >= 100f)
                {
                    BreakBlock(BreakPosition);
                    BreakCompletePercentage = 0;
                    resting = 4;
                }
            }
            else
            {
                BreakCompletePercentage = 0f;
            }

            // are we setting blocks?
            if (mouseRight && player.IsSelectedItemABlock())
            {
                Vector4 pos = World.Instance.PlayerVoxelTrace.ImpactPosition;
                PositionBlock posBlock = new PositionBlock((int)pos.X, (int)pos.Y, (int)pos.Z);
                World.Instance.SetBlock(posBlock.X, posBlock.Y, posBlock.Z, player.SelectedItemStack.Id);
                PositionChunk posChunk = PositionChunk.CreateFrom(posBlock);
                Chunk chunk = World.Instance.GetChunk(posChunk);
                chunk.InvalidateMeAndNeighbors();
                player.SelectedItemStack.RemoveItems(1);
                resting = 4;
            }
        }

        private void BreakBlock(Vector4 pos)
        {
            PositionBlock posBlock = new PositionBlock((int)pos.X, (int)pos.Y, (int)pos.Z);
            int blockId = World.Instance.GetBlock(posBlock.X, posBlock.Y, posBlock.Z);
            World.Instance.Player.Inventory.AddBlock(blockId);
            World.Instance.SetBlock(posBlock.X, posBlock.Y, posBlock.Z, 0);
            PositionChunk posChunk = PositionChunk.CreateFrom(posBlock);
            Chunk chunk = World.Instance.GetChunk(posChunk);
            chunk.InvalidateMeAndNeighbors();
        }
    }
}
