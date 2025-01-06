using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RealPropertySystemApp.bodies
{
    public class MessageMeta
    {

        private int _code;
        private string _message;

        [JsonPropertyName("code")]
        public int Code
        {
            get => _code; set => _code = value;
        }

        [JsonPropertyName("message")]
        public string Message
        {
            get { return _message; }
            set => _message = value;
        }


        public MessageMeta() { }

    }
}
