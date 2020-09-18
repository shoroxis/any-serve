﻿using AnyServe.Storage;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnyServe.Controllers
{
    [Route("api/[controller]")]
    public class BaseController<T> : Controller where T : class
    {
        private Storage<T> _storage;

        public BaseController(Storage<T> storage)
        {
            _storage = storage;
        }

        [HttpGet]
        public IEnumerable<T> Get()
        {
            return _storage.GetAll();
        }

        [HttpGet("{id}")]
        public T Get(Guid id)
        {
            return _storage.GetById(id);
        }

        [HttpPost("{id}")]
        public void Post(Guid id, [FromBody] T value)
        {
            _storage.AddOrUpdate(id, value);
        }
    }
}
