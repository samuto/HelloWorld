using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.Frontend.Gui.Controls;
using SlimDX;

namespace WindowsFormsApplication7.Frontend.Gui.Forms
{
    class GuiTestForm : GuiForm
    {
        GuiLabel label = new GuiLabel();
        GuiPanel panel = new GuiPanel();

        public GuiTestForm()
        {
            for (int i = 0; i < 9; i++)
            {
                Vector4 color = new Vector4(1, 1, 1, 1);
                if (i == 0 || i == 8)
                    color = new Vector4(1, 0, 0, 1);
                AddControl(new GuiPanel() { Text = "SIMON", Location = new SlimDX.Vector2(16 * i, 16 * i), Size = new SlimDX.Vector2(16, 16), Color = color });
            }

            
        }

       
    }
}
