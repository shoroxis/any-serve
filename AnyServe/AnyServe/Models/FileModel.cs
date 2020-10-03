using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AnyServe.Models
{
    
    public class FileModel : BaseModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
