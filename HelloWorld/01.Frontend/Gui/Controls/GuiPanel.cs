using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace WindowsFormsApplication7.Frontend.Gui.Controls
{
    class GuiPanel : GuiControl
    {
        private Vector4 ColorActivated = new Vector4(0.5f, 0.5f, 0.5f, 1);
        private Vector4 ColorDeactivated = new Vector4(0.4f, 0.4f, 0.4f, 1);

        public GuiPanel()
        {
            Color = ColorDeactivated;
        }

        internal override void OnMouseEnter()
        {
            Color = ColorActivated;
        }

        internal override void OnMouseLeave()
        {
            Color = ColorDeactivated;
        }

        


        internal void ClearControls()
        {
            controls.Clear();
        }
    }
}
