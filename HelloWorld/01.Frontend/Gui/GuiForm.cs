using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business;

namespace WindowsFormsApplication7.Frontend.Gui
{
    class GuiForm : GuiControl
    {
        public GuiForm()
        {
            CustomRendering = true;
        }

        internal virtual void Show()
        {
            OnLoad();
            Visible = true;
        }

        internal virtual void OnLoad()
        {
        }

        internal void Close()
        {
            Visible = false;
        }
    }
}
