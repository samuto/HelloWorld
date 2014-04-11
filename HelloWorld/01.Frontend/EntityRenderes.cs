using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities;
using SlimDX;
using SlimDX.Direct3D11;
using WindowsFormsApplication7._01.Frontend;

namespace WindowsFormsApplication7.Frontend
{
    class EntityRenderers
    {
        Dictionary<EntityPlayer, EntityRenderer> renderers = new Dictionary<EntityPlayer, EntityRenderer>();


        internal EntityRenderer GetRenderer(EntityPlayer entity)
        {
            if (renderers.ContainsKey(entity))
                return renderers[entity];
            // build renderer
            EntityRenderer renderer = new EntityRenderer(entity);
            renderers.Add(entity, renderer);
            return renderer;
           
        }
    }
}
