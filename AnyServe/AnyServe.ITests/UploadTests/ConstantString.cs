using System;
using System.Collections.Generic;
using System.Text;

namespace AnyServe.ITests.UploadTests
{
    class ConstantString
    {
        
        public readonly string urlGET = "/api/Media";
        public readonly string urlDelete = "/api/Media/ ";
        public readonly string urlSingleItemUpload = "/api/Media/uploadfile";
        public readonly string singleFile = "text_4.txt";
        public readonly string fileNameNotExist = "file_not_exist.txt";
        public readonly string filesToUploadPath = @"UploadTests\TestFiles\";
        public readonly string[] listFileName = { "text_1.txt", "text_2.txt", "text_3.txt" };
    }
}
