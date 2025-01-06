using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPropertySystemApp.models
{
    public class UserRole
    {
        private int _id;
        private string? _rolename;
        private string? _suffix;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string RoleName
        {
            get => _rolename;
            set => _rolename = value;
        }

        public string Suffix
        {
            get => _suffix;
            set => _suffix = value;
        }


    }
}
