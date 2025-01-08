using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPropertySystemApp.bodies
{
    public class JwtResponse
    {

        private int _code;

        public int Code
        {
            get => this._code;
            set => this._code = value;
        }


        private string? _access_token;

        [JsonProperty("access")]
        public string AccessToken
        {
            get => _access_token != null ? _access_token : "none";
            set => this._access_token = value;
        }

        private string? _refresh_token;

        [JsonProperty("refresh")]
        public string RefreshToken
        {
            get => _refresh_token != null ? _refresh_token : "none";
            set => this._refresh_token = value;
        }
    }
}
