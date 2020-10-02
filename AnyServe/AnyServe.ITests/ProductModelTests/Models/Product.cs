using System;
using AnyServe.Models;

namespace AnyServe.ITests.Models
{
    [GeneratedController("api/product")]
    public class Product : BaseModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

    }
}
