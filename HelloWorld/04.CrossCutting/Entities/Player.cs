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
        
        public Player()
            : base(new Vector4(1, 0, 0, 1))
        {
            AABB = new AxisAlignedBoundingBox(new Vector3(-0.4f, 0f, -0.4f), new Vector3(0.4f, 1.7f, 0.4f));
            EyePosition = new Vector3(0, AABB.Max.Y-0.1f, 0);
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
            KeyboardState prevKeyboardState = Input.Instance.CurrentInput.KeyboardState;
            KeyboardState keyboardState = Input.Instance.LastInput.KeyboardState;
            bool weAreInGame = TheGame.Instance.Mode == TheGame.GameMode.InGame;
            if (prevKeyboardState.IsPressed(Key.E) && weAreInGame)
            {
                TheGame.Instance.OpenGui(new GuiCraftingForm());
            }
            else if (prevKeyboardState.IsPressed(Key.D1))
            {
                World.Instance.Player.SelectedSlotId = 0;
            }
            else if (prevKeyboardState.IsPressed(Key.D2))
            {
                World.Instance.Player.SelectedSlotId = 1;
            }
            else if (prevKeyboardState.IsPressed(Key.D3))
            {
                World.Instance.Player.SelectedSlotId = 2;
            }
            else if (prevKeyboardState.IsPressed(Key.D4))
            {
                World.Instance.Player.SelectedSlotId = 3;
            }
            else if (prevKeyboardState.IsPressed(Key.D5))
            {
                World.Instance.Player.SelectedSlotId = 4;
            }
            else if (prevKeyboardState.IsPressed(Key.D6))
            {
                World.Instance.Player.SelectedSlotId = 5;
            }
            else if (prevKeyboardState.IsPressed(Key.D7))
            {
                World.Instance.Player.SelectedSlotId = 6;
            }
            else if (prevKeyboardState.IsPressed(Key.D8))
            {
                World.Instance.Player.SelectedSlotId = 7;
            }
            else if (prevKeyboardState.IsPressed(Key.D9))
            {
                World.Instance.Player.SelectedSlotId = 8;
            }

            // handle mouse
            MouseState mouseState = Input.Instance.CurrentInput.MouseState;
            if (weAreInGame && World.Instance.PlayerVoxelTrace.Hit && !Input.Instance.IsMouseFreezed())
            {
                if (mouseState.IsPressed(1) && World.Instance.Player.IsSelectedItemABlock())
                {
                    Input.Instance.FreezeMouse();
                    Vector4 pos = World.Instance.PlayerVoxelTrace.ImpactPosition;
                    PositionBlock posBlock = new PositionBlock((int)pos.X, (int)pos.Y, (int)pos.Z);

                    World.Instance.SetBlock(posBlock.X, posBlock.Y, posBlock.Z, World.Instance.Player.SelectedItemStack.Id);
                    PositionChunk posChunk = PositionChunk.CreateFrom(posBlock);
                    Chunk chunk = World.Instance.GetChunk(posChunk);
                    chunk.InvalidateMeAndNeighbors();

                    World.Instance.Player.SelectedItemStack.RemoveItems(1);
                }
                else if (mouseState.IsPressed(0))
                {
                    Input.Instance.FreezeMouse();
                    Vector4 pos = World.Instance.PlayerVoxelTrace.BuildPosition;
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
    }
}
