using AnyServe.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnyServe.Controllers
{
    /// <summary>
    /// Base generic CRUD controller
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Route("api/[controller]")]
    public class BaseController<T> : Controller where T : class
    {
        private Storage<T> _storage;
        private readonly ILogger<BaseController<T>> _logger;

        public BaseController(Storage<T> storage)
        {
            _storage = storage;
            _logger.LogInformation("BaseController created");
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
