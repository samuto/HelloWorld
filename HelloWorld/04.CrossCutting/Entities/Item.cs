using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Frontend;
using WindowsFormsApplication7.Business;
using SlimDX;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Item
    {
        public int Id;
        public string Name;

        public Item(int id, string name)
        {
            this.Id = ItemRepository.ItemIdOffset + id;
            this.Name = name;
            ItemRepository.Items[Id] = this;
        }

    }
}
