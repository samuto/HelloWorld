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
        private GuiLabel label;

        public GuiButton()
        {
            Color = ColorDeactivated;
            CustomRendering = true;
            this.RenderControl += new EventHandler<EventArgs>(GuiButton_RenderForm);
            label = new GuiLabel();
            label.Center = true;
            AddControl(label);
        }

        protected override void OnTextChanged(string text)
        {
            label.Text = text;
        }

        void GuiButton_RenderForm(object sender, EventArgs e)
        {
            t.StartDrawingColoredQuads();
            Vector4 black = new Vector4(0, 0, 0, 1f);
            float ofs = FontRenderer.Instance.CharScale;
            t.StartDrawingColoredQuads();
            if (!Pressed)
            {
                label.Location = new Vector2(0,0);
                t.AddVertexWithColor(new Vector4(GlobalLocation.X + ofs, GlobalLocation.Y - ofs, 0f, 1f), black);
                t.AddVertexWithColor(new Vector4(GlobalLocation.X + ofs, GlobalLocation.Y + Size.Y - ofs, 0f, 1f), black);
                t.AddVertexWithColor(new Vector4(GlobalLocation.X + Size.X + ofs, GlobalLocation.Y + Size.Y - ofs, 0f, 1f), black);
                t.AddVertexWithColor(new Vector4(GlobalLocation.X + Size.X + ofs, GlobalLocation.Y - ofs, 0f, 1f), black);
            }
            else
            {
                label.Location = new Vector2(1,-1);
            }

            float ofs2 = Pressed ? ofs : 0;
            t.AddVertexWithColor(new Vector4(GlobalLocation.X + ofs2, GlobalLocation.Y - ofs2, 0f, 1f), Color);
            t.AddVertexWithColor(new Vector4(GlobalLocation.X + ofs2, GlobalLocation.Y + Size.Y - ofs2, 0f, 1f), Color);
            t.AddVertexWithColor(new Vector4(GlobalLocation.X + Size.X + ofs2, GlobalLocation.Y + Size.Y - ofs2, 0f, 1f), Color);
            t.AddVertexWithColor(new Vector4(GlobalLocation.X + Size.X + ofs2, GlobalLocation.Y - ofs2, 0f, 1f), Color);
            t.Draw();
        }

        internal override void OnMouseEnter()
        {
            Color = ColorActivated;
        }

        internal override void OnMouseDown()
        {
            Pressed = true;
            base.OnMouseDown();
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
