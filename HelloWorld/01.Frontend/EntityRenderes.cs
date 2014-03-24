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
        Dictionary<Entity, EntityRenderer> renderers = new Dictionary<Entity, EntityRenderer>();


        internal EntityRenderer GetRenderer(Entity entity)
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
