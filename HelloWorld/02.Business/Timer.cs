using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WindowsFormsApplication7.Business
{
    static class Timer
    {
        private static double t = 0.0;
        private const double dt = 1d / 20d;
        private static double currentTime;
        private static double accumulator = 0.0;
        private static Stopwatch sw = new Stopwatch();
        private static double newTime;
        private static double frameTime;
        static Timer()
        {
            sw.Start();
            currentTime = ElapsedSeconds();
        }

        internal static double ElapsedSeconds()
        {
            return sw.ElapsedTicks / (double)Stopwatch.Frequency;
        }

        internal static void NextFrame()
        {
            newTime = ElapsedSeconds();
            frameTime = newTime - currentTime;
            if (frameTime > 0.25)
                frameTime = 0.25;	  // note: max frame time to avoid spiral of death
            currentTime = newTime;
            accumulator += frameTime;
        }

        internal static bool MoreStepsInFrame()
        {
            return accumulator >= dt;
        }

        internal static void Step()
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
