using AnyServe.Models;
using AnyServe.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnyServe.Controllers
{
    /// <summary>
    /// Base generic CRUD controller
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Route("api/[controller]")]
    public class BaseController<T> : Controller where T : BaseModel
    {
        private readonly IDataRepository<T> _storage;
        private readonly ILogger<BaseController<T>> _logger;

        public BaseController(IDataRepository<T> storage, ILogger<BaseController<T>> logger)
        {
            _storage = storage;
            _logger = logger;
            _logger.LogInformation("BaseController created");
        }

        [HttpGet]
        public IEnumerable<T> Get()
        {
            return _storage.GetAll();
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            return Ok(_storage.Get(id));
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Post(Guid id, [FromBody] T value)
        {
            await _storage.Insert(value);
            return CreatedAtAction(nameof(Post), value);
        }
    }
}
