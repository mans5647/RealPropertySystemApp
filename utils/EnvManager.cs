using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RealPropertySystemApp.utils
{
    public class EnvManager
    {
        private static string path = ".\\files\\settings.env";
        public static Dictionary<string, string> LoadAll()
        {
            string currentDir = Environment.CurrentDirectory;
            var lines = File.ReadAllLines(path);
            
            Dictionary<string,string> keyValuePairs = new Dictionary<string,string>();

            foreach (var line in lines)
            {
                var kv = line.Split('=');

                keyValuePairs.Add(kv[0], kv[1]);
            }

            return keyValuePairs;

        }
    }
}
