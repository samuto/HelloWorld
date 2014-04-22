using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7._01.Frontend.Gui.Controls;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting;

namespace WindowsFormsApplication7.Frontend.Gui.Controls
{
    class GuiLabel : GuiControl
    {
        private Label label = new Label();
        public bool Center = false;

        public GuiLabel()
        {
            Color = new Vector4(1, 1, 1, 1);
            CustomRendering = true;
            this.RenderControl += new EventHandler<EventArgs>(GuiLabel_RenderControl);
        }

        void GuiLabel_RenderControl(object sender, EventArgs e)
        {
            label.Color = Color;
            label.Text = text;
            Vector3 ofs = new Vector3();
            if(Center) 
                ofs = new Vector3((Parent.Size - label.f.TextSize(text)) / 2f, 0);
            label.Position = new Vector3(GlobalLocation, 0)+ofs;
            label.Render();
        }

        public override void Dispose()
        {
            base.Dispose();
            label.Dispose();
            label = null;
        }
    }
}