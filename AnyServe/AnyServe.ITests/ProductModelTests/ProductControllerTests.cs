using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AnyServe.ITests
{
    public class ProductControllerTests
    {
        private TestServer _server;
        public HttpClient Client { get; private set; }

        public ProductControllerTests()
        {
            SetUpClient();
        }


        #region Tests

        [Theory]
        [InlineData("/api/product")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange (test preparation)

            // Act
            var response = await Client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        #endregion


        #region Private Methods

        private void SetUpClient()
        {
            _server = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>());

            Client = _server.CreateClient();
        }

        #endregion
    }
}
