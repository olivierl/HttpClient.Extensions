using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttpClient.Extensions.Constants;
using HttpClient.Extensions.Tests.Fixtures;
using RichardSzalay.MockHttp;
using Xunit;

namespace HttpClient.Extensions.Tests.Extensions
{
    public class HttpResponseMessageExtensionsTests
    {
        [Fact]
        public async Task ReadJsonAsync_ReturnsTodoItem()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, "http://example.com/todos/1")
                .Respond(HttpStatusCode.OK, ContentTypes.Json, "{\"id\":1,\"title\":\"Remember the Milk\",\"status\":\"completed\"}");
            var httpClient = mockHttp.ToHttpClient();
            
            var response = await httpClient.GetAsync("http://example.com/todos/1");

            Assert.True(response.IsSuccessStatusCode);

            var todo = await response.ReadJsonAsync<Todo>();

            Assert.Equal(1, todo.Id);
            Assert.Equal("Remember the Milk", todo.Title);
            Assert.Equal("completed", todo.Status);
        }

        [Fact]
        public async Task ReadJsonAsync_ReturnsNullWhenNoContent()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, "http://example.com/todos/1")
                .Respond(HttpStatusCode.NoContent);
            var httpClient = mockHttp.ToHttpClient();
            
            var response = await httpClient.GetAsync("http://example.com/todos/1");

            Assert.True(response.IsSuccessStatusCode);

            var todo = await response.ReadJsonAsync<Todo>();

            Assert.Null(todo);
        }

        [Fact]
        public async Task ReadJsonAsync_ThrowsExceptionWhenXml()
        {
            const string contentType = "text/xml";

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, "http://example.com/todos/1")
                .Respond(HttpStatusCode.OK, contentType, "<hello></hello>");
            var httpClient = mockHttp.ToHttpClient();
            
            var response = await httpClient.GetAsync("http://example.com/todos/1");

            Assert.True(response.IsSuccessStatusCode);

            var exception = await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await response.ReadJsonAsync<Todo>();
            });

            Assert.Matches($"Content type \"{contentType}\" not supported", exception.Message);
        }
    }
}
