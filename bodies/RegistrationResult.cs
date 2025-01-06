using RealPropertySystemApp.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPropertySystemApp.bodies
{
    public class RegistrationResult
    {

        private int _code;
        private List<FieldError>? _errors;
        private UserModel? _registeredUser;
        private string? _optionalMessage;
        private bool _valid;

        public int Code
        {
            get => _code; set => _code = value;

        }

        public List<FieldError> Errors
        {
            get => _errors; set => _errors = value;
        }

        public UserModel RegisteredUser
        {
            get => _registeredUser ; set => _registeredUser = value;
        }

        public string OptionalMessage
        {
            get => _optionalMessage; set => _optionalMessage = value;
        }

        public bool IsValid
        {
            get => _valid; set => _valid = value;
        }

        public RegistrationResult(bool valid)
        {
            this._valid = valid;
        }

        public static RegistrationResult Invalid()
        {
            return new RegistrationResult(false);
        }

    }
}
