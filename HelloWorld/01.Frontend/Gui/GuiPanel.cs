using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace WindowsFormsApplication7.Frontend.Gui
{
    class GuiPanel : GuiControl
    {
        public event EventHandler<EventArgs> OnClick;
        private Vector4 ColorActivated = new Vector4(1f, 0.5f, 0.5f, 1);
        private Vector4 ColorDeactivated = new Vector4(0.8f, 0.3f, 0.3f, 1);

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

        internal override void OnMouseDown()
        {
            if(OnClick != null)
                OnClick(this, new EventArgs());
        }


        internal void ClearControls()
        {
            controls.Clear();
        }
    }
}
