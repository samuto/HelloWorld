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
    class GuiGenerateWorldForm : GuiForm
    {
        GuiPanel panelProgress = new GuiPanel();
        GuiPanel panel = new GuiPanel();
        GuiLabel labelProgress = new GuiLabel();
        GuiLabel labelGenerating = new GuiLabel();
        
        public GuiGenerateWorldForm()
        {
            Initialize();
        }


        private void Initialize()
        {
            panel.Location = new Vector2(0, 50);
            panel.Size = new Vector2(GuiScaling.Width, 10);
            AddControl(panel);

            panelProgress.Location = new Vector2(0, 0);
            panelProgress.Color = new Vector4(0.2f, 0.4f, 0.2f, 1);
            panel.AddControl(panelProgress);
            
            labelProgress.Center = true;
            panel.AddControl(labelProgress);

            labelGenerating.Text = "Loading ...";
            labelGenerating.Location = new Vector2(0, 60);
            AddControl(labelGenerating);
            
        }

        internal override void OnUpdate()
        {
            float upperLimit = 20;
            ChunkCache cache = World.Instance.GetCachedChunks();
            float total = cache.ChunkColumns.Values.Count();
            float current = cache.ChunkColumns.Values.Where(c => c.Stage == ChunkColumn.ColumnStageEnum.Decorated).Count();
            if (total > upperLimit)
            {
                total = upperLimit;
            }
            float progress = current / total;
            if (progress > 1f)
            {
                TheGame.Instance.CloseGui();
            }
            else
            {
                labelProgress.Text = (progress * 100).ToString()+"%";
                panelProgress.Size.X = progress * panel.Size.X;
                panelProgress.Size.Y = panel.Size.Y;
            }
            
        }

       
    }
}
