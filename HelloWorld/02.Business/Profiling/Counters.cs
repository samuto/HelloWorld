using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication7.Business.Profiling
{
    class Counters
    {
        public Dictionary<string, int> AllCounters = new Dictionary<string, int>();
        public static Counters Instance = new Counters();

        public void Reset()
        {
            foreach (var key in AllCounters.Keys.ToList())
            {
                AllCounters[key] = 0;

            }
        }


        internal void Increment(string key)
        {
            if (!AllCounters.ContainsKey(key))
                AllCounters[key] = 0;
            AllCounters[key]++;


        }

        internal void SetValue(string key, int newValue)
        {
            AllCounters[key] = newValue;
        }
    }
}
