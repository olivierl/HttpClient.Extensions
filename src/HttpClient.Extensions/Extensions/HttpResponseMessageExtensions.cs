using System.Threading.Tasks;
using HttpClient.Extensions.Constants;
using HttpClient.Extensions.Serialization;

// ReSharper disable once CheckNamespace
namespace System.Net.Http
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<T> ReadJsonAsync<T>(this HttpResponseMessage httpResponseMessage) where T : class
        {
            var content = httpResponseMessage.Content;

            if(content == null)
                return null;

            var contentType = content.Headers.ContentType.MediaType;
            if(!contentType.Contains(ContentTypes.Json))
                throw new HttpRequestException($"Content type \"{contentType}\" not supported");

            var json = await httpResponseMessage.Content.ReadAsStringAsync();
            var jsonDeserializer = new JsonDeserializer();

            return jsonDeserializer.Deserialize<T>(json);
        }
    }
}