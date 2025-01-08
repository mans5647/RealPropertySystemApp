using JWT.Algorithms;
using JWT.Serializers;
using JWT;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPropertySystemApp.utils
{
    public static class JwtManager
    {
        static IJsonSerializer serializer = new JsonNetSerializer();
        static IDateTimeProvider provider = new UtcDateTimeProvider();
        static IJwtValidator validator = new JwtValidator(serializer, provider);
        static IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
        static IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
        static IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

        public static string GetPayloadProperty(string name, string token)
        {
            var json = decoder.Decode(token, false);

            JObject credits = JObject.Parse(json);

            return credits[name].ToObject<string>();

        }
        public static T GetPayloadPropertyAs<T>(string name, string token)
        {
            var json = decoder.Decode(token, false);

            JObject credits = JObject.Parse(json);

            return credits[name].ToObject<T>();
        }

        public static DateTime GetExpProperty(string token)
        {
            long secs = GetPayloadPropertyAs<long>("exp", token);

            return DateTimeOffset.FromUnixTimeSeconds(secs).ToLocalTime().DateTime;
        }
    }
}
