using Newtonsoft.Json;
using RealPropertySystemApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPropertySystemApp.bodies
{
    public class UserBody
    {
        private UserModel _user;

        public UserModel user { get { return _user; } set { _user = value; } }

        private long _roleId;

        [JsonProperty("role_id")]
        public long roleId
        {
            get { return _roleId; }
            set { _roleId = value; }

        }
    }
}
