using AnyServe.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnyServe.ITests.ProductModelTests.Models
{
    [GeneratedController("api/user")]
    class User : BaseModel
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
