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
using WindowsFormsApplication7.CrossCutting;
using WindowsFormsApplication7.Frontend.Gui.Forms;

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
        public EntityPlayer entityToControl;
        public int CurrentTick = 0;
        public GameMode Mode = GameMode.InGame;
        public GuiForm ActiveGui = null;
        public GuiCursor Cursor = new GuiCursor();
        private RenderForm form;
        private double debugUpdateTime;
        private int fpsCounter;
        private bool shutdown = false;
        private Stopwatch sw = new Stopwatch();
        private Profiler p = Profiler.Instance;
        private bool showProfiler = true;
        public void Run()
        {
            this.Initialize();
            form.DesktopLocation = new Point(10, 10);
           
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
            float scale = 3f;
            form.ClientSize = new Size((int)(320f * scale), (int)(240f * scale));

            GlobalRenderer.Instance.Initialize(form);
            World.Instance.Player.PrevPosition = World.Instance.Player.Position = new Vector3(0, 66, -20);
            World.Instance.FlyingCamera.Position = new Vector3(10, 70, -25);
            entityToControl = World.Instance.Player;
            Camera.Instance.AttachTo(entityToControl);
            Input.Instance.Initialize();
        }

        private void Dispose()
        {
            Input.Instance.Dispose();
            Tessellator.Instance.Dispose();
            FontRenderer.Instance.Dispose();
            GlobalRenderer.Instance.Dispose();
        }

        private void RunGameLoop()
        {
            Frame.Next();
            p.StartSection("root");
            // update
            p.StartSection("update");
            while (Frame.HasMoreTicks())
            {
                Update();
                Frame.Tick();
            }
            float partialStep = Frame.GetPartialStep();

            // render game
            p.EndStartSection("render");
            GlobalRenderer.Instance.ClearTarget();
            GlobalRenderer.Instance.Render(partialStep);
           
            // render 2d gui
            p.EndStartSection("2dgui");
            if (ActiveGui != null)
            {
                GlobalRenderer.Instance.RenderGui(partialStep);
            }
            p.EndSection();

            p.EndSection(); // end root-section

            // render profiler info
            if (showProfiler)
            {
                GlobalRenderer.Instance.RenderProfiler();
            }
            p.Enabled = showProfiler;

            // commit the graphics (swap buffer)
            GlobalRenderer.Instance.Commit();

            // display debuginfo in widows caption
            
            while (GetTime() >= this.debugUpdateTime + 1d)
            {
                form.Text = this.fpsCounter + " fps (" + CurrentTick + ")";
                this.debugUpdateTime += 1d;
                this.fpsCounter = 0;
                GlobalRenderer.Instance.ProfilerSnapshot();
                p.Clear();
            }

            // update frame counter
            this.fpsCounter++;
        }

        private double GetTime()
        {
            return sw.ElapsedTicks / Stopwatch.Frequency;
        }

        private void Update()
        {
            Log.Instance.Update();
            // get input
            Input.Instance.Update();
            // gui scaling
            GuiScaling.Instance.Update();

            // handle input
            KeyboardState prevKeyboardState = Input.Instance.CurrentInput.KeyboardState;
            KeyboardState keyboardState = Input.Instance.LastInput.KeyboardState;
            MouseState mouseState = Input.Instance.CurrentInput.MouseState;

            if (prevKeyboardState.IsPressed(Key.Comma))
            {
                entityToControl = World.Instance.FlyingCamera;
                Camera.Instance.AttachTo(entityToControl);
            }
            else if (prevKeyboardState.IsPressed(Key.Period))
            {
                entityToControl = World.Instance.Player;
                Camera.Instance.AttachTo(entityToControl);
            }

            if (prevKeyboardState.IsPressed(Key.F11))
            {
                GlobalRenderer.Instance.ToggleFullScreen();
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
                p.ToggleReport();
            }
            else if (prevKeyboardState.IsReleased(Key.C) && keyboardState.IsPressed(Key.C))
            {
                showProfiler = !showProfiler;
            }
            else if (prevKeyboardState.IsReleased(Key.Z) && keyboardState.IsPressed(Key.Z))
            {
                p.ToggleMarkedSection();
            }
            else if (prevKeyboardState.IsReleased(Key.X) && keyboardState.IsPressed(Key.X))
            {
                p.SelectedSection = p.MarkedSection;
            }
            else if (prevKeyboardState.IsReleased(Key.Backspace) && keyboardState.IsPressed(Key.Backspace))
            {
                int index = p.SelectedSection.LastIndexOf(".");
                if (index >= 0)
                    p.SelectedSection = p.SelectedSection.Substring(0, index);
            }

            p.StartSection("2dgui");
            if (ActiveGui != null)
            {
                ActiveGui.Update();
            }
            p.EndSection();

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
            ActiveGui.OnClose();
            // Restore parent gui
            ActiveGui = (GuiForm)ActiveGui.Parent;
            Mode = ActiveGui == null ? GameMode.InGame : GameMode.Gui;
        }

        internal void OpenGui(GuiForm gui)
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

        internal bool IsEntityControlled(EntityPlayer entity)
        {
            return entityToControl == entity;
        }
    }
}
