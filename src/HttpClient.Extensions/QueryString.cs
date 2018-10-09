using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace HttpClient.Extensions
{
    public class QueryString : Collection<KeyValuePair<string, string>>
    {
        public void Add(string key, string value)
        {
            Add(new KeyValuePair<string, string>(key, value));
        }

        public override string ToString()
        {
            var keys = this.Select(kv =>
            {
                var key = WebUtility.UrlEncode(kv.Key);
                var value = WebUtility.UrlEncode(kv.Value);
                return $"{key}={value}";
            }).ToArray();

            return string.Join("&", keys);
        }
    }
}