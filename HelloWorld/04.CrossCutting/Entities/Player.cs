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
using WindowsFormsApplication7.Frontend.Gui.Forms;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Player : EntityPlayer
    {
        public int SelectedSlotId = 0;
        public Inventory Inventory = new Inventory();
        public float DestroyProgress;
        public Vector4 prevBlockAttackPosition;
        public Vector4 BlockAttackPosition;
        private int resting = 0;
        private int throwStackDelay = 0;
        public float Health = 100;
        public float Hunger = 100;
        public bool Dead = false;

        public Player()
            : base(new Vector4(1, 0, 0, 1))
        {
            AABB = new AxisAlignedBoundingBox(new Vector3(-0.4f, 0f, -0.4f), new Vector3(0.4f, 1.7f, 0.4f));
            EyePosition = new Vector3(0, AABB.Max.Y - 0.1f, 0);
            Speed = 0.15f;

            Inventory.Slots[9].Content.ReplaceWith(BlockRepository.Wood.Id, 64);
            Inventory.Slots[10].Content.ReplaceWith(BlockRepository.CobbleStone.Id, 64);
            Inventory.Slots[11].Content.ReplaceWith(ItemRepository.Coal.Id, 64);
            Inventory.Slots[0].Content.ReplaceWith(BlockRepository.FurnaceOff.Id, 1);
            Inventory.Slots[2].Content.ReplaceWith(ItemRepository.StonePickAxe.Id, 1);
            Inventory.Slots[3].Content.ReplaceWith(ItemRepository.StoneAxe.Id, 1);
            Inventory.Slots[4].Content.ReplaceWith(ItemRepository.StoneShovel.Id, 1);
            Inventory.Slots[5].Content.ReplaceWith(ItemRepository.StoneHoe.Id, 1);
            Inventory.Slots[6].Content.ReplaceWith(ItemRepository.SeedsWheat.Id, 64);
            Inventory.Slots[7].Content.ReplaceWith(ItemRepository.Bread.Id, 64);
            Inventory.Slots[8].Content.ReplaceWith(BlockRepository.Diamond.Id, 1);

            collisionSystem = new CollisionComplex(this);
        }

       
        public EntityStack SelectedStack
        {
            get
            {
                return Inventory.Slots[SelectedSlotId].Content;
            }
        }

        internal override void OnUpdate()
        {
            // Handle hunger/health
            DecreaseHunger(-0.1f);
            if (Health == 0)
                Dead = true;
            if (Hunger == 100)
                GiveHealth(-0.1f);
            if(Hunger < 10)
                GiveHealth(0.2f);

            HandleInput();
            CalculateVelocity();
            base.OnUpdate();
            CollectNearbyEntities();
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

        private void CollectNearbyEntities()
        {
            if (throwStackDelay > 0)
            {
                throwStackDelay--;
                return;
            }

            AxisAlignedBoundingBox collectArea = AABB;
            collectArea.Translate(Position);
            float radius = 0.5f;
            collectArea.Min -= new Vector3(radius, radius, radius);
            collectArea.Max += new Vector3(radius, radius, radius);
            PositionChunk minChunk = PositionChunk.CreateFrom(collectArea.Min);
            PositionChunk maxChunk = PositionChunk.CreateFrom(collectArea.Max);
            PositionChunk chunkPos;
            for (int x = minChunk.X; x <= maxChunk.X; x++)
            {
                for (int y = minChunk.Y; y <= maxChunk.Y; y++)
                {
                    for (int z = minChunk.Z; z <= maxChunk.Z; z++)
                    {
                        chunkPos = new PositionChunk(x, y, z);
                        Chunk chunk = World.Instance.GetChunk(chunkPos);
                        foreach (EntityStack stack in chunk.EntitiesInArea(collectArea))
                        {
                            // transfer items
                            Inventory.CollectStack(stack);
                            // if stack is empty remove it
                            if (stack.IsEmpty)
                            {
                                chunk.RemoveEntity(stack);
                            }
                        }
                    }
                }
            }

        }

        private void HandleInput()
        {
            // handle keyboard
            bool inGuiMode = TheGame.Instance.Mode == TheGame.GameMode.Gui;
            HandleKeyboard();
            if (inGuiMode)
                return;
            if (DoRest())
                return;
            HandleMouse();
        }

        private bool DoRest()
        {
            if (resting == 0)
                return false;
            resting--;
            return true;
        }

        private void NeedsRest()
        {
            int updatesToRest = 6;
            resting = updatesToRest;
        }

        private void HandleMouse()
        {
            // handle mouse
            MouseState mouseState = Input.Instance.CurrentInput.MouseState;
            MouseState prevState = Input.Instance.LastInput.MouseState;
            bool mouseLeft = mouseState.IsPressed(0);
            bool mouseRight = mouseState.IsPressed(1);
            bool prevMouseLeft = prevState.IsPressed(0);
            bool mouseHold = mouseLeft && prevMouseLeft;

            // destroy / attack
            if (mouseLeft)
                HandleMouseLeft(mouseHold);
            else
                DestroyProgress = 0f;

            // use block / use item
            if (mouseRight)
                HandleMouseRight();

            
        }

        private void HandleMouseRight()
        {
            // activate block?
            PositionBlock positionBlock = PositionBlock.FromVector(World.Instance.PlayerVoxelTrace.ImpactPosition);
            Block impactBlock = Block.FromId(World.Instance.GetBlock(positionBlock));
            if (impactBlock.OnActivate(positionBlock))
                return;

            // are we using a block?
            if (SelectedStack.AsBlock != null)
            {
                UseBlockInHand();
            }
            // are we using an item?
            else if (SelectedStack.AsItem != null)
            {
                UseItemInHand();
                NeedsRest();
            }
        }

        private void UseBlockInHand()
        {
            if (World.Instance.PlayerVoxelTrace.Hit)
            {
                UseBlockOnBlock();
            }
            else
            {
                UseBlockOnMe();
            }
        }

        private void UseItemInHand()
        {
            if (World.Instance.PlayerVoxelTrace.Hit)
            {
                UseItemOnBlock();
            }
            else
            {
                UseItemOnMe();
            }
        }

        private void UseItemOnMe()
        {
            Item item = Item.FromId(SelectedStack.Id);
            if (item.OnUseOnPlayer())
            {
                if (item.Consumable)
                {
                    SelectedStack.Remove(1);
                }
            }
        }

        private void UseBlockOnMe()
        {
        }

        private void UseItemOnBlock()
        {
            // used on a block!
            Vector4 pos = World.Instance.PlayerVoxelTrace.ImpactPosition;
            PositionBlock posBlock = new PositionBlock((int)pos.X, (int)pos.Y, (int)pos.Z);
            Item item = Item.FromId(SelectedStack.Id);
            if (item.OnUseOnBlock(posBlock))
            {
                // here if world is changed
                PositionChunk posChunk = PositionChunk.CreateFrom(posBlock);
                Chunk chunk = World.Instance.GetChunk(posChunk);
                chunk.InvalidateMeAndNeighbors();
                if (item.Consumable)
                {
                    SelectedStack.Remove(1);
                }
            }
        }

        private void UseBlockOnBlock()
        {
            Vector4 pos = World.Instance.PlayerVoxelTrace.BuildPosition;
            PositionBlock posBlock = new PositionBlock((int)pos.X, (int)pos.Y, (int)pos.Z);
            World.Instance.SetBlock(posBlock, SelectedStack.Id);
            PositionChunk posChunk = PositionChunk.CreateFrom(posBlock);
            Chunk chunk = World.Instance.GetChunk(posChunk);
            chunk.InvalidateMeAndNeighbors();
            SelectedStack.Remove(1);
            NeedsRest();
        }

        private void HandleMouseLeft(bool mouseHold)
        {
            // if no block is selected then we cannot attack..
            if (!World.Instance.PlayerVoxelTrace.Hit)
                return;
            // if this is the first click then reset destroy counter.. and see if player holds down the mouse...
            if (!mouseHold)
            {
                DestroyProgress = 0f;
                return;
            }
            // are we destroying blocks?
            BlockAttackPosition = World.Instance.PlayerVoxelTrace.ImpactPosition;
            bool destroyInProgress = prevBlockAttackPosition == BlockAttackPosition;
            prevBlockAttackPosition = BlockAttackPosition;
            if(!destroyInProgress)
            {
                DestroyProgress = 0f;
                return;
            }
            
            // we are punching the block!!
            UpdateBlockDestroyProgress();
         
            // have we destroyed the block?
            if (DestroyProgress >= 100f)
            {
                DestroyBlock(BlockAttackPosition);
                DestroyProgress = 0;
                NeedsRest();
            }
        }

        private void UpdateBlockDestroyProgress()
        {
            Block blockToDestroy = Block.FromId(World.Instance.GetBlock(PositionBlock.FromVector(BlockAttackPosition)));
            float efficiency = ToolMatrix.GetEfficiency(SelectedStack.Id, blockToDestroy.Id);
            DestroyProgress += (100f / (float)blockToDestroy.Density) * efficiency;
            if (SelectedStack.AsItem != null)
            {
                Item itemInHand = Item.FromId(SelectedStack.Id);
                itemInHand.OnAfterAttack();
            }
        }

        private void HandleKeyboard()
        {
            KeyboardState prevKeyboardState = Input.Instance.CurrentInput.KeyboardState;
            KeyboardState keyboardState = Input.Instance.LastInput.KeyboardState;
            bool inGuiMode = TheGame.Instance.Mode == TheGame.GameMode.Gui;
            Player player = World.Instance.Player;
            if (prevKeyboardState.IsPressed(Key.E) && !inGuiMode)
            {
                //open player inventory gui...
                //TheGame.Instance.OpenGui(new GuiTestForm());
                TheGame.Instance.OpenGui(new GuiCraftingForm());
            }
            else if (prevKeyboardState.IsPressed(Key.D1))
            {
                SelectedSlotId = 0;
            }
            else if (prevKeyboardState.IsPressed(Key.D2))
            {
                SelectedSlotId = 1;
            }
            else if (prevKeyboardState.IsPressed(Key.D3))
            {
                SelectedSlotId = 2;
            }
            else if (prevKeyboardState.IsPressed(Key.D4))
            {
                SelectedSlotId = 3;
            }
            else if (prevKeyboardState.IsPressed(Key.D5))
            {
                SelectedSlotId = 4;
            }
            else if (prevKeyboardState.IsPressed(Key.D6))
            {
                SelectedSlotId = 5;
            }
            else if (prevKeyboardState.IsPressed(Key.D7))
            {
                SelectedSlotId = 6;
            }
            else if (prevKeyboardState.IsPressed(Key.D8))
            {
                SelectedSlotId = 7;
            }
            else if (prevKeyboardState.IsPressed(Key.D9))
            {
                SelectedSlotId = 8;
            }
        }

        private void DestroyBlock(Vector4 pos)
        {
            PositionBlock posBlock = PositionBlock.FromVector(pos);
            Block block = Block.FromId(World.Instance.GetBlock(posBlock));
            block.OnDestroy(posBlock);
            World.Instance.SetBlock(posBlock, 0);
            PositionChunk posChunk = PositionChunk.CreateFrom(posBlock);
            Chunk chunk = World.Instance.GetChunk(posChunk);
            chunk.InvalidateMeAndNeighbors();
        }

        internal void ThrowStack(EntityStack stack)
        {
            // drop items in crafting table and in hand...
            stack.Position = Position + EyePosition;
            stack.Position.Y -= (float)(MathLibrary.GlobalRandom.NextDouble() * 0.5f);
            stack.Velocity = Direction * 0.1f;
            stack.Yaw = (float)(MathLibrary.GlobalRandom.NextDouble() * Math.PI);
            stack.Pitch = (float)(MathLibrary.GlobalRandom.NextDouble() * Math.PI);
            stack.Velocity.X += (float)0.05f * (float)(MathLibrary.GlobalRandom.NextDouble() * 2.0 - 1.0);
            stack.Velocity.Z += (float)0.05f * (float)(MathLibrary.GlobalRandom.NextDouble() * 2.0 - 1.0);
            stack.Velocity.Y = 0.2f + (float)0.05f * (float)(MathLibrary.GlobalRandom.NextDouble() * 2.0 - 1.0);
            World.Instance.SpawnStack(stack);
            throwStackDelay = 20;
        }

        internal void GiveHealth(float percentPoints)
        {
            Health += percentPoints;
            if (Health > 100f) 
                Health = 100f;
            if (Health < 0)
                Health = 0f;
        }

        internal void DecreaseHunger(float percentPoints)
        {
            Hunger -= percentPoints;
            if (Hunger > 100f)
                Hunger = 100f;
            if (Hunger < 0f)
                Hunger = 0;
        }
    }
}
