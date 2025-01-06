using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPropertySystemApp.codes
{
    public class AuthCode
    {
        public const int ErrInvalidFields = -3;
        public const int AuthErrorNoHeader = 1;
        public const int AuthErrorFailed = 2;
        public const int AuthTokenInvalid = 3;
        public const int AuthLoginNotFound = 4;
        public const int AuthPasswordIncorrect = 5;
        public const int AuthTokenExpired = 6;
        public const int RegisterFailedAccountAlreadyExists = 7;
        public const int InvalidCode = 8;
    }
}
