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
using WindowsFormsApplication7.CrossCutting.Entities.Items;
using WindowsFormsApplication7.CrossCutting.Entities.Blocks;

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
            Inventory.Slots[2].Content.ReplaceWith(ItemRepository.StonePickAxe.Id, 1);
            Inventory.Slots[3].Content.ReplaceWith(ItemRepository.StoneAxe.Id, 1);
            Inventory.Slots[4].Content.ReplaceWith(ItemRepository.StoneShovel.Id, 1);
            Inventory.Slots[5].Content.ReplaceWith(ItemRepository.StoneHoe.Id, 1);

            collisionSystem = new CollisionComplex(this);
        }

        public bool IsSelectedItemABlock()
        {
            return Inventory.Slots[SelectedSlotId].Content.IsBlock;
        }

        private bool IsSelectedItemAnItem()
        {
            return Inventory.Slots[SelectedSlotId].Content.IsItem;
        }

        public EntityStack SelectedStack
        {
            get
            {
                return Inventory.Slots[SelectedSlotId].Content;
            }
        }

        internal override void Update()
        {
            HandleInput();
            CalculateVelocity();
            base.Update();
            CollectNearbyItems();
        }

        protected void CalculateVelocity()
        {

            Vector3 forward = Direction;
            forward.Y = 0;
            Vector3 left = new Vector3(forward.Z, 0, -forward.X);
            Vector3 direction = new Vector3();
            if (moveLeft)
                Vector3.Add(ref direction, ref left, out direction);
            if (moveRight)
                Vector3.Subtract(ref direction, ref left, out direction);
            if (moveForward)
                Vector3.Add(ref direction, ref forward, out direction);
            if (moveBackward)
                Vector3.Subtract(ref direction, ref forward, out direction);
            direction.Normalize();
            Vector3.Multiply(ref direction, Speed, out direction);
            Velocity.X = direction.X;
            Velocity.Z = direction.Z;

            if (onGround)
                Velocity.Y = moveJump ? 0.25f : 0;
            if (moveUp)
                Velocity.Y += Speed;
            if (moveDown)
                Velocity.Y -= Speed;

        }

        private void CollectNearbyItems()
        {
            AxisAlignedBoundingBox collectArea = AABB;
            collectArea.Translate(Position);
            collectArea.Min -= new Vector3(1, 1, 1);
            collectArea.Max += new Vector3(1, 1, 1);
            PositionChunk minChunk = PositionChunk.CreateFrom(collectArea.Min);
            PositionChunk maxChunk = PositionChunk.CreateFrom(collectArea.Max);
            PositionChunk chunkPos;
            for (int x = minChunk.X; x <= maxChunk.X; x++)
            {
                for (int y = minChunk.Y; y <= maxChunk.Y; y++)
                {
                    for (int z = minChunk.Z; z <= maxChunk.Z; z++)
                    {
                        chunkPos.X = x;
                        chunkPos.Y = y;
                        chunkPos.Z = z;
                        Chunk chunk = World.Instance.GetChunk(chunkPos);
                        foreach (EntityStack stack in chunk.EntitiesInArea(collectArea))
                        {
                            // transfer items
                            Inventory.CollectStack(stack);
                            // if stack is empty remove it
                            if (stack.IsEmpty)
                            {
                                chunk.StackEntities.Remove(stack);
                            }
                        }
                    }
                }
            }

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
                BreakPosition = World.Instance.PlayerVoxelTrace.ImpactPosition;
                bool sameBlock = prevBreakPosition == BreakPosition;
                prevBreakPosition = BreakPosition;
                if (sameBlock)
                {
                    int id = player.SelectedStack.Id;
                    int blockToDestroy = World.Instance.GetBlock((int)BreakPosition.X, (int)BreakPosition.Y, (int)BreakPosition.Z);
                    float efficiency = ToolMatrix.GetEfficiency(id, blockToDestroy);
                    Block block = Block.FromId(blockToDestroy);
                    BreakCompletePercentage += (100f / (float)block.Density) * efficiency;
                    if (player.SelectedStack.IsItem)
                    {
                        Item itemInHand = Item.FromId(id);
                        itemInHand.OnPunchedWith();
                    }
                }
                else
                    BreakCompletePercentage = 0f;
                if (BreakCompletePercentage >= 100f)
                {
                    BreakBlock(BreakPosition);
                    BreakCompletePercentage = 0;
                    resting = 0;
                }
            }
            else
            {
                BreakCompletePercentage = 0f;
            }

            // are we setting blocks?
            if (mouseRight && player.IsSelectedItemABlock())
            {
                Vector4 pos = World.Instance.PlayerVoxelTrace.BuildPosition;
                PositionBlock posBlock = new PositionBlock((int)pos.X, (int)pos.Y, (int)pos.Z);
                World.Instance.SetBlock(posBlock.X, posBlock.Y, posBlock.Z, player.SelectedStack.Id);
                PositionChunk posChunk = PositionChunk.CreateFrom(posBlock);
                Chunk chunk = World.Instance.GetChunk(posChunk);
                chunk.InvalidateMeAndNeighbors();
                player.SelectedStack.Remove(1);
                resting = 6;
            }
            // are we using a tool?
            else if (mouseRight && player.IsSelectedItemAnItem())
            {
                Vector4 pos = World.Instance.PlayerVoxelTrace.ImpactPosition;
                PositionBlock posBlock = new PositionBlock((int)pos.X, (int)pos.Y, (int)pos.Z);
                Item item = Item.FromId(SelectedStack.Id);
                if (item.UseOnBlock(posBlock))
                {
                    PositionChunk posChunk = PositionChunk.CreateFrom(posBlock);
                    Chunk chunk = World.Instance.GetChunk(posChunk);
                    chunk.InvalidateMeAndNeighbors();
                    if (item.Consumable)
                    {
                        player.SelectedStack.Remove(1);
                    }
                }
                
                resting = 6;
            }
        }

       

       

        private void BreakBlock(Vector4 pos)
        {
            PositionBlock posBlock = new PositionBlock((int)pos.X, (int)pos.Y, (int)pos.Z);
            Block block = Block.FromId(World.Instance.GetBlock(posBlock.X, posBlock.Y, posBlock.Z));
            int[] droppedIds = block.DroppedIds();
            if (droppedIds.Length > 0)
            {
                foreach (int id in droppedIds)
                {
                    EntityStack stackToSpawn = new EntityStack(id, 1);
                    stackToSpawn.Position = new Vector3(pos.X + 0.5f, pos.Y + 0.5f, pos.Z + 0.5f);
                    stackToSpawn.Yaw = (float)(MathLibrary.GlobalRandom.NextDouble() * Math.PI);
                    stackToSpawn.Pitch = (float)(MathLibrary.GlobalRandom.NextDouble() * Math.PI);
                    stackToSpawn.Velocity = new Vector3(
                        (float)0.05f * (float)(MathLibrary.GlobalRandom.NextDouble() * 2.0 - 1.0),
                        (float)0.25f,
                        (float)0.05f * (float)(MathLibrary.GlobalRandom.NextDouble() * 2.0 - 1.0));
                    World.Instance.SpawnStack(stackToSpawn);
                }
            }
            World.Instance.SetBlock(posBlock.X, posBlock.Y, posBlock.Z, 0);
            PositionChunk posChunk = PositionChunk.CreateFrom(posBlock);
            Chunk chunk = World.Instance.GetChunk(posChunk);
            chunk.InvalidateMeAndNeighbors();
        }
    }
}
