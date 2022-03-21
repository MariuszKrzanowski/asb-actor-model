using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;

namespace MrMatrix.Net.ActorOnServiceBus.Messages.Transforms
{
    internal static class JsonHelper
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Converters = { new StringEnumConverter(true) }
        };

        public static string SerializeObject(object o)
        {
            return JsonConvert.SerializeObject(o, _jsonSerializerSettings);
        }

        public static T DeserializeObject<T>(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(text, _jsonSerializerSettings);
        }

        public static object DeserializeObject(Type type, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            return JsonConvert.DeserializeObject(text, type, _jsonSerializerSettings);
        }
    }
}
