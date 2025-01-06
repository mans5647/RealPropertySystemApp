using RealPropertySystemApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPropertySystemApp.events
{
    public class MainEvents
    {
        public delegate void LoginPerformed(object data, int code);
        public delegate void UserChanged(UserModel updatedUser);
        public delegate void onSessionFinishedCb(object data);
    }
}
