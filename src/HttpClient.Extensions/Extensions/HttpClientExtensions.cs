using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HttpClient.Extensions;
using HttpClient.Extensions.Constants;
using HttpClient.Extensions.Serialization;

// ReSharper disable once CheckNamespace
namespace System.Net.Http
{
    public static class HttpClientExtensions
    {   
        public static void SetBasicAuthentication(this HttpClient httpClient, string username, string password)
        {
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        }

        public static void SetBearerToken(this HttpClient httpClient, string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        
        public static async Task<HttpResponseMessage> GetAsync(this HttpClient httpClient, string url,
            QueryString queryString = null, IDictionary<string, string> headers = null)
        {
            var request = httpClient.CreateRequest(HttpMethod.Get, url, queryString, headers);

            return await httpClient.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> PostAsync<T>(this HttpClient httpClient, string url, T body,
            QueryString queryString = null, IDictionary<string, string> headers = null) where T : class
        {
            var request = httpClient.CreateRequest(HttpMethod.Post, url, queryString, headers);
            request.Content = CreateContent(body);

            return await httpClient.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> PatchAsync<T>(this HttpClient httpClient, string url, T body,
            QueryString queryString = null, IDictionary<string, string> headers = null) where T : class
        {
            var request = httpClient.CreateRequest(new HttpMethod("PATCH"), url, queryString, headers);
            request.Content = CreateContent(body);

            return await httpClient.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> PutAsync<T>(this HttpClient httpClient, string url, T body,
            QueryString queryString = null, IDictionary<string, string> headers = null) where T : class
        {
            var request = httpClient.CreateRequest(HttpMethod.Put, url, queryString, headers);
            request.Content = CreateContent(body);

            return await httpClient.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> DeleteAsync(this HttpClient httpClient, string url,
            QueryString queryString = null, IDictionary<string, string> headers = null)
        {
            var request = httpClient.CreateRequest(HttpMethod.Delete, url, queryString, headers);

            return await httpClient.SendAsync(request);
        }

        public static async Task<HttpResponseMessage> DeleteAsync<T>(this HttpClient httpClient, string url, T body,
            QueryString queryString = null, IDictionary<string, string> headers = null) where T : class
        {
            var request = httpClient.CreateRequest(HttpMethod.Delete, url, queryString, headers);
            request.Content = CreateContent(body);

            return await httpClient.SendAsync(request);
        }

        private static string BuildFullUrl(this HttpClient httpClient, string url, QueryString queryString = null)
        {
            var fullUrl = new StringBuilder();

            if (httpClient.BaseAddress != null)
                fullUrl.Append(httpClient.BaseAddress.OriginalString.TrimEnd('/')).Append('/');

            fullUrl.Append(url.TrimStart('/'));

            if (queryString == null)
                return fullUrl.ToString();

            fullUrl.Append("?");
            fullUrl.Append(queryString);

            return fullUrl.ToString();
        }

        private static HttpRequestMessage CreateRequest(this HttpClient httpClient, HttpMethod httpMethod, string url,
            QueryString queryString = null, IDictionary<string, string> headers = null)
        {
            var fullUrl = httpClient.BuildFullUrl(url, queryString);

            var request = new HttpRequestMessage(httpMethod, fullUrl);

            if (headers == null)
                return request;

            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);

            return request;
        }

        private static HttpContent CreateContent<T>(T body)
        {
            var jsonSerializer = new JsonSerializer();
            var json = jsonSerializer.Serialize(body);

            return new StringContent(json, Encoding.UTF8, ContentTypes.Json);
        }
    }
}