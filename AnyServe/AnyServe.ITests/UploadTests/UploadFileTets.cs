using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using AnyServe.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
        [InlineData("/api/Media")]
        public async Task GetAll_ReturnExistedFilesAndSuccess(string url)
        {
            // Arrange (test preparation)
            var expectedContentType = "application/json; charset=utf-8";

            // Act
            HttpResponseMessage response;

            response = await Client.GetAsync(url);
            

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var responseString = await response.Content.ReadAsStringAsync();


            //Assert.NotEmpty(responseString);
            Assert.Equal(expectedContentType, response.Content.Headers.ContentType.ToString());
            //Assert.Equal("application/json; charset=utf-8",
            //response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/api/Media/uploadfiles")]
        public async Task Upload_SavesFileAndReturnSuccess(string url)
        {
            // Arrange (test preparation)
            string[] listFileName = { "text_1.txt", "text_2.txt", "text_3.txt" };
            var expectedContentType = "application/json; charset=utf-8";

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
            var listOfFiles = JsonConvert.DeserializeObject<IEnumerable<FileModelResponse>>(await response.Content.ReadAsStringAsync());

            //Checking not existed file name
            Assert.Null(listOfFiles.FirstOrDefault(f => f.OriginalName == "text_222.txt"));

            //Checking what all files were uploaded
            foreach(string fileName in listFileName)
                Assert.NotNull(listOfFiles.FirstOrDefault(f => (f.OriginalName == fileName && f.Id != null)));

            //TODO: Check response
            string urlGET = "/api/Media";

            response = await Client.GetAsync(urlGET);

            //Recive all files
            //var listOfFiles = JsonConvert.DeserializeObject<IEnumerable<string>>( await response.Content.ReadAsStringAsync());
            int expectedFileNumbers = 3;

            //Checkin number of files 
            //Assert.Equal(expectedFileNumbers, listOfFiles.Count<string>());//Have to FIX

            //Deleting all files to keep folder clear
            foreach (var file in listOfFiles)
            {
                var urlDelete = urlGET + "/" + file.Id;

                response = await Client.DeleteAsync(urlDelete);

                response.EnsureSuccessStatusCode();
                Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

                //Checking delete response then file not found
                response = await Client.DeleteAsync(urlDelete);

                //response.EnsureSuccessStatusCode();
                Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            }

            //Checking delete response then file not found
            //Check if no file exist
            response = await Client.GetAsync(urlGET);

            //responseString = await response.Content.ReadAsStringAsync();

           //TODO:FIX
            Assert.Empty(JsonConvert.DeserializeObject<IEnumerable<FileModelResponse>>(await response.Content.ReadAsStringAsync()));

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

