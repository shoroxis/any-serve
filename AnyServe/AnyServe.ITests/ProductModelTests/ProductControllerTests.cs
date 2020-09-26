﻿using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using Newtonsoft.Json;
using System.Text;

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

        [Theory]
        [InlineData("/api/product")]
        public async Task POST_AddsItemCorrect(string url)
        {
            // Arrange (test preparation)
            var product = new Product()
            {
                id = Guid.NewGuid(),
                Name = "TestProductName",
                Description = "Test product description"
            };
            var json = JsonConvert.SerializeObject(product);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var urlWithId = url + "/" + product.id;

            // Act
            var response = await Client.PostAsync(urlWithId, data);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            //Try get stored product
            response = await Client.GetAsync(urlWithId);
            var addedProduct = JsonConvert.DeserializeObject<Product>(await response.Content.ReadAsStringAsync());
            Assert.Equal(product.id, addedProduct.id);
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
