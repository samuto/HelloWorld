using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.Frontend.Gui.Controls;

namespace WindowsFormsApplication7.Frontend.Gui.Forms
{
    class GuiTestForm : GuiForm
    {
        GuiLabel label = new GuiLabel();
        GuiPanel panel = new GuiPanel();

        public GuiTestForm()
        {
            panel.Text = "p1";
            panel.Location = new SlimDX.Vector2(10, 10);
            panel.Size = new SlimDX.Vector2(10, 10);
            AddControl(panel);

            label.Text = "SIMON";
            label.Location = new SlimDX.Vector2(0, 0);
            AddControl(label);

            
        }

       
    }
}
