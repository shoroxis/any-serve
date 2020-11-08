using System;
using System.Collections.Generic;
using System.Text;

namespace AnyServe.ITests.UploadTests
{
    static class ConstantString
    {
        
        public const string urlGET = "/api/Media";
        public const string urlDelete = "/api/Media/ ";
        public const string urlSingleItemUpload = "/api/Media/uploadfile";
        public const string singleFile = "text_4.txt";
        public const string fileNameNotExist = "file_not_exist.txt";
        public const string filesToUploadPath = @"UploadTests\TestFiles\";
        public static readonly string[] listFileName = { "text_1.txt", "text_2.txt", "text_3.txt" };
        public const string LoginUrl = @"api/Login";
    }
}
