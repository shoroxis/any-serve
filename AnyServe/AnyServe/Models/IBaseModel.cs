using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnyServe.Models
{
    interface IBaseModel
    {
        Guid id { get; set; }
    }
}
