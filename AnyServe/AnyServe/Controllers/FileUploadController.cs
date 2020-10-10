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
            return Ok("Hello");
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


                FileModel file = new FileModel { Name = fileName, Path = fullPath, Id = id };

                return Ok("File uploaded successfully");

            }


            //TODO
            // Need to deside what return then file is not in Form
            return Ok("Unable to upload file");
        }


        // Multiple files upload
        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFile( List<IFormFile> uploads)
        {
            if (uploads != null)
            {
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
                    }


                    FileModel file = new FileModel { Name = fileName, Path = fullPath, Id = id };
                    //TODO: Need do save file info to DB
                }

                // TODO
                // Need to check if files are saved
                return Ok("All files uploaded successfully");
            }

            //TODO
            // Need to deside what return then file is not in Form
            return Ok("Unable to upload one or more files");
        }
    }
}
