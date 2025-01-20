using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPropertySystemApp.bodies
{
    public class AuthResponse
    {
        [JsonProperty("code")]
        public int authCode {  get; set; }

        [JsonProperty("message")]
        public string? message {  get; set; }
    }
}
