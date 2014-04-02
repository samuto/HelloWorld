using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities;
using System.ComponentModel;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.Frontend.Gui
{
    class GuiMovableControl : GuiControl
    {
        private bool dragMode = false;

        public GuiMovableControl()
        {
            CustomRendering = true;
        }


        internal override void OnMouseMoved()
        {
            if (dragMode)
            {
                FollowMouseLocation();
            }
        }

        private void FollowMouseLocation()
        {
            Location = GuiScaling.Instance.CalcMouseLocation(Input.Instance.CurrentInput.MouseLocation) - ParentLocation - Size / 2;
        }

        internal void AttachToCursor()
        {
            dragMode = true;
            FollowMouseLocation();
        }
    }
}
