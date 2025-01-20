using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPropertySystemApp.utils
{
    public interface ILoggerService
    {
        public void warn(string msg);
        public void error(string msg);
        public void info(string msg);

        public void SetModuleName(string modName);
    }
}
