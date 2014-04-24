using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.Business.Landscape
{
    abstract class GeneratorBase
    {
        internal abstract void Generate(Chunk chunk);

        internal virtual void Decorate(Chunk chunk)
        {
        }
    }
}