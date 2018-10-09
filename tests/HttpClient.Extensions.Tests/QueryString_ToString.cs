using Xunit;

namespace HttpClient.Extensions.Tests
{
    // ReSharper disable once InconsistentNaming
    public class QueryString_ToString
    {
        [Fact]
        public void ReturnsTheQueryStringUrlEncoded()
        {
            var queryString = new QueryString();

            queryString.Add("param1", "a value");
            queryString.Add("param2", 3.ToString());
            queryString.Add("param2", 4.ToString());
            queryString.Add("param3", "50% charged");

            var result = queryString.ToString();

            Assert.Equal("param1=a+value&param2=3&param2=4&param3=50%25+charged", result);
        }

        [Fact]
        public void ReturnsEmptyStringWhenEmpty()
        {
            var quesryString = new QueryString();

            Assert.Equal(string.Empty, quesryString.ToString());
        }
    }
}