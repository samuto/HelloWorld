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
    class GuiOptionsForm : GuiForm
    {
        GuiButton buttonCreate;
        GuiButton buttonGenerator;
        GuiLabel label;
        string[] generators = new string[] { "BIOME", "FLAT", "DEBUG" };
        

        public GuiOptionsForm()
        {
            Initialize();
        }

        private void Initialize()
        {
            buttonCreate = new GuiButton() { Size = new Vector2(130,10), Location = new Vector2(0,10), Text = "New Game" };
            buttonCreate.OnClick += new EventHandler<EventArgs>(button_OnClick);
            AddControl(buttonCreate);

            GuiPanel panel = new GuiPanel() { Size = new Vector2(110, 10), Location = new Vector2(20, 30) };
            label = new GuiLabel() { Size = new Vector2(0, 0), Location = new Vector2(0, 0), Center = true, Text = "", Color = new Vector4(0.7f, 0.7f, 0.7f, 1) };
            panel.AddControl(label);
            AddControl(panel);
            buttonGenerator = new GuiButton() { Size = new Vector2(10, 10), Location = new Vector2(0, 30), Text = "=" };
            buttonGenerator.OnClick += new EventHandler<EventArgs>(buttonGenerator_OnClick);
            AddControl(buttonGenerator);

            DataBind();
        }

        void buttonGenerator_OnClick(object sender, EventArgs e)
        {
            Toggle(generators);
            DataBind();
        }

        private void DataBind()
        {
            label.Text = generators[0];
        }

        void Toggle(string[] strings)
        {
            string firstValue = strings[0];
            for (int i = 0; i < strings.Length-1; i++)
            {
                strings[i] = strings[i + 1];
            }
            strings[strings.Length - 1] = firstValue;
        }

        void button_OnClick(object sender, EventArgs e)
        {
            WorldConfiguration config = new WorldConfiguration();
            if(generators[0] == "FLAT")
                config.Generator = new GeneratorFlat();
            else if (generators[0] == "DEBUG")
                config.Generator = new GeneratorDebug();
            else if (generators[0] == "BIOME")
                config.Generator = new GeneratorBiome();
            TheGame.Instance.NewGame(config);
        }
    }
}
