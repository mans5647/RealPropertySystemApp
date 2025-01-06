using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RealPropertySystemApp.bodies
{
    public class FieldError
    {
        private string _field;
        private string _code;
        private string _message;

        [JsonPropertyName("field")]
        public string Field { get { return _field; } set { _field = value; } }
        [JsonPropertyName("code")]
        public string Code { get { return _code; } set { _code = value; } }
        [JsonPropertyName("message")]
        public string Message { get { return _message; } set { _message = value; } }

        public FieldError() { }

    }
}
