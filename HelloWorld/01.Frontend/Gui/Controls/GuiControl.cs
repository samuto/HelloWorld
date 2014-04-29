using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.Business;

namespace WindowsFormsApplication7.Frontend.Gui.Controls
{
    class GuiControl
    {
        public event EventHandler<EventArgs> OnClick;
        public object Tag;
        private Vector2 location;
        public Vector2 Size;
        public string text = "<text>";
        public bool MouseIsOver = false;
        public Vector4 Color = new Vector4(0f, 0f, 1f, 1f);
        public bool Visible = true;
        public bool CustomRendering;
        public GuiControl Parent = null;
        public event EventHandler<EventArgs> RenderControl;
        protected List<GuiControl> controls = new List<GuiControl>();
        public bool SkipUpdate = true;
        protected Tessellator t = Tessellator.Instance;

        public Vector2 GlobalLocation
        {
            get
            {
                if (Parent == null)
                    return Location;
                return Location + Parent.GlobalLocation;
            }
        }

        public Vector2 Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
                OnLocationChanged();
            }
        }

        protected virtual void OnLocationChanged()
        {
        }

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                OnTextChanged(text);
            }
        }

        protected virtual void OnTextChanged(string text)
        {
        }

        public GuiControl[] Controls
        {
            get
            {
                return controls.ToArray();
            }
        }
        public GuiControl()
        {

        }

        internal void AddControl(GuiControl control)
        {
            controls.Add(control);
            control.Parent = this;
            control.OnLocationChanged();
        }

        internal void RemoveControl(GuiControl control)
        {
            controls.Remove(control);
            control.Parent = null;
            control.OnLocationChanged();
        }

        internal virtual void OnRender(float partialStep)
        {
        }

        internal void Render(float partialStep)
        {
            if (!Visible)
                return;

            if (!CustomRendering)
            {
                t.StartDrawingColoredQuads();
                t.AddVertexWithColor(new Vector4(GlobalLocation.X, GlobalLocation.Y, 0f, 1f), Color);
                t.AddVertexWithColor(new Vector4(GlobalLocation.X, GlobalLocation.Y + Size.Y, 0f, 1f), Color);
                t.AddVertexWithColor(new Vector4(GlobalLocation.X + Size.X, GlobalLocation.Y + Size.Y, 0f, 1f), Color);
                t.AddVertexWithColor(new Vector4(GlobalLocation.X + Size.X, GlobalLocation.Y, 0f, 1f), Color);
                t.Draw();
            }
            else
                OnRender(partialStep);
            if(RenderControl != null)
                RenderControl(this, new EventArgs());
            
            foreach (GuiControl control in controls)
            {
                control.Render(partialStep);
            }
        }

     
        internal void CenterInParent()
        {
            if(Parent == null)
                return;
            this.Location = (Parent.Size - this.Size) / 2f;
        }

        internal virtual void OnUpdate()
        {

        }

        internal void Update()
        {
            if (SkipUpdate)
            {
                SkipUpdate = false;
                return;
            }
            OnUpdate();
            bool mouseLeft = Input.Instance.CurrentInput.MouseState.IsPressed(0);
            bool mouseRight = Input.Instance.CurrentInput.MouseState.IsPressed(1);
            bool mouseLeftPrev = Input.Instance.LastInput.MouseState.IsPressed(0);
            bool mouseRightPrev = Input.Instance.LastInput.MouseState.IsPressed(1);
            bool mouseMoved = Input.Instance.CurrentInput.MouseLocation != Input.Instance.LastInput.MouseLocation;


            bool mouseOverPrev = MouseIsOver;
            Vector2 pos = new Vector2(GlobalLocation.X, GlobalLocation.Y);
            bool mouseOver = MouseInRect(GuiScaling.Instance.CalcMouseLocation(Input.Instance.CurrentInput.MouseLocation), pos, Size);
            MouseIsOver = mouseOver;

            // Fire events
            if (mouseMoved)
            {
                OnMouseMoved();
            }
            if (mouseOver)
            {
                if (!mouseOverPrev)
                {
                    OnMouseEnter();
                    if (mouseLeft || mouseRight)
                    {
                        OnMouseDown();
                    }
                }
                if (mouseLeft && !mouseLeftPrev || mouseRight && !mouseRightPrev)
                {
                    OnMouseDown();
                }
                if (!mouseLeft && mouseLeftPrev || !mouseRight && mouseRightPrev)
                {
                    OnMouseUp();
                }
            }
            else
            {
                if (mouseOverPrev)
                {
                    OnMouseLeave();
                    if (mouseLeft)
                        OnMouseUp();
                }
            }
           

            foreach (GuiControl control2 in controls.ToArray())
            {
                control2.Update();
            }
        }



        protected bool MouseInRect(SlimDX.Vector2 mouseLocation, SlimDX.Vector2 rectangleLocation, SlimDX.Vector2 rectangleSize)
        {
            if (mouseLocation.X >= rectangleLocation.X &&
                mouseLocation.Y >= rectangleLocation.Y &&
                mouseLocation.X <= rectangleLocation.X + rectangleSize.X &&
                mouseLocation.Y <= rectangleLocation.Y + rectangleSize.Y)
            {
                return true;
            }
            return false;
        }


        internal virtual void OnMouseEnter()
        {
        }

        internal virtual void OnMouseLeave()
        {
        }

        internal virtual void OnMouseDown()
        {
            if (OnClick != null)
                OnClick(this, new EventArgs());
        }

        internal virtual void OnMouseUp()
        {
        }
        
        internal virtual void OnMouseMoved()
        {
        }

        public virtual void Dispose()
        {
            foreach (GuiControl child in controls)
            {
                child.Dispose();
            }
        }

    }
}
