using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Frontend;
using SlimDX;
using WindowsFormsApplication7.CrossCutting;

namespace WindowsFormsApplication7._01.Frontend.Gui.Controls
{
    class Label
    {
        private VertexBuffer bufferText = null;
        private string text;
        private Vector3 position = new Vector3();
        public Tessellator t = Tessellator.Instance;
        public FontRenderer f = FontRenderer.Instance;
        private bool changed = false;
        private bool enableShadow = true;
        public Vector4 Color = new Vector4(1,1,1,1);

        public bool EnableShadow
        {
            get
            {
                return enableShadow;
            }
            set
            {
                changed |= enableShadow != value;
                enableShadow = value;
            }
        }
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                changed |= position != value;
                position = value;
            }
        }
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                changed |= text != value;
                text = value;

            }
        }

        private void Rebuild()
        {
            // rebuild
            VertexBuffer.Dispose(ref bufferText);
            if (string.IsNullOrEmpty(text))
                return;
            f.BeginBatch();
            if (!EnableShadow)
                f.RenderText(text, Position.X, Position.Y);
            else
                f.RenderTextShadow(Color, text, Position.X, Position.Y);
            f.StopBatch(); 
            bufferText = t.GetVertexBuffer();
            
        }

       

        public void Render()
        {
            if (string.IsNullOrEmpty(text))
                return;
            if (changed)
                Rebuild();
            changed = false;
            t.StartDrawingAlphaTexturedQuads("ascii");
            t.Draw(bufferText);
        }

        public void Dispose()
        {
            VertexBuffer.Dispose(ref bufferText);
        }

    }
}
