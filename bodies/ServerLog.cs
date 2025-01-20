using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPropertySystemApp.bodies
{
    public class ServerLog
    {
        [JsonProperty("id")]
        private int _logId;

        [JsonProperty("time")]
        private DateTime _logTime;

        [JsonProperty("level")]
        private int _logLevel;

        [JsonProperty("content")]
        private string _content;


        public int LogId
        {
            get { return _logId; }
            set { _logId = value; }
        }

        public DateTime LogTime
        { get { return _logTime; }
            set { _logTime = value; } }
        

        public int LogLevel
            { get { return _logLevel; } set { _logLevel = value; } }

        public string Content
            { get { return _content; } set {_content = value; } }



    }
}
