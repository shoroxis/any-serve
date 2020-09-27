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
    public class BaseController<T> : Controller where T : class
    {
        private Storage<T> _storage;
        private readonly ILogger<BaseController<T>> _logger;

        public BaseController(Storage<T> storage, ILogger<BaseController<T>> logger)
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
            var s = _storage.GetById(id);
            if(s != null)
                return Ok(_storage.GetById(id));

            return BadRequest();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Post(Guid id, [FromBody] T value)
        {
            await _storage.AddOrUpdate(id, value);
            return CreatedAtAction(nameof(Post), value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ifModelDeleted = await _storage.Delete(id);

            if (ifModelDeleted)
                return Ok();

            return BadRequest();
        }
    }
}
