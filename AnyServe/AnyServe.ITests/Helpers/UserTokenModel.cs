using AnyServe.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnyServe.ITests.Helpers
{
    class UserTokenModel
    {
        public string Token { get; set; }
        public AnyServUser UserDetails { get; set; }
    }
}
