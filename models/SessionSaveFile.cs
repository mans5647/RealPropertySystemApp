using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPropertySystemApp.models
{
    public class SessionSaveFile
    {
        private UserModel _user;
        private string  _accessToken;
        private string  _refreshToken;
        private DateTime expireTime;

        public UserModel User
     { get { return _user; } set { _user = value; } }
        
        public string AccessToken
        {
            get { return _accessToken; }
            set { _accessToken = value; }
        }

        public string RefreshToken
        {
            get { return _refreshToken; }
            set { _refreshToken = value; }
        }

        public DateTime ExpireTime
        {
            get { return expireTime; }
            set { expireTime = value; }
        }

    }
}
