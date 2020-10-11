using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System;

namespace AnyServe.ITests
{
    public class UploadFileTets
    {

        private TestServer _server;
        public HttpClient Client { get; private set; }

        public UploadFileTets()
        {
            SetUpClient();
        }

        #region Tests

        [Theory]
        [InlineData("/api/Media/uploadfiles")]
        public async Task Upload_SavesFileAndReturnSuccess(string url)
        {
            // Arrange (test preparation)
            var expectedContentType = "text/plan; charset=utf-8";

            // Act
            HttpResponseMessage response;

            using (var file1 = File.OpenRead(@"UploadTests\TestFiles\text_1.txt"))
            using (var content1 = new StreamContent(file1))
            using (var file2 = File.OpenRead(@"UploadTests\TestFiles\text_2.txt"))
            using (var content2 = new StreamContent(file2))
            using (var file3 = File.OpenRead(@"UploadTests\TestFiles\text_3.txt"))
            using (var content3 = new StreamContent(file3))
            using (var formData = new MultipartFormDataContent())
            {
                // Add file (file, field name, file name)
                formData.Add(content1, "uploads", "text_1.txt");
                formData.Add(content2, "uploads", "text_2.txt");
                formData.Add(content3, "uploads", "text_3.txt");

                response = await Client.PostAsync(url, formData);
            }
            
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var responseString = await response.Content.ReadAsStringAsync();

            //Guid id = Guid.NewGuid();
            var urlDelete = "/api/Media/" + "51bc656f-4bbc-496e-83d2-6b7bc521a583";
            response = await Client.DeleteAsync(urlDelete);
            //Assert.NotEmpty(responseString);
            Assert.Equal(expectedContentType, response.Content.Headers.ContentType.ToString());
            //Assert.Equal("application/json; charset=utf-8",
                //response.Content.Headers.ContentType.ToString());
        }

        #endregion

        #region Private Methods

        private void SetUpClient()
        {
            _server = new TestServer(new WebHostBuilder()
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseWebRoot("wwwroot")
                        .UseStartup<Startup>());

            Client = _server.CreateClient();
        }

        #endregion
    }
}

