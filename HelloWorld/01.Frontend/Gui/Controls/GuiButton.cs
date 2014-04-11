using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace WindowsFormsApplication7.Frontend.Gui.Controls
{
    class GuiButton : GuiControl
    {
        private bool Pressed = false;
        private Vector4 ColorActivated = new Vector4(0.5f, 0.5f, 1, 1);
        private Vector4 ColorDeactivated = new Vector4(0.3f, 0.3f, 0.8f, 1);

        public GuiButton()
        {
            Color = ColorDeactivated;
            CustomRendering = true;
            this.RenderControl += new EventHandler<EventArgs>(GuiButton_RenderForm);
        }

        void GuiButton_RenderForm(object sender, EventArgs e)
        {
            t.StartDrawingColoredQuads();
            Vector4 black = new Vector4(0, 0, 0, 1f);
            float ofs = FontRenderer.Instance.CharScale;
            if (!Pressed)
            {
                t.AddVertexWithColor(new Vector4(Location.X + ofs, Location.Y - ofs, 0f, 1f), black);
                t.AddVertexWithColor(new Vector4(Location.X + ofs, Location.Y + Size.Y - ofs, 0f, 1f), black);
                t.AddVertexWithColor(new Vector4(Location.X + Size.X + ofs, Location.Y + Size.Y - ofs, 0f, 1f), black);
                t.AddVertexWithColor(new Vector4(Location.X + Size.X + ofs, Location.Y - ofs, 0f, 1f), black);
            }
            float ofs2 = Pressed ? ofs : 0;
            t.AddVertexWithColor(new Vector4(Location.X + ofs2, Location.Y - ofs2, 0f, 1f), Color);
            t.AddVertexWithColor(new Vector4(Location.X + ofs2, Location.Y + Size.Y - ofs2, 0f, 1f), Color);
            t.AddVertexWithColor(new Vector4(Location.X + Size.X + ofs2, Location.Y + Size.Y - ofs2, 0f, 1f), Color);
            t.AddVertexWithColor(new Vector4(Location.X + Size.X + ofs2, Location.Y - ofs2, 0f, 1f), Color);
            
        }

        internal override void OnMouseEnter()
        {
            Color = ColorActivated;
        }

        internal override void OnMouseDown()
        {
            Pressed = true;
        }

        internal override void OnMouseUp()
        {
            Pressed = false;
        }

        internal override void OnMouseLeave()
        {
            Pressed = false;
            Color = ColorDeactivated;
        }

    }
}
