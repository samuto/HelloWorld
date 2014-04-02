using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.Business;

namespace WindowsFormsApplication7.Frontend.Gui
{
    class GuiControl
    {
        public object Tag;
        public Vector2 Location;
        public Vector2 Size;
        public string Text = "<text>";
        public bool MouseIsOver = false;
        public Vector4 Color = new Vector4(0f, 0f, 1f, 1f);
        public bool Visible = true;
        public bool CustomRendering;
        public Vector2 ParentLocation = new Vector2();
        public GuiControl Parent = null;
        public event EventHandler<EventArgs> OnRender;
        protected List<GuiControl> controls = new List<GuiControl>();
        public bool SkipUpdate = true;

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
            control.ParentLocation = this.Location;
            control.Parent = this;
        }

        internal void RemoveControl(GuiControl control)
        {
            controls.Remove(control);
            control.ParentLocation = new Vector2();
            control.Parent = null;
        }


        internal virtual void Render(float partialStep)
        {

            if (!Visible)
                return;

            if (!CustomRendering)
            {
                Tessellator t = Tessellator.Instance;
                t.StartDrawingColoredQuads();
                Vector4 black = new Vector4(0, 0, 0, 1f);
                t.AddVertexWithColorAndOffset(new Vector4(Location.X, Location.Y, 0f, 1f), Color, ParentLocation);
                t.AddVertexWithColorAndOffset(new Vector4(Location.X, Location.Y + Size.Y, 0f, 1f), Color, ParentLocation);
                t.AddVertexWithColorAndOffset(new Vector4(Location.X + Size.X, Location.Y + Size.Y, 0f, 1f), Color, ParentLocation);
                t.AddVertexWithColorAndOffset(new Vector4(Location.X + Size.X, Location.Y, 0f, 1f), Color, ParentLocation);
                t.Draw();

                FontRenderer f = FontRenderer.Instance;
                t.StartDrawingAlphaTexturedQuads("ascii");
                Vector2 textSize = f.TextSize(Text);
                f.RenderText(Text, Location.X + (Size.X - textSize.X) / 2f + ParentLocation.X, Location.Y + (Size.Y - textSize.Y) / 2f + ParentLocation.Y);
                t.Draw();
            }
            if(OnRender != null)
                OnRender(this, new EventArgs());
            
            foreach (GuiControl control in controls)
            {
                control.Render(partialStep);
            }
        }

     
        internal void CenterInParent()
        {
            if(Parent == null)
                return;
            this.Location = Parent.Size / 2 - this.Size / 2;
        }


        internal void Update()
        {
            if (SkipUpdate)
            {
                SkipUpdate = false;
                return;
            }
            bool mouseLeft = Input.Instance.CurrentInput.MouseState.IsPressed(0);
            bool mouseRight = Input.Instance.CurrentInput.MouseState.IsPressed(1);
            bool mouseLeftPrev = Input.Instance.LastInput.MouseState.IsPressed(0);
            bool mouseRightPrev = Input.Instance.LastInput.MouseState.IsPressed(1);
            bool mouseMoved = Input.Instance.CurrentInput.MouseLocation != Input.Instance.LastInput.MouseLocation;

            GuiControl control = this;
            bool mouseOverPrev = control.MouseIsOver;
            bool mouseOver = MouseInRect(GuiScaling.Instance.CalcMouseLocation(Input.Instance.CurrentInput.MouseLocation), control.Location+ParentLocation, control.Size);
            control.MouseIsOver = mouseOver;

            // Fire events
            if (mouseMoved)
            {
                control.OnMouseMoved();
            }
            if (mouseOver)
            {
                if (!mouseOverPrev)
                {
                    control.OnMouseEnter();
                    if (mouseLeft || mouseRight)
                    {
                        control.OnMouseDown();
                    }
                }
                if (mouseLeft && !mouseLeftPrev || mouseRight && !mouseRightPrev)
                {
                    control.OnMouseDown();
                }
                if (!mouseLeft && mouseLeftPrev || !mouseRight && mouseRightPrev)
                {
                    control.OnMouseUp();
                }
            }
            else
            {
                if (mouseOverPrev)
                {
                    control.OnMouseLeave();
                    if (mouseLeft)
                        control.OnMouseUp();
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
        }

        internal virtual void OnMouseUp()
        {
        }
        
        internal virtual void OnMouseMoved()
        {
        }

       

    }
}
