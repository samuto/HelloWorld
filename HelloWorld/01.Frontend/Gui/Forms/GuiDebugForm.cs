using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.Frontend.Gui.Controls;
using SlimDX;
using WindowsFormsApplication7.Business.Landscape;

namespace WindowsFormsApplication7.Frontend.Gui.Forms
{
    class GuiDebugForm : GuiForm
    {
        GuiButton buttonToggleGravity;
        GuiButton buttonToggleSpeed;
        GuiButton buttonToggleCollision;
        bool gravity;
        bool speed;
        bool collision;

        public GuiDebugForm()
        {
            Initialize();
            DataBind();
        }

        private void DataBind()
        {
            gravity = World.Instance.Player.accGravity.Length() > 0;
            buttonToggleGravity.Text = "Gravity " + (gravity ? "on" : "off");

            speed = World.Instance.Player.Speed != 0.15f;
            buttonToggleSpeed.Text = "Speed " + (speed ? "on" : "off");

            collision = World.Instance.Player.CollisionEnabled;
            buttonToggleCollision.Text = "Collision " + (collision ? "on" : "off");

        }

        private void Initialize()
        {
            buttonToggleGravity = new GuiButton() { Size = new Vector2(130,10), Location = new Vector2(0,10), Text = "" };
            buttonToggleGravity.OnClick += new EventHandler<EventArgs>(buttonToggleGravity_OnClick);
            AddControl(buttonToggleGravity);

            buttonToggleSpeed= new GuiButton() { Size = new Vector2(130, 10), Location = new Vector2(0, 30), Text = "" };
            buttonToggleSpeed.OnClick += new EventHandler<EventArgs>(buttonToggleSpeed_OnClick);
            AddControl(buttonToggleSpeed);

            buttonToggleCollision = new GuiButton() { Size = new Vector2(130, 10), Location = new Vector2(0, 50), Text = "" };
            buttonToggleCollision.OnClick += new EventHandler<EventArgs>(buttonToggleCollision_OnClick);
            AddControl(buttonToggleCollision);

        }

        void buttonToggleGravity_OnClick(object sender, EventArgs e)
        {
            if (gravity)
                World.Instance.Player.accGravity = new Vector3();
            else
                World.Instance.Player.accGravity = Player.defaultGravity;
            DataBind();
        }

        void buttonToggleSpeed_OnClick(object sender, EventArgs e)
        {
            if (speed)
                World.Instance.Player.Speed = 0.15f;
            else
                World.Instance.Player.Speed = 0.75f;
            DataBind();
        }

        void buttonToggleCollision_OnClick(object sender, EventArgs e)
        {
            if (collision)
                World.Instance.Player.CollisionEnabled = false;
            else
                World.Instance.Player.CollisionEnabled = true;
            DataBind();
        }

       

        

       
    }
}
