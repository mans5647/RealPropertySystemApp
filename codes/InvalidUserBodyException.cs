using RealPropertySystemApp.bodies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPropertySystemApp.codes
{
    public class InvalidUserBodyException : Exception
    {
        private ValidationResponse response;
        public InvalidUserBodyException(ValidationResponse response, string message) : base(message)
        {
            this.response = response;
        }

        public ValidationResponse Get()
        {
            return this.response; 
        }

        public static string GetAllFormatted(InvalidUserBodyException ex)
        {
            var response = ex.response;
            var errs = response.Errors;
            string desc = string.Empty;
            foreach (FieldError i in errs)
            {
                desc += (i.Message + '\n');
            }

            return desc;
        }
    }
}
