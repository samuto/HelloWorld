using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.RawInput;
using SlimDX.DirectInput;
using WindowsFormsApplication7.CrossCutting.Entities;
using SlimDX;
using WindowsFormsApplication7._01.Frontend;
using WindowsFormsApplication7.Frontend;

namespace WindowsFormsApplication7.Business
{
    class Input
    {
        public static Input Instance = new Input();

        private DirectInput directInput;
        private Keyboard keyboard;
        private Mouse mouse;
        private Vector2 MouseLocation = new Vector2();

        public FrameInput LastInput;
        public FrameInput CurrentInput;

        public Input()
        {
            directInput = new DirectInput();
            keyboard = new Keyboard(directInput);
            mouse = new Mouse(directInput);
        }

        internal void Update()
        {
            // save old state
            LastInput = CurrentInput;
            // read keyboard state
            keyboard.Acquire();
            mouse.Acquire();
            KeyboardState ks = keyboard.GetCurrentState();
            MouseState ms = mouse.GetCurrentState();
            FrameInput frameInput = new FrameInput();
            frameInput.KeyboardState = ks;
            frameInput.MouseState = ms;
            MouseLocation.X += ms.X * GameSettings.MouseSensitivity;
            MouseLocation.Y += -ms.Y * GameSettings.MouseSensitivity;
            // Cap mouse location
            if (MouseLocation.X < 0)
                MouseLocation.X = 0;
            if (MouseLocation.Y < 0)
                MouseLocation.Y = 0;
            if (MouseLocation.Y > TheGame.Instance.Height)
                MouseLocation.Y = TheGame.Instance.Height - 1;
            if (MouseLocation.X > TheGame.Instance.Width)
                MouseLocation.X = TheGame.Instance.Width - 1;
            frameInput.MouseLocation = MouseLocation;
            CurrentInput = frameInput;
        }

        internal void Dispose()
        {
            mouse.Dispose();
            keyboard.Dispose();
            directInput.Dispose();
        }

        internal Vector2 InterpolatedMouseLocation(float partialStep)
        {
            Vector2 vector = Interpolate.Vector(CurrentInput.MouseLocation, LastInput.MouseLocation, partialStep);
            return vector;
        }

        internal int InterpolatedMouseDeltaX(float partialStep)
        {
            return (int)Interpolate.Scalar(CurrentInput.MouseState.X, LastInput.MouseState.X, partialStep);
        }

        internal int InterpolatedMouseDeltaY(float partialStep)
        {
            return (int)Interpolate.Scalar(CurrentInput.MouseState.Y, LastInput.MouseState.Y, partialStep);
        }

        internal void Initialize()
        {
            Update();
            LastInput = CurrentInput;
        }
    }
}
