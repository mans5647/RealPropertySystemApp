using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPropertySystemApp.utils
{
    public enum LoggerType
    {
        File,
        Console
    };
    public class LoggerServiceFactory
    {
        public static ILoggerService CreateFileLogger(Type of, string filename)
        {
            return FileLogger.NewFileLogger(of.Name, filename);
        }

        public static ILoggerService CreateFileLogger(Type of)
        {
            return FileLogger.NewFileLogger(of.Name);
        }
    }
}
