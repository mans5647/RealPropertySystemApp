using RealPropertySystemApp.codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPropertySystemApp.utils
{
    public static class LogFormatter
    {
        public static string GetFmtLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace: return "TRACE";
                case LogLevel.Debug: return "DEBUG";
                case LogLevel.Info: return "INFO";
                case LogLevel.Warn: return "WARNING";
                case LogLevel.Error: return "ERROR";
                case LogLevel.Fatal: return "FATAL";
            }

            return "DEFAULT";
        }

        public static string GetFmtTime(DateTime tm)
        {
            return tm.ToString("d MMM yyyy hh:mm:ss");
        }

    }
}
