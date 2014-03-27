using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Recepe
    {
        public ItemStack Product;
        private string rows;
        private int[] ids;
        private string key;

        internal Recepe Input(string rows, params int[] ids)
        {
            this.rows = rows;
            this.ids = ids;
            this.key = rows + "," + string.Join(":", ids);
            return this;
        }

        internal Recepe Output(int id, int count)
        {
            Product = new ItemStack(id, count);
            return this;
        }

        internal string Key
        {
            get
            {
                return key;
            }
        }

        internal static string GetKeyFromGrid(Slot[] Grid)
        {
            List<int> ids = new List<int>();
            int minX = 3;
            int maxX = 0;
            int minY = 3;
            int maxY = 0;
            int x, y;
            for (int i = 0; i < 9; i++)
            {
                x = i % 3;
                y = i / 3;
                if (Grid[i].IsNotEmpty)
                {
                    if (x < minX)
                        minX = x;
                    if (x > maxX)
                        maxX = x;
                    if (y < minY)
                        minY = y;
                    if (y > maxY)
                        maxY = y;
                }
            }
            string key = "";
            for (y = minY; y <= maxY; y++)
            {
                for (x = minX; x <= maxX; x++)
                {

                    int i = x + y * 3;
                    if (Grid[i].Content.IsEmpty)
                    {
                        key += "0";
                        continue;
                    }
                    int id = Grid[i].Content.Id;
                    if (!ids.Contains(id))
                    {
                        ids.Add(id);
                    }
                    key += ids.IndexOf(id) + 1;
                }
                key += ",";
            }
            key += string.Join(":", ids);
            return key;
        }
    }
}
