using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Slot
    {
        private ItemStack content = new ItemStack(0, 0);

        public ItemStack Content
        {
            get
            {
                return content;
            }
        }

        public bool IsNotEmpty
        {
            get
            {
                return Content.Count > 0;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return !IsNotEmpty;
            }
        }
    }
}
