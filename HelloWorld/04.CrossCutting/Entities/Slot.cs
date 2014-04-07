using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;

namespace WindowsFormsApplication7.CrossCutting.Entities
{
    class Slot
    {
        private EntityStack content = new EntityStack(0, 0);

        public EntityStack Content
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

        public bool IsFull
        {
            get
            {
                return Content.Count == 64;
            }
        }
    }
}
