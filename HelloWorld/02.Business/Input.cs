using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.RawInput;
using SlimDX.DirectInput;
using WindowsFormsApplication7.CrossCutting.Entities;

namespace WindowsFormsApplication7.Business
{
    class Input
    {
        public static Input Instance = new Input();
        
        private static Keyboard keyboard;
        private static DirectInput directInput;
        private static Mouse mouse;
        private TickInput lastInput;

        static Input()
        {
            directInput = new DirectInput();
            keyboard = new Keyboard(directInput);
            mouse = new Mouse(directInput);
        }

        internal TickInput Capture()
        {
            // read keyboard state
            keyboard.Acquire();
            KeyboardState ks = keyboard.GetCurrentState();
            TickInput tickInput = new TickInput();
            tickInput.KeyboardStateNow = ks;
            if (lastInput == null)
            {
                lastInput = tickInput;
            }
            // set old state...
            tickInput.KeyboardStateLast = lastInput.KeyboardStateNow;
            // save old state for next capture
            lastInput = tickInput;
            return tickInput;
        }

        internal MouseState GetMouseState()
        {
            mouse.Acquire();
            MouseState ms = mouse.GetCurrentState();
            return ms;
        }

        internal static void Dispose()
        {
            mouse.Dispose();
            keyboard.Dispose();
            directInput.Dispose();

        }
    }
}
