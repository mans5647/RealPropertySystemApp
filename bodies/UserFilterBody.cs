using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPropertySystemApp.bodies
{
    public class UserFilterBody
    {
        [JsonProperty("firstname")]
        public string Firstname { get; set; }
        
        [JsonProperty("lastname")]
        public string Lastname { get; set; }
        
        [JsonProperty("birthDateMin")]
        public DateTime DateMin { get; set; }

        [JsonProperty("birthDateMax")]
        public DateTime DateMax { get; set; }

        [JsonProperty("role")]
        public string Rolename { get; set; }
    }
}
