using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RealPropertySystemApp.bodies
{
    public class ValidationResponse
    {

        private MessageMeta _meta;
        private List<FieldError> _errors;

        [JsonPropertyName("meta")]
        public MessageMeta Meta { get { return _meta; } set => _meta = value; }

        [JsonPropertyName("errors")]
        public List<FieldError> Errors
        {
            get => _errors;
            set => _errors = value;
        }

        public ValidationResponse() { }

    }
}
