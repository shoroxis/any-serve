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
using AnyServe.Utility;

namespace AnyServe.Controllers
{
    [Route("api/[controller]")]
    public class MediaController : Controller
    {
        private IWebHostEnvironment _appEnvironment;

        public MediaController( IWebHostEnvironment appEnviroment)
        {
            _appEnvironment = appEnviroment;
        }

        [HttpGet]
        public IActionResult GetAllFiles()
        {
            string uploadsRootFolder = Path.Combine(_appEnvironment.WebRootPath, ConstantString.directoryName);
            if (!Directory.Exists(uploadsRootFolder))
                Directory.CreateDirectory(uploadsRootFolder);

            return Ok(AllFilesInWebRootPath(uploadsRootFolder));
        }

        // Single file upload
        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile upload)
        {
            if (upload != null && Path.GetExtension(upload.FileName) != null)
            {
                List<FileModel> fileToDB = new List<FileModel>();
                List<FileModelResponse> filesResponse = new List<FileModelResponse>();

                var uploadsRootFolder = Path.Combine(_appEnvironment.WebRootPath, ConstantString.directoryName);

                if (!Directory.Exists(uploadsRootFolder))
                    Directory.CreateDirectory(uploadsRootFolder);

                await UploadAndSave(upload, uploadsRootFolder, fileToDB, filesResponse);

                return Ok(filesResponse.First());

            }


            //TODO
            // Need to deside what return then file is not in Form
            return BadRequest("Unable to upload file");
        }

        

        // Multiple files upload
        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFiles(List<IFormFile> uploads)
        {
            if (uploads != null)
            {
                List<FileModel> fileToDB = new List<FileModel>();
                List<FileModelResponse> filesResponse = new List<FileModelResponse>();

                //Path for wwwroot\Files
                var uploadsRootFolder = Path.Combine(_appEnvironment.WebRootPath, ConstantString.directoryName);

                if (!Directory.Exists(uploadsRootFolder))
                    Directory.CreateDirectory(uploadsRootFolder);

                foreach (var upload in uploads)
                {
                    await UploadAndSave(upload, uploadsRootFolder, fileToDB, filesResponse);

                    //TODO: Need do save file info to DB
                }

                // TODO
                // Need to check if files are saved
                return Ok(filesResponse);
            }

            //TODO
            // Need to deside what return then file is not in Form
            return BadRequest("Unable to upload one or more files");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            if (id != null)
            {
                bool isFileDeleted = false;

                // To convert to async fanction
                await Task.FromResult( isFileDeleted = PhysicalDeleteFile(id) );

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
            var allFilesInFolder = Directory.EnumerateFiles(Path.Combine(_appEnvironment.WebRootPath, ConstantString.directoryName));

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

        #region private func for Upload File

        private async Task UploadAndSave(IFormFile uploadedFile, string uploadsRootFolder, List<FileModel> fileToDB, List<FileModelResponse> filesResponse)
        {
            Guid id = Guid.NewGuid();

            //new unique file name
            string fileName = id + Path.GetExtension(uploadedFile.FileName);
            string fullFilePath = uploadsRootFolder + fileName;

            // save file to folder Files in catalog wwwroot
            using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(fileStream);

                var newFile = new FileModel { Name = fileName, Path = fullFilePath, Id = id, OriginalName = uploadedFile.FileName };

                //Adding to file for DB
                fileToDB.Add(newFile);

                //Adding file to response list
                filesResponse.Add(new FileModelResponse(newFile));
            }
        }

        #endregion
    }
}
