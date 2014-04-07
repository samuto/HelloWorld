using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication7.CrossCutting
{
    class Log
    {
        public static Log Instance = new Log();
        List<string> logLines = new List<string>();
        int logTime = 0;
        int logStep = 0;

        public void Debug(string text)
        {
            LogLine("Debug", text);
        }

        public void Update()
        {
            logTime = TheGame.Instance.CurrentTick;
            logStep = 0;
        }

        private void LogLine(string type, string text)
        {
            logLines.Add(string.Format("{0}.{1}: {1} {2}",
                (logTime+"."+logStep).PadRight(8),
                type.PadRight(8),
                text));
            logStep++;
        }

        internal string[] Last(int returnCount)
        {
            int currentCount = logLines.Count;
            if (returnCount > currentCount)
                returnCount = currentCount;
            string[] lastLines = new string[returnCount];
            for (int i = 0; i < returnCount; i++)
            {
                lastLines[i] = logLines[currentCount - returnCount + i];
            }
            return lastLines;
        }
    }

}
