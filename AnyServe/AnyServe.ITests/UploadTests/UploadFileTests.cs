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
using AnyServe.ITests.UploadTests;
using System.Text;
using Microsoft.Extensions.Configuration;
using AnyServe.ITests.Helpers;

namespace AnyServe.ITests
{
    public class UploadFileTests
    {

        private TestServer _server;
        private AnyServUser _admin;
        private AnyServUser _user;
        private IConfigurationRoot _config;

        public HttpClient Client { get; private set; }

        public UploadFileTests()
        {
            _config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", false, true)
            .Build();

            _admin = new AnyServUser()
            {
                UserName = "admin",
                Password = "1234",
            };

            _user = new AnyServUser()
            {
                UserName = "user",
                Password = "1234",
            };

            SetUpClient();
        }

        #region Tests

        [Theory]
        [InlineData("api/Media")]
        public async Task GetAll_ReturnExistedFilesAndSuccess(string url)
        {
            // Arrange (test preparation)
            string token = await GetToken(_admin);
            HttpResponseMessage response;

            // Act
            var getRequest = new HttpRequestMessage(HttpMethod.Get, url);
            getRequest.Headers.Add("Authorization", $"Bearer {token}");
            response = await Client.SendAsync(getRequest);
            

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            //No files exists
            Assert.Empty(JsonConvert.DeserializeObject<IEnumerable<FileModelResponse>>(await response.Content.ReadAsStringAsync()));

            //Upload single file
            using (var file1 = File.OpenRead(ConstantString.filesToUploadPath + ConstantString.singleFile))
            using (var content1 = new StreamContent(file1))
            using (var formData = new MultipartFormDataContent())
            {
                // Add file (file, field name, file name)
                formData.Add(content1, "upload", ConstantString.singleFile);

                var postRequest = new HttpRequestMessage(HttpMethod.Post, ConstantString.urlSingleItemUpload);
                postRequest.Headers.Add("Authorization", $"Bearer {token}");
                postRequest.Content = formData;
                response = await Client.SendAsync(postRequest);
            }

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var expectedContentType = "application/json; charset=utf-8";
            Assert.Equal(expectedContentType, response.Content.Headers.ContentType.ToString());

            var singleFile = JsonConvert.DeserializeObject<FileModelResponse>(await response.Content.ReadAsStringAsync());

            //Checking not existed file name
            Assert.False(singleFile.OriginalName == ConstantString.fileNameNotExist);

            //Checking what file was uploaded
            Assert.True(singleFile.OriginalName == ConstantString.singleFile && singleFile.Id != null);


            //Delete file to keep folder clear
            response = await Client.DeleteAsync(ConstantString.urlDelete + singleFile.Id);

            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            //Checking delete response then file not found
            response = await Client.DeleteAsync(ConstantString.urlDelete + singleFile.Id);

            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);

            //Checking delete response then file not found
            //Check if no file exist
            response = await Client.GetAsync(ConstantString.urlGET);


            Assert.Empty(JsonConvert.DeserializeObject<IEnumerable<FileModelResponse>>(await response.Content.ReadAsStringAsync()));

        }


        [Theory]
        [InlineData("/api/Media/uploadfile")]
        //Description:
        //That test checking upload mechanism for single file and then delete it from host
        public async Task Upload_SingleFileAndReturnSuccess(string url)
        {
            // Arrange (test preparation)
            string token = await GetToken(_admin);
            var expectedContentType = "application/json; charset=utf-8";

            // Act
            HttpResponseMessage response;

            using (var file1 = File.OpenRead(ConstantString.filesToUploadPath + ConstantString.singleFile))
            using (var content1 = new StreamContent(file1))
            using (var formData = new MultipartFormDataContent())
            {
                // Add file (file, field name, file name)
                formData.Add(content1, "upload", ConstantString.singleFile);

                var postRequest = new HttpRequestMessage(HttpMethod.Post, url);
                postRequest.Headers.Add("Authorization", $"Bearer {token}");
                postRequest.Content = formData;
                response = await Client.SendAsync(postRequest);
            }

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            Assert.Equal(expectedContentType, response.Content.Headers.ContentType.ToString());

            var singleFile = JsonConvert.DeserializeObject<FileModelResponse>(await response.Content.ReadAsStringAsync());
            
            //Checking not existed file name
            Assert.False(singleFile.OriginalName == ConstantString.fileNameNotExist);

            //Checking what file was uploaded
            Assert.True(singleFile.OriginalName == ConstantString.singleFile && singleFile.Id != null);


            //Delete file to keep folder clear
            response = await Client.DeleteAsync(ConstantString.urlDelete + singleFile.Id);

            response.EnsureSuccessStatusCode();
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            //Checking delete response then file not found
            response = await Client.DeleteAsync(ConstantString.urlDelete + singleFile.Id);

            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);

            //Checking delete response then file not found
            //Check if no file exist
            response = await Client.GetAsync(ConstantString.urlGET);


            Assert.Empty(JsonConvert.DeserializeObject<IEnumerable<FileModelResponse>>(await response.Content.ReadAsStringAsync()));

        }

        [Theory]
        [InlineData("/api/Media/uploadfiles")]
        public async Task Upload_ThreeFilesAndReturnSuccessThenDeleteAllFilesAndCheckIfAllDeleted(string url)
        {
            // Arrange (test preparation)
            string[] listFileName = { "text_1.txt", "text_2.txt", "text_3.txt" };
            var expectedContentType = "application/json; charset=utf-8";

            // Act
            HttpResponseMessage response;

            using (var file1 = File.OpenRead(ConstantString.filesToUploadPath + ConstantString.listFileName[0]))
            using (var content1 = new StreamContent(file1))
            using (var file2 = File.OpenRead(ConstantString.filesToUploadPath + ConstantString.listFileName[1]))
            using (var content2 = new StreamContent(file2))
            using (var file3 = File.OpenRead(ConstantString.filesToUploadPath + ConstantString.listFileName[2]))
            using (var content3 = new StreamContent(file3))
            using (var formData = new MultipartFormDataContent())
            {
                // Add file (file, field name, file name)
                formData.Add(content1, "uploads", listFileName[0]);
                formData.Add(content2, "uploads", listFileName[1]);
                formData.Add(content3, "uploads", listFileName[2]);

                response = await Client.PostAsync(url, formData);
            }
            
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            Assert.Equal(expectedContentType, response.Content.Headers.ContentType.ToString());

            var listOfFiles = JsonConvert.DeserializeObject<IEnumerable<FileModelResponse>>(await response.Content.ReadAsStringAsync());

            //Checking not existed file name
            Assert.Null(listOfFiles.FirstOrDefault(f => f.OriginalName == ConstantString.fileNameNotExist));

            //Checking what all files were uploaded
            foreach(string fileName in listFileName)
                Assert.NotNull(listOfFiles.FirstOrDefault(f => (f.OriginalName == fileName && f.Id != null)));


            int expectedFileNumbers = 3;

            //Checkin number of files 
            Assert.Equal(expectedFileNumbers, listOfFiles.Count<FileModelResponse>());

            //Deleting all files to keep folder clear
            foreach (var file in listOfFiles)
            {

                response = await Client.DeleteAsync(ConstantString.urlDelete + file.Id);

                response.EnsureSuccessStatusCode();
                Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

                //Checking delete response then file not found
                response = await Client.DeleteAsync(ConstantString.urlDelete + file.Id);

                //response.EnsureSuccessStatusCode();
                Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            }

            //Checking delete response then file not found
            //Check if no file exist
            response = await Client.GetAsync(ConstantString.urlGET);

            Assert.Empty(JsonConvert.DeserializeObject<IEnumerable<FileModelResponse>>(await response.Content.ReadAsStringAsync()));

        }

        #endregion

        #region Private Methods

        private void SetUpClient()
        {
            _server = new TestServer(new WebHostBuilder()
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseWebRoot("wwwroot")
                        .UseConfiguration(_config)
                        .UseStartup<Startup>());
            _server.BaseAddress = new Uri(@"https://localhost:5001/");

            Client = _server.CreateClient();
        }

        private async Task<string> GetToken(AnyServUser credentials)
        {
            var json = JsonConvert.SerializeObject(credentials);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(ConstantString.LoginUrl, data);
            response.EnsureSuccessStatusCode();

            UserTokenModel responseModel = JsonConvert.DeserializeObject<UserTokenModel>(await response.Content.ReadAsStringAsync());
            return responseModel.Token;
        }

        #endregion
    }
}

