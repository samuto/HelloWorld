using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.DirectInput;
using SlimDX;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    struct FrameInput
    {
        public KeyboardState KeyboardState;
        public MouseState MouseState;
        public Vector2 MouseLocation;
    }
}
