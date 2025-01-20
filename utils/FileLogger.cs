using RealPropertySystemApp.codes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RealPropertySystemApp.utils
{
    public class FileLogger : ILoggerService
    {
        public const string GenericName = "errorLog.log";

        private string fileName;
        private const string DirName = "logs";
        private string moduleName;
        private FileLogger(string fileName = GenericName)
        {
            this.fileName = fileName;


            if (!Directory.Exists(GetDirPath()))
            {
                Directory.CreateDirectory(GetDirPath());
            }

            if (!File.Exists(GetFullFilePath()))
            {
                File.Create(FilePath()).Close();
            }

            
        }

        public static FileLogger NewFileLogger(string modName, string filename = GenericName)
        {
            var logger = new FileLogger(filename);
            logger.SetModuleName(modName);
            
            return logger;
        }

        private string GetDirPath()
        {
            return $"{Environment.CurrentDirectory}\\{DirName}";
        }
        private string GetFullFilePath()
        {
            return $"{Environment.CurrentDirectory}\\{DirName}\\{fileName}";
        }

        
        public static string GetGenericLogsFilePath()
        {
            return @$"{Environment.CurrentDirectory}\\{DirName}\\{GenericName}";
        }

        public void error(string msg)
        {
            File.AppendAllText(FilePath(), GetFormattedMessage(msg, LogLevel.Error));
        }

        public void info(string msg)
        {
            File.AppendAllText(FilePath(), GetFormattedMessage(msg, LogLevel.Info));
        }

        public void warn(string msg)
        {
            File.AppendAllText(FilePath(), GetFormattedMessage(msg, LogLevel.Warn));
        }

        private string GetFormattedMessage(string msg, LogLevel level)
        {
            string fmtDate = LogFormatter.GetFmtTime(DateTime.Now);
            string fmtLevel = LogFormatter.GetFmtLogLevel(level);

            return $"{moduleName} [{fmtDate}] {fmtLevel} {msg}\n";

        }
        private string FilePath()
        {
            return Path.Combine(GetDirPath(), fileName);
        }
        public void SetModuleName(string modName)
        {
            moduleName = modName;
        }
    }
}
