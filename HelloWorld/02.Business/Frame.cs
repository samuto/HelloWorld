using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WindowsFormsApplication7.Business
{
    static class Frame
    {
        private static double t = 0.0;
        private const double dt = 1d / 40d;
        private static double currentTime;
        private static double accumulator = 0.0;
        private static Stopwatch sw = new Stopwatch();
        private static double newTime;
        private static double frameTime;

        static Frame()
        {
            sw.Start();
            currentTime = ElapsedSecondsSinceGameStart();
        }

        private static double ElapsedSecondsSinceGameStart()
        {
            return sw.ElapsedTicks / (double)Stopwatch.Frequency;
        }

        internal static void Next()
        {
            newTime = ElapsedSecondsSinceGameStart();
            frameTime = newTime - currentTime;
            if (frameTime > 0.25)
                frameTime = 0.25;	  // note: max frame time to avoid spiral of death
            currentTime = newTime;
            accumulator += frameTime;
        }

        internal static bool HasMoreTicks()
        {
            return accumulator >= dt;
        }

        internal static void Tick()
        {
            t += dt;
            accumulator -= dt;
        }

        internal static float GetPartialStep()
        {
            return (float)(accumulator / dt);
        }
    }
}
