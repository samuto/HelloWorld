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

        public FontRenderer()
        {
        }

        internal void RenderTextShadow(string text, float x, float y)
        {
            RenderText(new Vector4(0, 0, 0, 1), text, x + 1, y - 1);
            RenderText(new Vector4(1, 1, 1, 1), text, x, y);
        }

        internal void RenderText(string text, float x, float y)
        {
            RenderText(new Vector4(1, 1, 1, 1), text, x, y);
        }

        private void RenderText(Vector4 color, string text, float x, float y)
        {
            GlobalRenderer.Instance.Setup2dCamera();
            Tessellator t = Tessellator.Instance;
            t.StartDrawingQuadsWithAlphaBlend("ascii");
            float scale = 1;
            float width = 8 * scale;
            float height = 8 * scale;
            foreach (var character in text)
            {
                RenderChar(color, character, x, y, width, height);
                x += width;
            }
            t.Draw();
        }

        private void RenderChar(Vector4 color, char character, float posx, float posy, float width, float height)
        {
            if (character == ' ')
                return;
            Tessellator t = Tessellator.Instance;
            int index = fontMap.IndexOf(character);
            float texX = (int)(index % 16);
            float texY = (int)(index / 16);

            t.SetTextureQuad(new Vector2(texX / 16f, texY / 16f), 1f / 16f, 1f / 16f);
            t.AddVertexWithColor(new Vector4(posx + 0f, posy + 0f, 0f, 1.0f), color);
            t.AddVertexWithColor(new Vector4(posx + 0f, posy + height, 0f, 1.0f), color);
            t.AddVertexWithColor(new Vector4(posx + width, posy + height, 0f, 1.0f), color);
            t.AddVertexWithColor(new Vector4(posx + width, posy + 0f, 0f, 1.0f), color);
        }
    }
}
