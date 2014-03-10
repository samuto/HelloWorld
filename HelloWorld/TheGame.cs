using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Frontend;
using SlimDX.Windows;
using WindowsFormsApplication7.Business;
using System.Drawing;
using SlimDX.RawInput;
using SlimDX.Multimedia;
using WindowsFormsApplication7.CrossCutting.Entities;
using SlimDX.DirectInput;
using System.IO;
using WindowsFormsApplication7.Business.Profiling;
using SlimDX;

namespace WindowsFormsApplication7
{
    class TheGame
    {
        public static TheGame Instance = new TheGame();

        private Timer timer;
        private RenderForm form;

        private long debugUpdateTime = Timer.getSystemTime();
        private int fpsCounter;

        private Keyboard keyboard;
        private DirectInput directInput;
        private Mouse mouse;
        private bool shutdown = false;

        public Entity entityToControl;


        public void Run()
        {
            this.StartGame();


            MessagePump.Run(form, () =>
            {
                this.RunGameLoop();
                if (shutdown)
                    form.Close();
            });

            this.EndGame();
        }



        private void EndGame()
        {
            mouse.Dispose();
            keyboard.Dispose();
            directInput.Dispose();
            Tessellator.Instance.Dispose();
            GlobalRenderer.Instance.Dispose();
        }

        private void RunGameLoop()
        {
            timer.updateTimer();
            Profiler p = Profiler.Instance;
            p.Clear();

            p.StartSection("update");
            // update
            for (int i = 0; i < timer.ElapsedTicks; i++)
            {
                RunTick();
            }
            p.EndStartSection("getmouse");
            // get mouse
            mouse.Acquire();
            MouseState ms = mouse.GetCurrentState();
            float mousedx = -ms.X * 0.002f;
            float mousedy = ms.Y * 0.002f;
            entityToControl.SetViewAngles(mousedx, mousedy);

            // render game
            p.EndStartSection("render");
            GlobalRenderer.Instance.Render(timer.RenderPartialTicks);
            p.EndSection();

            p.EndSection(); // Root

            if(Profiler.Instance.Enabled)
                GlobalRenderer.Instance.RenderProfiler();

            GlobalRenderer.Instance.Commit();

            // display debuginfo
            while (Timer.getSystemTime() >= this.debugUpdateTime + 1000L)
            {
                form.Text = this.fpsCounter + " fps";
                this.debugUpdateTime += 1000L;
                this.fpsCounter = 0;
            }


            this.fpsCounter++;
        }

        KeyboardState lastKeyboardState = new KeyboardState();

        private void RunTick()
        {
            
            keyboard.Acquire();
            KeyboardState ks = keyboard.GetCurrentState();

            if (ks.IsPressed(Key.D1))
            {
                entityToControl = World.Instance.Player;
                Camera.Instance.AttachTo(entityToControl);
            }
            else if (ks.IsPressed(Key.D2))
            {
                entityToControl = World.Instance.FlyingCamera;
                Camera.Instance.AttachTo(entityToControl);
            }
            if (ks.IsPressed(Key.W))
            {
                entityToControl.MoveForward();
            }
            if (ks.IsPressed(Key.E))
            {
                GameSettings.EnableEntityUpdate = !GameSettings.EnableEntityUpdate;
            }
            if (ks.IsPressed(Key.S))
            {
                entityToControl.MoveBackward();
            }
            if (ks.IsPressed(Key.A))
            {
                entityToControl.MoveLeft();
            }
            if (ks.IsPressed(Key.D))
            {
                entityToControl.MoveRight();
            }
            if (ks.IsPressed(Key.F))
            {
                entityToControl.MoveDown();
            }
            if (ks.IsPressed(Key.R))
            {
                entityToControl.MoveUp();
            }
            if (ks.IsPressed(Key.Space))
            {
                entityToControl.Jump();
            }
            if (ks.IsPressed(Key.Escape))
            {
                shutdown = true;
            }
            else if (ks.IsReleased(Key.V) && lastKeyboardState.IsPressed(Key.V))
            {
                Profiler.Instance.ToggleReport();
            }
            else if (ks.IsReleased(Key.C) && lastKeyboardState.IsPressed(Key.C))
            {
                Profiler.Instance.ToggleEnabled();
            }
            else if (ks.IsReleased(Key.Z) && lastKeyboardState.IsPressed(Key.Z))
            {
                Profiler.Instance.ToggleMarkedSection();
            }
            else if (ks.IsReleased(Key.X) && lastKeyboardState.IsPressed(Key.X))
            {
                Profiler.Instance.SelectedSection = Profiler.Instance.MarkedSection;
            }
            else if (ks.IsReleased(Key.Backspace) && lastKeyboardState.IsPressed(Key.Backspace))
            {

                int index = Profiler.Instance.SelectedSection.LastIndexOf(".");
                if(index>=0)
                    Profiler.Instance.SelectedSection = Profiler.Instance.SelectedSection.Substring(0, index);
            }
            lastKeyboardState = ks;
            World.Instance.Update();

            System.Windows.Forms.Cursor.Position = form.PointToScreen(new Point(Width / 2, Height / 2));
            System.Windows.Forms.Cursor.Hide();
            

        }

        private void StartGame()
        {
            directInput = new DirectInput();
            keyboard = new Keyboard(directInput);
            mouse = new Mouse(directInput);

            timer = new Timer(20.0F);
            form = new RenderForm("SlimDX - MiniTri Direct3D 11 Sample");
            int scale = 1;
            form.Width = 200 * scale;
            form.Height = 150 * scale;

            GlobalRenderer.Instance.Initialize(form);
            World.Instance.Player.Position = new Vector3(-30, 64, 0);
            World.Instance.FlyingCamera.Position = new Vector3(-30, 35, -25);
            Camera.Instance.AttachTo(World.Instance.FlyingCamera);
            entityToControl = World.Instance.FlyingCamera;
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
