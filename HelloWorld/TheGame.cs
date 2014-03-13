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

namespace WindowsFormsApplication7
{
    class TheGame
    {
        public static TheGame Instance = new TheGame();
        public Entity entityToControl;
        public int CurrentTick = 0;

        private RenderForm form;
        private double debugUpdateTime;
        private int fpsCounter;
        private bool shutdown = false;
        private Stopwatch sw = new Stopwatch();

        public void Run()
        {
            this.Initialize();

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

            form = new RenderForm("SlimDX - MiniTri Direct3D 11 Sample");
            int scale = 2;
            form.Width = 200 * scale;
            form.Height = 150 * scale;

            GlobalRenderer.Instance.Initialize(form);
            World.Instance.Player.PrevPosition = World.Instance.Player.Position = new Vector3(-30, 64, 0);
            World.Instance.FlyingCamera.Position = new Vector3(-30, 35, -25);
            Camera.Instance.AttachTo(World.Instance.FlyingCamera);
            entityToControl = World.Instance.FlyingCamera;
        }


        private void Dispose()
        {
            Input.Dispose();
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
            
            // get mouse
            p.EndStartSection("getmouse");
            MouseState ms = Input.Instance.GetMouseState();
            float mousedx = -ms.X * 0.002f;
            float mousedy = ms.Y * 0.002f;
            entityToControl.SetViewAngles(mousedx, mousedy);

            // render game
            p.EndStartSection("render");
            GlobalRenderer.Instance.Render(Frame.GetPartialStep());
            p.EndSection();

            p.EndSection(); // end root-section

            // render profiler info
            if (Profiler.Instance.Enabled)
            {
                GlobalRenderer.Instance.RenderProfiler();
            }

            // commit the graphics (swap buffer)
            GlobalRenderer.Instance.Commit();

            // display debuginfo
            while (GetTime() >= this.debugUpdateTime + 1d)
            {
                form.Text = this.fpsCounter + " fps ("+CurrentTick+")";
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
            TickInput input = Input.Instance.Capture();

            // handle input
            KeyboardState keyboardNow = input.KeyboardStateNow;
            KeyboardState keyboardLast = input.KeyboardStateLast;
            if (keyboardNow.IsPressed(Key.D1))
            {
                entityToControl = World.Instance.Player;
                Camera.Instance.AttachTo(entityToControl);
            }
            else if (keyboardNow.IsPressed(Key.D2))
            {
                entityToControl = World.Instance.FlyingCamera;
                Camera.Instance.AttachTo(entityToControl);
            }
            if (keyboardNow.IsPressed(Key.W))
            {
                entityToControl.MoveForward();
            }
            if (keyboardNow.IsPressed(Key.E))
            {
                GameSettings.EnableEntityUpdate = !GameSettings.EnableEntityUpdate;
            }
            if (keyboardNow.IsPressed(Key.S))
            {
                entityToControl.MoveBackward();
            }
            if (keyboardNow.IsPressed(Key.A))
            {
                entityToControl.MoveLeft();
            }
            if (keyboardNow.IsPressed(Key.D))
            {
                entityToControl.MoveRight();
            }
            if (keyboardNow.IsPressed(Key.F))
            {
                entityToControl.MoveDown();
            }
            if (keyboardNow.IsPressed(Key.R))
            {
                entityToControl.MoveUp();
            }
            if (keyboardNow.IsPressed(Key.Space))
            {
                entityToControl.Jump();
            }
            if (keyboardNow.IsPressed(Key.Escape))
            {
                shutdown = true;
            }
            else if (keyboardNow.IsReleased(Key.V) && keyboardLast.IsPressed(Key.V))
            {
                Profiler.Instance.ToggleReport();
            }
            else if (keyboardNow.IsReleased(Key.C) && keyboardLast.IsPressed(Key.C))
            {
                Profiler.Instance.ToggleEnabled();
            }
            else if (keyboardNow.IsReleased(Key.Z) && keyboardLast.IsPressed(Key.Z))
            {
                Profiler.Instance.ToggleMarkedSection();
            }
            else if (keyboardNow.IsReleased(Key.X) && keyboardLast.IsPressed(Key.X))
            {
                Profiler.Instance.SelectedSection = Profiler.Instance.MarkedSection;
            }
            else if (keyboardNow.IsReleased(Key.Backspace) && keyboardLast.IsPressed(Key.Backspace))
            {
                int index = Profiler.Instance.SelectedSection.LastIndexOf(".");
                if(index>=0)
                    Profiler.Instance.SelectedSection = Profiler.Instance.SelectedSection.Substring(0, index);
            }

            // update world
            World.Instance.Update();

            // reset mouse
            System.Windows.Forms.Cursor.Position = form.PointToScreen(new Point(Width / 2, Height / 2));
            System.Windows.Forms.Cursor.Hide();

            // update global game clock
            CurrentTick++;
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
