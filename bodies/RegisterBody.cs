using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RealPropertySystemApp.bodies
{
    public class RegisterBody
    {
        private string _login, _password, _code;

        [JsonProperty("login")]
        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }

        [JsonProperty("password")]
        public string Password
            { get { return _password; } set { _password = value; } }


        [JsonProperty("code")]
        public string Code { get { return _code;
                } set { _code = value; } }


    }
}
