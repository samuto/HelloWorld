using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using WindowsFormsApplication7.Frontend;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Profiling;
using SlimDX.Windows;
using SlimDX.Multimedia;
using SlimDX;
using SlimDX.DirectInput;
using WindowsFormsApplication7.Frontend.Gui;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7
{
    class TheGame
    {
        public enum GameMode
        {
            Gui,
            InGame
        }
        public static TheGame Instance = new TheGame();
        public Entity entityToControl;
        public int CurrentTick = 0;
        public GameMode Mode = GameMode.InGame;
        public GuiForm ActiveGui = null;
        public GuiCursor Cursor = new GuiCursor();
        private RenderForm form;
        private double debugUpdateTime;
        private int fpsCounter;
        private bool shutdown = false;
        private Stopwatch sw = new Stopwatch();

        public void Run()
        {
            this.Initialize();
            form.DesktopLocation = new Point(100, 100);

            MessagePump.Run(form, () =>
            {
                this.RunGameLoop();
                if (shutdown)
                    form.Close();
            });

            this.Dispose();
        }

        private void Initialize()
        {
            sw.Start();
            debugUpdateTime = GetTime();

            form = new RenderForm("Hello World");
            float scale = 4f;
            form.Width = (int)(200f * scale);
            form.Height = (int)(150f * scale);

            GlobalRenderer.Instance.Initialize(form);
            World.Instance.Player.PrevPosition = World.Instance.Player.Position = new Vector3(-30, 64, 0);
            World.Instance.FlyingCamera.Position = new Vector3(-30, 70, -25);
            entityToControl = World.Instance.Player;
            Camera.Instance.AttachTo(entityToControl);
            Input.Instance.Initialize();
        }


        private void Dispose()
        {
            Input.Instance.Dispose();
            Tessellator.Instance.Dispose();
            GlobalRenderer.Instance.Dispose();
        }

        private void RunGameLoop()
        {
            Frame.Next();
            Profiler p = Profiler.Instance;
            p.Clear();

            // update
            p.StartSection("update");
            while (Frame.HasMoreTicks())
            {
                RunTick();
                Frame.Tick();
            }

            float partialStep = Frame.GetPartialStep();
            float mousedx = -Input.Instance.InterpolatedMouseDeltaX(partialStep) * GameSettings.MouseSensitivity * 0.001f;
            float mousedy = Input.Instance.InterpolatedMouseDeltaY(partialStep) * GameSettings.MouseSensitivity * 0.001f;
            if (Mode == GameMode.InGame)
            {
                entityToControl.SetViewAngles(mousedx, mousedy);
            }

            // render game
            p.EndStartSection("render");
            GlobalRenderer.Instance.ClearTarget();
            //GlobalRenderer.Instance.Render(partialStep);
            p.EndSection();

            p.EndSection(); // end root-section

            // render profiler info
            if (Profiler.Instance.Enabled)
            {
                GlobalRenderer.Instance.RenderProfiler();
            }

            if (ActiveGui != null)
            {
                GlobalRenderer.Instance.RenderGui(partialStep);
            }

            // commit the graphics (swap buffer)
            GlobalRenderer.Instance.Commit();

            // display debuginfo
            while (GetTime() >= this.debugUpdateTime + 1d)
            {
                form.Text = this.fpsCounter + " fps (" + CurrentTick + ")";
                this.debugUpdateTime += 1d;
                this.fpsCounter = 0;
            }

            // update frame counter
            this.fpsCounter++;
        }

        private double GetTime()
        {
            return sw.ElapsedTicks / Stopwatch.Frequency;
        }

        private void RunTick()
        {
            // get input
            Input.Instance.Update();

            // handle input
            KeyboardState prevKeyboardState = Input.Instance.CurrentInput.KeyboardState;
            KeyboardState keyboardState = Input.Instance.LastInput.KeyboardState;
            MouseState mouseState = Input.Instance.CurrentInput.MouseState;
            if (prevKeyboardState.IsPressed(Key.E) && Mode == GameMode.InGame)
            {
                // Launch crafting gui
                OpenGui(new GuiCraftingForm());
            }
            else if (prevKeyboardState.IsPressed(Key.D1))
            {
                World.Instance.Player.SelectedBlockId = BlockRepository.Grass.Id;
            }
            else if (prevKeyboardState.IsPressed(Key.D1))
            {
                World.Instance.Player.SelectedBlockId = BlockRepository.Dirt.Id;
            }
            else if (prevKeyboardState.IsPressed(Key.D2))
            {
                World.Instance.Player.SelectedBlockId = BlockRepository.Stone.Id;
            }
            else if (prevKeyboardState.IsPressed(Key.D3))
            {
                World.Instance.Player.SelectedBlockId = BlockRepository.Brick.Id;
            }
            else if (prevKeyboardState.IsPressed(Key.D4))
            {
                World.Instance.Player.SelectedBlockId = BlockRepository.BedRock.Id;
            }
            else if (prevKeyboardState.IsPressed(Key.D5))
            {
                World.Instance.Player.SelectedBlockId = BlockRepository.Leaf.Id;
            }
            else if (prevKeyboardState.IsPressed(Key.D6))
            {
                World.Instance.Player.SelectedBlockId = BlockRepository.Wood.Id;
            }
            else if (prevKeyboardState.IsPressed(Key.D7))
            {
                World.Instance.Player.SelectedBlockId = BlockRepository.Diamond.Id;
            }
            else if (prevKeyboardState.IsPressed(Key.Comma))
            {
                entityToControl = World.Instance.FlyingCamera;
                Camera.Instance.AttachTo(entityToControl);
            }
            else if (prevKeyboardState.IsPressed(Key.Period))
            {
                entityToControl = World.Instance.Player;
                Camera.Instance.AttachTo(entityToControl);
            }
            if (prevKeyboardState.IsPressed(Key.W))
            {
                entityToControl.MoveForward();
            }
            if (prevKeyboardState.IsPressed(Key.S))
            {
                entityToControl.MoveBackward();
            }
            if (prevKeyboardState.IsPressed(Key.A))
            {
                entityToControl.MoveLeft();
            }
            if (prevKeyboardState.IsPressed(Key.D))
            {
                entityToControl.MoveRight();
            }
            if (prevKeyboardState.IsPressed(Key.F))
            {
                entityToControl.MoveDown();
            }
            if (prevKeyboardState.IsPressed(Key.R))
            {
                entityToControl.MoveUp();
            }
            if (prevKeyboardState.IsPressed(Key.Space))
            {
                entityToControl.Jump();
            }
            if (prevKeyboardState.IsPressed(Key.Escape) && Mode == GameMode.Gui)
            {
                CloseGui();
            }
            else if (prevKeyboardState.IsReleased(Key.V) && keyboardState.IsPressed(Key.V))
            {
                Profiler.Instance.ToggleReport();
            }
            else if (prevKeyboardState.IsReleased(Key.C) && keyboardState.IsPressed(Key.C))
            {
                Profiler.Instance.ToggleEnabled();
            }
            else if (prevKeyboardState.IsReleased(Key.Z) && keyboardState.IsPressed(Key.Z))
            {
                Profiler.Instance.ToggleMarkedSection();
            }
            else if (prevKeyboardState.IsReleased(Key.X) && keyboardState.IsPressed(Key.X))
            {
                Profiler.Instance.SelectedSection = Profiler.Instance.MarkedSection;
            }
            else if (prevKeyboardState.IsReleased(Key.Backspace) && keyboardState.IsPressed(Key.Backspace))
            {
                int index = Profiler.Instance.SelectedSection.LastIndexOf(".");
                if (index >= 0)
                    Profiler.Instance.SelectedSection = Profiler.Instance.SelectedSection.Substring(0, index);
            }

            if (ActiveGui != null)
            {
                ActiveGui.Update();
            }

            // handle mouse
            if (World.Instance.PlayerVoxelTrace.Hit && !Input.Instance.IsMouseFreezed())
            {
                if (mouseState.IsPressed(1))
                {
                    Input.Instance.FreezeMouse();
                    Vector4 pos = World.Instance.PlayerVoxelTrace.ImpactPosition;
                    PositionBlock posBlock = new PositionBlock((int)pos.X, (int)pos.Y, (int)pos.Z);

                    World.Instance.SetBlock(posBlock.X, posBlock.Y, posBlock.Z, World.Instance.Player.SelectedBlockId);
                    PositionChunk posChunk = PositionChunk.CreateFrom(posBlock);
                    Chunk chunk = World.Instance.GetChunk(posChunk);
                    chunk.InvalidateMeAndNeighbors();

                }
                else if (mouseState.IsPressed(0))
                {
                    Input.Instance.FreezeMouse();
                    Vector4 pos = World.Instance.PlayerVoxelTrace.BuildPosition;
                    PositionBlock posBlock = new PositionBlock((int)pos.X, (int)pos.Y, (int)pos.Z);
                    World.Instance.SetBlock(posBlock.X, posBlock.Y, posBlock.Z, 0);
                    PositionChunk posChunk = PositionChunk.CreateFrom(posBlock);
                    Chunk chunk = World.Instance.GetChunk(posChunk);
                    chunk.InvalidateMeAndNeighbors();
                }
            }

            // update world
            World.Instance.Update();

            // reset mouse
            System.Windows.Forms.Cursor.Position = form.PointToScreen(new Point(Width / 2, Height / 2));
            System.Windows.Forms.Cursor.Hide();

            // update global game clock
            CurrentTick++;
        }

        private void CloseGui()
        {
            // Close gui
            ActiveGui.Close();
            // Restore parent gui
            ActiveGui = (GuiForm) ActiveGui.Parent;
            Mode = ActiveGui == null ? GameMode.InGame : GameMode.Gui;
        }

        private void OpenGui(GuiForm gui)
        {
            // Save parent
            gui.Parent = ActiveGui;
            // Open new gui
            ActiveGui = gui;
            gui.Show();
            Mode = GameMode.Gui;
        }

        public int Width
        {
            get
            {
                return form.ClientSize.Width;
            }
        }

        public int Height
        {
            get
            {
                return form.ClientSize.Height;
            }
        }
    }
}
