using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace HttpClient.Extensions.Serialization
{
    public class JsonSerializer : ISerializer
    {
        private readonly JsonSerializerSettings _settings;

        public JsonSerializer(JsonSerializerSettings settings = null)
        {
            _settings = settings ?? new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new StringEnumConverter() }
            };
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, _settings);
        }
    }
}