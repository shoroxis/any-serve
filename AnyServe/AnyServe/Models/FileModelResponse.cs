using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnyServe.Models
{
    public class FileModelResponse 
    {
        public Guid Id { get; set; }
        public string OriginalName { get; set; }


        public FileModelResponse()
        {
           
        }
        public FileModelResponse(Guid id, string originalname)
        {
            Id = id;
            OriginalName = originalname;
        }

        public FileModelResponse(FileModel fileModel)
        {
            Id = fileModel.Id;
            OriginalName = fileModel.OriginalName;
        }
    }
}
