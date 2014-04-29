using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.RawInput;
using SlimDX.DirectInput;
using WindowsFormsApplication7.CrossCutting.Entities;
using SlimDX;
using WindowsFormsApplication7._01.Frontend;
using WindowsFormsApplication7.Frontend;
using WindowsFormsApplication7.Business.Landscape;

namespace WindowsFormsApplication7.Business
{
    class WorldConfiguration
    {
        public GeneratorBase Generator = new GeneratorBiome();
    }
}