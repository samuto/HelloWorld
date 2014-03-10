using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Diagnostics;

namespace WindowsFormsApplication7.Business
{
	public class Timer
	{
        public int ElapsedTicks;
        public float RenderPartialTicks;
        public float TimerSpeed = 1.0F;
        public float ElapsedPartialTicks = 0.0F;

        private float ticksPerSecond;
		private double lastHRTime;
		private long lastSyncSysClock;
		private long lastSyncHRClock;
		private long elapsedSinceLastSync;
		private double timeSyncAdjustment = 1.0D;
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        private static Stopwatch sw = new Stopwatch();

        static Timer()
        {
            sw.Start();
        }

        public static long getSystemTime()
        {
            return getTime() * 1000L / getTimerResolution();
        }


        public static long currentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

        public static long nanoTime()
        {
            return (getTime()* 1000000000L) / getTimerResolution(); 
        }

        internal static long getTime()
        {
            return sw.ElapsedTicks;
        }

        internal static long getTimerResolution()
        {
            return Stopwatch.Frequency;
        }


        public Timer(float par1)
        {
            this.ticksPerSecond = par1;
            this.lastSyncSysClock = getSystemTime();
            this.lastSyncHRClock = nanoTime() / 1000000L;
        }

        /**
    * Updates all fields of the Timer using the current time
    */
        public void updateTimer()
        {
            long now = getSystemTime();
            long elapsed = now - this.lastSyncSysClock;
            long nowMilliseconds = nanoTime() / 1000000L;
            double nowSeconds = (double)nowMilliseconds / 1000.0D;

            if (elapsed <= 1000L && elapsed >= 0L)
            {
                this.elapsedSinceLastSync += elapsed;

                if (this.elapsedSinceLastSync > 1000L)
                {
                    long elapsedSync = nowMilliseconds - this.lastSyncHRClock;
                    double var11 = (double)this.elapsedSinceLastSync / (double)elapsedSync;
                    this.timeSyncAdjustment += (var11 - this.timeSyncAdjustment) * 0.20000000298023224D;
                    this.lastSyncHRClock = nowMilliseconds;
                    this.elapsedSinceLastSync = 0L;
                }

                if (this.elapsedSinceLastSync < 0L)
                {
                    this.lastSyncHRClock = nowMilliseconds;
                }
            }
            else
            {
                this.lastHRTime = nowSeconds;
            }

            this.lastSyncSysClock = now;
            double var13 = (nowSeconds - this.lastHRTime) * this.timeSyncAdjustment;
            this.lastHRTime = nowSeconds;

            if (var13 < 0.0D)
            {
                var13 = 0.0D;
            }

            if (var13 > 1.0D)
            {
                var13 = 1.0D;
            }

            this.ElapsedPartialTicks = (float)((double)this.ElapsedPartialTicks + var13 * (double)this.TimerSpeed * (double)this.ticksPerSecond);
            this.ElapsedTicks = (int)this.ElapsedPartialTicks;
            this.ElapsedPartialTicks -= (float)this.ElapsedTicks;

            if (this.ElapsedTicks > 10)
            {
                this.ElapsedTicks = 10;
            }

            this.RenderPartialTicks = this.ElapsedPartialTicks;
        }
	}
}
