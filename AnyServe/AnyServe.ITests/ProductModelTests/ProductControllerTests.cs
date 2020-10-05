using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using Newtonsoft.Json;
using System.Text;
using AnyServe.ITests.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace AnyServe.ITests
{
    public class ProductControllerTests : IDisposable
    {
        private IConfigurationRoot _config;
        private TestServer _server;
        public HttpClient Client { get; private set; }

        public ProductControllerTests()
        {
            _config = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", false, true)
                        .Build();
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

        [Theory]
        [InlineData("/api/product")]
        public async Task POST_AddsItemCorrect(string url)
        {
            // Arrange (test preparation)
            var product = new Product()
            {
                Id = Guid.NewGuid(),
                Name = "TestProductName",
                Description = "Test product description"
            };
            var json = JsonConvert.SerializeObject(product);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var urlWithId = url + "/" + product.Id;

            // Act
            var response = await Client.PostAsync(urlWithId, data);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            //Try get stored product
            response = await Client.GetAsync(urlWithId);
            var addedProduct = JsonConvert.DeserializeObject<Product>(await response.Content.ReadAsStringAsync());
            Assert.Equal(product.Id, addedProduct.Id);
        }

        [Theory]
        [InlineData("/api/product")]
        public async Task Delete_ItemWithExistingId_ReturnsSuccess(string url)
        {
            // Arrange (test preparation)
            var product = new Product()
            {
                Id = Guid.NewGuid(),
                Name = "TestProductName",
                Description = "Test product description"
            };
            var json = JsonConvert.SerializeObject(product);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var urlWithId = url + "/" + product.Id;

            // Act
            var response = await Client.PostAsync(urlWithId, data);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            //Try get stored product
            response = await Client.GetAsync(urlWithId);
            var addedProduct = JsonConvert.DeserializeObject<Product>(await response.Content.ReadAsStringAsync());
            Assert.Equal(product.Id, addedProduct.Id);

            //Try Delete stored product
            response = await Client.DeleteAsync(urlWithId);

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal( System.Net.HttpStatusCode.OK, response.StatusCode);

            //Try get stored product after deleting 
            response = await Client.GetAsync(urlWithId);
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }


        [Theory]
        [InlineData("/api/product")]
        public async Task Delete_ItemWithNotExistingId_ReturnsSuccess_NoContent(string url)
        {
            // Arrange 
            var urlWithId = url + "/" + Guid.NewGuid();

            // Act
            var response = await Client.DeleteAsync(urlWithId);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);

        }

        #endregion

        #region Dispose implementation

        //Dispose used to clean up resources after tests finished
        public void Dispose()
        {
            //Delete all rows from Product
            var connectionString = _config.GetConnectionString("ProductsConnectionSqlite");
            using (var con = new SqliteConnection(connectionString))
            {
                con.Open();
                var deleteAllRows = con.CreateCommand();
                deleteAllRows.CommandText = "DELETE FROM Product";
                deleteAllRows.ExecuteNonQuery();
            }
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
