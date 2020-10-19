using System;
using System.Collections.Generic;
using System.Text;

namespace AnyServe.ITests.UploadTests
{
    class StringHelper
    {
        public readonly string fileNameNotExist = "file_not_exist.txt";
        public readonly string urlGET = "/api/Media";
        public readonly string urlDelete = "/api/Media/ "; //+ singleFile.Id;
    }
}
