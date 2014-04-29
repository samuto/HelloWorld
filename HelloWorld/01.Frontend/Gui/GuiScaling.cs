using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace WindowsFormsApplication7.Frontend.Gui
{
    class GuiScaling
    {
        public static GuiScaling Instance = new GuiScaling();
        public Vector3 Scale3 = new Vector3();
        public Vector3 Translate3 = new Vector3();
        public float Scale;
        public Vector2 Translate2 = new Vector2();
        public const float Width = 9 * 16;
        public const float Height = 9 * 16;
        public const float ItemSize = 16;

        internal void Update()
        {
            // set scale
            float scaleX = (TheGame.Instance.Width / Width);
            float scaleY = (TheGame.Instance.Height / Height);
            Scale = Math.Min(scaleX, scaleY);
            if (Scale < 1f)
                Scale = 1f;
            Scale3.X = Scale;
            Scale3.Y = Scale;
            Scale3.Z = Scale;
            
            // set translate
            float translateX = (TheGame.Instance.Width - Width * Scale) / 2f;
            float translateY = (TheGame.Instance.Height - Height * Scale) / 2f;

            Translate2.X = Translate3.X = translateX;
            Translate2.Y = Translate3.Y = translateY;
            Translate3.Z = 0;
            
        }

        internal Vector2 CalcMouseLocation(Vector2 location)
        {
            return (location - Translate2) / Scale;
        }
    }
}
