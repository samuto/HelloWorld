using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.Frontend.Gui.Controls;

namespace WindowsFormsApplication7.Frontend.Gui.Forms
{
    class GuiForm : GuiControl
    {
        public GuiForm()
        {
            CustomRendering = true;
        }

        internal void Show()
        {
            OnLoad();
            Visible = true;
        }

        internal virtual void OnLoad()
        {
        }

        internal virtual void OnClose()
        {
            Visible = false;
            Dispose();
        }
    }
}
