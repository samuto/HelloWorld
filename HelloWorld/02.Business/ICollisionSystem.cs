using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using SlimDX;

namespace WindowsFormsApplication7.Business
{
    interface ICollisionSystem
    {
        Vector3 Resolve(CrossCutting.Entities.Entity entity);
        
    }
}
