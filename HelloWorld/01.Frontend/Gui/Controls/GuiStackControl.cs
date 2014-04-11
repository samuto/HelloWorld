using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities;
using System.ComponentModel;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.Frontend.Gui.Controls
{
    class GuiStackControl : GuiControl
    {
        private bool dragMode = false;
        private GuiLabel labelCount = new GuiLabel();

        public GuiStackControl()
        {
            CustomRendering = true;
            AddControl(labelCount);
        }

        protected override void OnTextChanged(string text)
        {
            labelCount.Text = text;
        }

        internal override void OnRender(float partialStep)
        {
            float itemSize = this.Size.X;
            
            t.StartDrawingTiledQuadsWTF();
            Camera.Instance.World = Matrix.Multiply(Camera.Instance.World, Matrix.Scaling(new Vector3(itemSize, itemSize, itemSize)));
            Camera.Instance.World = Matrix.Multiply(Camera.Instance.World, Matrix.Translation(new Vector3(GlobalLocation,0)));
            EntityStack stack = (EntityStack)Tag;
            t.Draw(TileTextures.Instance.GetItemVertexBuffer(stack.Id));
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
            Location = GuiScaling.Instance.CalcMouseLocation(Input.Instance.CurrentInput.MouseLocation) - Size / 2;
        }

        internal void AttachToCursor()
        {
            dragMode = true;
            FollowMouseLocation();
        }
    }
}
