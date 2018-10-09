using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttpClient.Extensions.Constants;
using HttpClient.Extensions.Serialization;
using HttpClient.Extensions.Tests.Fixtures;
using RichardSzalay.MockHttp;
using Xunit;

namespace HttpClient.Extensions.Tests.Extensions
{
    public class HttpClientExtensionsTests
    {
        private readonly Todo _todo;
        private readonly QueryString _queryString;
        private readonly Dictionary<string, string> _headers;

        private readonly JsonSerializer _jsonSerializer;

        public HttpClientExtensionsTests()
        {
            _todo = new Todo { Id = 1, Title = "Remeber the Milk", Status = "completed" };
            _queryString = new QueryString
            {
                { "status", "completed" }
            };
            _headers = new Dictionary<string, string>
            {
                { "X-Todo-ApiKey", "123" }
            };
            _jsonSerializer = new JsonSerializer();
        }

        [Fact]
        public async Task SetBasicAuthentication_ShouldAddAuthorizationHeaderWithBase64EncodedCredentials()
        {
            const string username = "admin";
            const string password = "pasword";
            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            var expectedAuthHeader = $"Basic {encodedCredentials}";

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, "http://example.com/secret")
                .WithHeaders("Authorization", expectedAuthHeader)
                .Respond(HttpStatusCode.OK);
            var httpClient = mockHttp.ToHttpClient();

            httpClient.SetBasicAuthentication(username, password);

            var response = await httpClient.GetAsync("http://example.com/secret");

            Assert.True(response.IsSuccessStatusCode);

            mockHttp.VerifyNoOutstandingExpectation();
            mockHttp.VerifyNoOutstandingRequest();
        }

        [Fact]
        public async Task SetBearerToken_ShouldAddAuthorizationHeaderWithToken()
        {
            const string token = "abc123";
            var expectedAuthHeader = $"Bearer {token}";

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, "http://example.com/secret")
                .WithHeaders("Authorization", expectedAuthHeader)
                .Respond(HttpStatusCode.OK);
            var httpClient = mockHttp.ToHttpClient();

            httpClient.SetBearerToken(token);

            var response = await httpClient.GetAsync("http://example.com/secret");

            Assert.True(response.IsSuccessStatusCode);

            mockHttp.VerifyNoOutstandingExpectation();
            mockHttp.VerifyNoOutstandingRequest();
        }

        [Fact]
        public async Task GetAsync_ShouldReturnFirstTodo()
        {
            var todo = new Todo { Id = 1, Title = "Remeber the Milk", Status = "completed" };
            var todoJson = _jsonSerializer.Serialize(todo);

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, "http://example.com/todos/1")
                    .WithQueryString("status", "completed")
                    .WithHeaders("X-Todo-ApiKey", "123")
                    .Respond(HttpStatusCode.OK, ContentTypes.Json, todoJson);
            var httpClient = mockHttp.ToHttpClient();

            var queryString = new QueryString
            {
                {"status", "completed" }
            };
            var headers = new Dictionary<string, string>
            {
                { "X-Todo-ApiKey", "123" }
            };
            var response = await httpClient.GetAsync("http://example.com/todos/1", queryString, headers);

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.ReadJsonAsync<Todo>();

            Assert.NotStrictEqual(todo, result);

            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Theory]
        [InlineData("http://example.com/todos", "http://example.com", "/todos")]
        [InlineData("http://example.com/todos", "http://example.com/", "/todos")]
        [InlineData("http://example.com/todos/1/comments", "http://example.com/todos", "/1/comments")]
        public async Task GetAsync_HandleFullAndPartialUrl(string expectedResult, string host, string path)
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, expectedResult)
                .WithQueryString("status", "completed")
                .Respond(HttpStatusCode.OK);
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri(host);

            var response = await httpClient.GetAsync(path, _queryString);

            Assert.True(response.IsSuccessStatusCode);

            mockHttp.VerifyNoOutstandingExpectation();
            mockHttp.VerifyNoOutstandingRequest();
        }

        private MockHttpMessageHandler MockHttp(HttpMethod method, HttpStatusCode statusCode)
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(method, "http://example.com/todos/1")
                .WithQueryString("status", "completed")
                .WithHeaders(new Dictionary<string, string>
                {
                    { "X-Todo-ApiKey", "123" },
                    { "Content-Type", "application/json; charset=utf-8" }
                })
                .WithContent(_jsonSerializer.Serialize(_todo))
                .Respond(statusCode);

            return mockHttp;
        }

        [Fact]
        public async Task PostAsync_SendJsonSerializedTodo()
        {
            const HttpStatusCode statusCode = HttpStatusCode.Created;

            var mockHttp = MockHttp(HttpMethod.Post, statusCode);
            var httpClient = mockHttp.ToHttpClient();

            var response = await httpClient.PostAsync("http://example.com/todos/1", _todo, _queryString, _headers);

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(statusCode, response.StatusCode);

            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task PuAsync_SendJsonSerializedTodo()
        {
            const HttpStatusCode statusCode = HttpStatusCode.OK;

            var mockHttp = MockHttp(HttpMethod.Put, statusCode);
            var httpClient = mockHttp.ToHttpClient();

            var response = await httpClient.PutAsync("http://example.com/todos/1", _todo, _queryString, _headers);

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(statusCode, response.StatusCode);

            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task PatchAsync_SendJsonSerializedTodo()
        {
            const HttpStatusCode statusCode = HttpStatusCode.OK;

            var mockHttp = MockHttp(new HttpMethod("PATCH"), statusCode);
            var httpClient = mockHttp.ToHttpClient();

            var response = await httpClient.PatchAsync("http://example.com/todos/1", _todo, _queryString, _headers);

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(statusCode, response.StatusCode);

            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task DeleteAsync_SendJsonSerializedTodo()
        {
            const HttpStatusCode statusCode = HttpStatusCode.NoContent;

            var mockHttp = MockHttp(HttpMethod.Delete, statusCode);
            var httpClient = mockHttp.ToHttpClient();

            var response = await httpClient.DeleteAsync("http://example.com/todos/1", _todo, _queryString, _headers);

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(statusCode, response.StatusCode);

            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task DeleteAsync_SendRequestWithoutContent()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Delete, "http://example.com/todos/1")
                .WithQueryString("status", "completed")
                .WithHeaders("X-Todo-ApiKey", "123")
                .Respond(HttpStatusCode.NoContent);
            var httpClient = mockHttp.ToHttpClient();

            var response = await httpClient.DeleteAsync("http://example.com/todos/1", _queryString, _headers);

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            mockHttp.VerifyNoOutstandingExpectation();
        }
    }
}