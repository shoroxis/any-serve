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
                // path to folder Files
                string path = "/Files/" + uploadedFile.FileName;
                // save file to folder Files in catalog wwwroot
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                FileModel file = new FileModel { Name = uploadedFile.FileName, Path = path, id = new Guid() };


                //The request has been accepted for processing, but the processing has not been completed. 
                //The request might or might not eventually be acted upon, as it might be disallowed when processing actually takes place. 
                //There is no facility for re-sending a status code from an asynchronous operation such as this.
                return Accepted();

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
                    // path to folder Files
                    string path = "/Files/" + uploadedFile.FileName;
                    // save file to folder Files in catalog wwwroot
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }


                    FileModel file = new FileModel { Name = uploadedFile.FileName, Path = path, id = new Guid() };

                }

                // TODO
                // Need to check if files are saved
                return Accepted();
            }

            //TODO
            // Need to deside what return then file is not in Form
            return Ok("Unable to upload one or more files");
        }
    }
}
