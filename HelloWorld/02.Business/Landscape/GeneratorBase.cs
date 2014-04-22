using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business.Repositories;

namespace WindowsFormsApplication7.Business.Landscape
{
    abstract class ChunkGeneratorBase
    {
        internal abstract void Generate(Chunk chunk);
    }
}