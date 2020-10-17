using System;
using System.Collections.Generic;
using System.Linq;
using AnyServe.Models;
using System.IO;
using System.Threading.Tasks;
using AnyServe.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Collections;

namespace AnyServe.Controllers
{
    [Route("api/[controller]")]
    public class MediaController : Controller
    {
        //private Storage<T> _storage;
        //private readonly ILogger<BaseController<T>> _logger;
        private IWebHostEnvironment _appEnvironment;

        public MediaController(/*Storage<T> storage, ILogger<BaseController<T>> logger,*/ IWebHostEnvironment appEnviroment)
        {
            //_storage = storage;
            //_logger = logger;
            //_logger.LogInformation("UploadController created");
            _appEnvironment = appEnviroment;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string filesPath = _appEnvironment.WebRootPath + "\\Files";

            return Ok(AllFilesInWebRootPath(filesPath));
        }

        // Single file upload
        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                string[] splitedName = (uploadedFile.FileName).Split('.');
                string fileExtansion = "." + splitedName[1];

                Guid id = Guid.NewGuid();

                // path to folder Files 
                //@"./Files/" path from run executing

                string partialPath = @"\Files\";
                if (!Directory.Exists(_appEnvironment.WebRootPath + partialPath))
                    Directory.CreateDirectory(_appEnvironment.WebRootPath + partialPath);

                string fileName = id + fileExtansion;
                string fullPath = partialPath + fileName;

                // save file to folder Files in catalog wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + fullPath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                //add following instance to db
                FileModel file = new FileModel { Name = fileName, Path = fullPath, Id = id };
             
                return Ok(new FileModelResponse(file));

            }


            //TODO
            // Need to deside what return then file is not in Form
            return Ok("Unable to upload file");
        }


        // Multiple files upload
        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFile(List<IFormFile> uploads)
        {
            if (uploads != null)
            {
                List<FileModel> fileToDB = new List<FileModel>();
                List<FileModelResponse> filesResponse = new List<FileModelResponse>();
                foreach (var uploadedFile in uploads)
                {
                    string[] splitedName = (uploadedFile.FileName).Split('.');
                    string fileExtansion = "." + splitedName[1];

                    Guid id = Guid.NewGuid();
                    
                    // path to folder Files 
                    //@"./Files/" path from run executing

                    string partialPath = @"\Files\";
                    if (!Directory.Exists(_appEnvironment.WebRootPath + partialPath))
                        Directory.CreateDirectory(_appEnvironment.WebRootPath + partialPath);

                    string fileName = id + fileExtansion;
                    string fullPath = partialPath + fileName;

                    // save file to folder Files in catalog wwwroot
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + fullPath, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);

                        var newFile = new FileModel { Name = fileName, Path = fullPath, Id = id, OriginalName = uploadedFile.FileName };
                        
                        //Adding to file for DB
                        fileToDB.Add(newFile);

                        //Adding file to response list
                        filesResponse.Add(new FileModelResponse(newFile));
                    }

                    //TODO: Need do save file info to DB
                }

                // TODO
                // Need to check if files are saved
                return Ok(filesResponse);
            }

            //TODO
            // Need to deside what return then file is not in Form
            return Ok("Unable to upload one or more files");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            if (id != null)
            {
                bool isFileDeleted = false;

                // To convert to async fanction
                await Task.FromResult( isFileDeleted = PhysicalDeleteFile(id) );//"51bc656f-4bbc-496e-83d2-6b7bc521a583"

                if (isFileDeleted)
                    return Ok();
            }

            return NotFound();
        }


        #region private function for HttpDelete
        //Function created for Task
        private bool PhysicalDeleteFile(Guid id)
        {
            //Colect all files in directory
            var allFilesInFolder = Directory.EnumerateFiles(_appEnvironment.WebRootPath + "\\Files");

            //Find file with id in name
            var fileToDelete = allFilesInFolder.FirstOrDefault(f => Path.GetFileName(f).Contains(id.ToString()));

            //Delete file
            if (fileToDelete != null)
            {
                System.IO.File.Delete(fileToDelete);
                return true;
            }

            return false;
        }

        #endregion

        #region private function for HttpGet
        private IEnumerable AllFilesInWebRootPath(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
                //return Enamerable string of files
                return Directory.EnumerateFiles(directoryPath).Select(Path.GetFileName);

            return null;
        }

        #endregion
    }
}
