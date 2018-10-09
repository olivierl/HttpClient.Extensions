using HttpClient.Extensions.Serialization;
using HttpClient.Extensions.Tests.Fixtures;
using Xunit;

namespace HttpClient.Extensions.Tests.SerializationTests
{
    // ReSharper disable once InconsistentNaming
    public class JsonDeserializer_Deserialize
    {
        [Fact]
        public void ShouldReturnDefaultWhenJsonIsNull()
        {
            var jsonDeserializer = new JsonDeserializer();

            var todo = jsonDeserializer.Deserialize<Todo>(null);

            Assert.Null(todo);
        }
    }
}
