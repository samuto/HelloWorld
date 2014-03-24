using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace WindowsFormsApplication7.Business
{
    class DayWatch
    {
        float timeOfDay;
        public DayWatch(float timeOfDay)
        {
            this.timeOfDay = timeOfDay;
        }

        public bool IsNight
        {
            get
            {
                return !IsDay;
            }
        }
        public bool IsDay
        {
            get
            {
                return World.Instance.TimeOfDay >= 6 && World.Instance.TimeOfDay <= 18;
            }
        }

        public double DayProgress
        {
            get
            {
                return (timeOfDay - 6f) / 12f;
            }
        }

        public float SunHeight
        {
            get
            {
                float height = 0;
                if (IsDay)
                {
                    height = (float)(Math.Sin(DayProgress * Math.PI));
                }

                return height;
            }
        }

        public SlimDX.Vector3 SunPosition
        {
            get
            {
                double t = DayProgress * Math.PI;
                return new Vector3((float)Math.Cos(t), (float)Math.Sin(t), 0);
            }
        }

        public SlimDX.Vector3 MoonPosition
        {
            get
            {
                double t = (DayProgress-1f) * Math.PI;
                return new Vector3((float)Math.Cos(t), (float)Math.Sin(t), 0);
            }
        }

        public static DayWatch Now
        {
            get
            {
                return new DayWatch(World.Instance.TimeOfDay);
            }
        }
    }
}
