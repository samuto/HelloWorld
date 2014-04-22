using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Frontend;
using SlimDX;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Frontend.Gui;
using WindowsFormsApplication7._01.Frontend.Gui;
using WindowsFormsApplication7._01.Frontend.Gui.Controls;

namespace WindowsFormsApplication7.Frontend
{
    class HeadUpDisplay
    {
        private Tessellator t = Tessellator.Instance;
        private float frameSize = 24f;
        private float itemSize = 16f;
        private Player player;
        private Label[] labels = new Label[9];
        private VertexBuffer healthBuffer = new VertexBuffer();
        private VertexBuffer hungerBuffer = new VertexBuffer();
        private float lastHealth = -1f;
        private float lastHunger = -1f;
        private VertexBuffer background = new VertexBuffer();


        public HeadUpDisplay()
        {
            player = World.Instance.Player;

            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = new Label();
                labels[i].Color = new Vector4(1f, 1f, 0.2f, 1);
            }

            t.StartDrawingColoredQuads();
            float m = 0.05f;
            Vector4 Color1 = new Vector4(0.1f, 0.1f, 0.1f, 1);
            t.AddVertexWithColor(new Vector4(0, 0, 0f, 1f), Color1);
            t.AddVertexWithColor(new Vector4(0, 1, 0f, 1f), Color1);
            t.AddVertexWithColor(new Vector4(1, 1, 0f, 1f), Color1);
            t.AddVertexWithColor(new Vector4(1, 0, 0f, 1f), Color1);
         background = t.GetVertexBuffer();
        }

        private void GenerateHealthBuffer()
        {
            if (lastHealth == player.Health)
                return;
            VertexBuffer.Dispose(ref healthBuffer);
            float pixel = 1f / 256f;
            float texX = 16f * pixel;
            float texY = 0;
            t.StartDrawingAlphaTexturedQuads("icons");
            Vector4 color = new Vector4(1, 1, 1, 1);
            float posx = 0;
            for (int i = 0; i < 10; i++)
            {
                t.SetTextureQuad(new Vector2(texX, texY), 9f * pixel, 9f * pixel);
                t.AddVertexWithColor(new Vector4(posx + 0f, 0f, 0f, 1.0f), color);
                t.AddVertexWithColor(new Vector4(posx + 0f, 9f, 0f, 1.0f), color);
                t.AddVertexWithColor(new Vector4(posx + 9f, 9f, 0f, 1.0f), color);
                t.AddVertexWithColor(new Vector4(posx + 9f, 0f, 0f, 1.0f), color);
                if (i < (int)(player.Health / 10f))
                {
                    t.SetTextureQuad(new Vector2(texX + pixel * 4f * 9f, texY), 9f * pixel, 9f * pixel);
                    t.AddVertexWithColor(new Vector4(posx + 0f, 0f, 0f, 1.0f), color);
                    t.AddVertexWithColor(new Vector4(posx + 0f, 9f, 0f, 1.0f), color);
                    t.AddVertexWithColor(new Vector4(posx + 9f, 9f, 0f, 1.0f), color);
                    t.AddVertexWithColor(new Vector4(posx + 9f, 0f, 0f, 1.0f), color);
                }
                posx += 9f;
            }
            healthBuffer = t.GetVertexBuffer();
            lastHealth = player.Health;
        }

        private void GenerateHungerBuffer()
        {
            if (lastHunger == player.Hunger)
                return;
            VertexBuffer.Dispose(ref hungerBuffer);
            float pixel = 1f / 256f;
            float texX = pixel * (16f + 9f * 0f);
            float texY = pixel * (9f * 3f);
            t.StartDrawingAlphaTexturedQuads("icons");
            Vector4 color = new Vector4(1, 1, 1, 1);
            float posx = 9f * 12;
            for (int i = 0; i < 10; i++)
            {
                t.SetTextureQuad(new Vector2(texX, texY), 9f * pixel, 9f * pixel);
                t.AddVertexWithColor(new Vector4(posx + 0f, 0f, 0f, 1.0f), color);
                t.AddVertexWithColor(new Vector4(posx + 0f, 9f, 0f, 1.0f), color);
                t.AddVertexWithColor(new Vector4(posx + 9f, 9f, 0f, 1.0f), color);
                t.AddVertexWithColor(new Vector4(posx + 9f, 0f, 0f, 1.0f), color);
                if (i > (int)(player.Hunger/10f)-1)
                {
                    t.SetTextureQuad(new Vector2(texX + pixel * 4f * 9f, texY), 9f * pixel, 9f * pixel);
                    t.AddVertexWithColor(new Vector4(posx + 0f, 0f, 0f, 1.0f), color);
                    t.AddVertexWithColor(new Vector4(posx + 0f, 9f, 0f, 1.0f), color);
                    t.AddVertexWithColor(new Vector4(posx + 9f, 9f, 0f, 1.0f), color);
                    t.AddVertexWithColor(new Vector4(posx + 9f, 0f, 0f, 1.0f), color);
                }
                posx += 10.5f;
            }
            hungerBuffer = t.GetVertexBuffer();
            lastHunger = player.Hunger;
        }

        public void Render()
        {
            GenerateHealthBuffer();
            GenerateHungerBuffer();

            t.ResetTransformation();
            float temp = (int)(TheGame.Instance.Width / (frameSize * 13f));
            t.Scale = new Vector3(1, 1, 1) * temp;
            t.Translate = new Vector3(1, 0, 0) * (TheGame.Instance.Width - frameSize * 9f * temp) / 2f;
            t.Translate.Y = 0;

            // Draw slot 0-8 in inventory
            int[] order = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            order[8] = player.SelectedSlotId;
            order[player.SelectedSlotId] = 8;
            foreach (int i in order)
            {
                if (i == player.SelectedSlotId)
                    continue;
                DrawItem(i);
            }
            DrawItem(player.SelectedSlotId);

            // Draw hearts
            t.Scale = new Vector3(1, 1, 1) * temp;
            t.Translate = new Vector3(
                (TheGame.Instance.Width - frameSize * 9f * temp) / 2f,
                (frameSize + 8) * temp,
                0);
            t.StartDrawingAlphaTexturedQuads("icons");
            t.Draw(healthBuffer);
            t.StartDrawingAlphaTexturedQuads("icons");
            t.Draw(hungerBuffer);

        }

        private void DrawItem(int i)
        {
            float scaleAdjust = i == player.SelectedSlotId ? 1.00f : 1f;
            Vector3 postAdjust = i != player.SelectedSlotId ? new Vector3(0, 4, 0) : new Vector3();
            t.StartDrawingColoredQuads();
            Vector3 pos = new Vector3((i) * frameSize, 0, 0) + postAdjust;
            Camera.Instance.World = Matrix.Multiply(Camera.Instance.World, Matrix.Scaling(new Vector3(frameSize, frameSize, frameSize) * scaleAdjust));
            Camera.Instance.World = Matrix.Multiply(Camera.Instance.World, Matrix.Translation(pos));
            t.Draw(background);

            EntityStack stack = player.Inventory.Slots[i].Content;
            if (stack.IsEmpty)
                return;

            t.StartDrawingTiledQuadsWTF();
            pos += new Vector3(1 + (frameSize - itemSize) / 2f, -1 + (frameSize - itemSize) / 2f, 0);
            Camera.Instance.World = Matrix.Multiply(Camera.Instance.World, Matrix.Scaling(new Vector3(itemSize, itemSize, itemSize) * scaleAdjust));
            Camera.Instance.World = Matrix.Multiply(Camera.Instance.World, Matrix.Translation(pos));
            t.Draw(TileTextures.Instance.GetItemVertexBuffer(stack.Id));

            labels[i].Text = stack.Count.ToString();
            labels[i].Position = pos;
            labels[i].Render();
        }

        public void Dispose()
        {
            for (int i = 0; i < labels.Length; i++)
            {
                if (labels[i] != null)
                    labels[i].Dispose();
                labels[i] = null;
            }
            VertexBuffer.Dispose(ref healthBuffer);
            VertexBuffer.Dispose(ref hungerBuffer);
        }
    }
}
