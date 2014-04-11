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

        public GuiLabel()
        {
            CustomRendering = true;
            this.RenderControl += new EventHandler<EventArgs>(GuiLabel_RenderControl);
        }

        void GuiLabel_RenderControl(object sender, EventArgs e)
        {
            label.Text = text;
            bool test = Parent.Parent.Parent == null;
            label.Position = new Vector3(GlobalLocation, 0);
            
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