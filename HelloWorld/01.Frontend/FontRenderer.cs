using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace WindowsFormsApplication7.Frontend
{
    class FontRenderer
    {
        public static FontRenderer Instance = new FontRenderer();
        private Tessellator t = Tessellator.Instance;
        string fontMap = @"
                
                
 !""#$%&'()*+,-./
0123456789:;<=>?
@ABCDEFGHIJKLMNO
PQRSTUVWXYZ[\]^_
'abcdefghijklmno
pqrstuvwxyz{|}~⌂
ÇüéâäàåçêëèïîìÄÅ
ÉæÆôöòûùÿÖÜø£Ø×ƒ
áíóúñÑªº¿®¬½¼¡«»".Replace(Environment.NewLine, "");
        private float charScale = 1f;
        private VertexBuffer[] charVertexBuffers = new VertexBuffer[256];
        public float CharWidth;
        public float LineHeight;
        private bool renderBatch = false;

        public float CharScale
        {
            get
            {
                return charScale;
            }
            set
            {
                charScale = value;
                CharWidth = 8f * charScale;
                LineHeight = 8f * charScale;
            }
        }
       
        public FontRenderer()
        {
            CharWidth = 8f * CharScale;
            LineHeight = 8f * CharScale;

        }

        internal void RenderTextShadow(string text, float x, float y)
        {
            RenderText(new Vector4(0, 0, 0, 1), text, x + CharScale, y - CharScale);
            RenderText(new Vector4(1, 1, 1, 1), text, x, y);
        }

        internal void RenderTextShadow(Vector4 color, string text, float x, float y)
        {
            RenderText(new Vector4(0, 0, 0, 1), text, x + CharScale, y - CharScale);
            RenderText(color, text, x, y);
        }

        internal Vector2 TextSize(string text)
        {
            return new Vector2(text.Length * CharWidth, LineHeight);
        }

        internal void RenderText(string text, float x, float y)
        {
            RenderText(new Vector4(1, 1, 1, 1), text, x, y);
        }

        internal void StopBatch()
        {
            renderBatch = false;
        }

        internal void BeginBatch()
        {
            t.StartDrawingAlphaTexturedQuads("ascii");
            renderBatch = true;
        }

        internal void DrawBatch()
        {
            renderBatch = false;
            t.Draw();
        }

        internal void RenderText(Vector4 color, string text, float x, float y)
        {
            GlobalRenderer.Instance.Setup2dCamera();
            if (!renderBatch)
            {
                t.StartDrawingAlphaTexturedQuads("ascii");
            }
            foreach (var character in text)
            {
                RenderChar(color, character, x, y);
                x += CharWidth;
            }
            if (!renderBatch)
                t.Draw();
        }

        private void RenderChar(Vector4 color, char character, float posx, float posy)
        {
           
            if (character == ' ')
                return;
            int index = fontMap.IndexOf(character);
            float texX = (int)(index % 16);
            float texY = (int)(index / 16);
            t.SetTextureQuad(new Vector2(texX / 16f, texY / 16f), 1f / 16f, 1f / 16f);
            t.AddVertexWithColor(new Vector4(posx + 0f, posy + 0f, 0f, 1.0f), color);
            t.AddVertexWithColor(new Vector4(posx + 0f, posy + LineHeight, 0f, 1.0f), color);
            t.AddVertexWithColor(new Vector4(posx + CharWidth, posy + LineHeight, 0f, 1.0f), color);
            t.AddVertexWithColor(new Vector4(posx + CharWidth, posy + 0f, 0f, 1.0f), color);
        }

        public void Dispose()
        {
            for(int i=0; i<charVertexBuffers.Length; i++)
            {
                VertexBuffer buffer = charVertexBuffers[i];
                if(buffer != null)
                    buffer.Dispose();
                buffer = null;
            }
        }
    }
}
