using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace WindowsFormsApplication7.Frontend.Gui
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
            this.OnRender += new EventHandler<EventArgs>(GuiButton_OnRender);
        }

        void GuiButton_OnRender(object sender, EventArgs e)
        {
            Tessellator t = Tessellator.Instance;
            t.StartDrawingColoredQuads();
            Vector4 black = new Vector4(0, 0, 0, 1f);
            float ofs = FontRenderer.CharScale;
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
            FontRenderer f = FontRenderer.Instance;
            t.Draw();
            t.StartDrawingAlphaTexturedQuads("ascii");
            Vector2 textSize = f.TextSize(Text);
            f.RenderTextShadow(Text, Location.X + (Size.X - textSize.X) / 2f + ofs2 + ParentLocation.X, Location.Y + (Size.Y - textSize.Y) / 2f - ofs2 + ParentLocation.Y);
            t.Draw();
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
