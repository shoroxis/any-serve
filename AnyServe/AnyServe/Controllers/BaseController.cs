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
        private IRepository<T> _repository;
        private readonly ILogger<BaseController<T>> _logger;

        public BaseController(IRepository<T> repository, ILogger<BaseController<T>> logger)
        {
            _repository = repository;
            _logger = logger;
            _logger.LogInformation("BaseController created");
        }

        [HttpGet]
        public IEnumerable<T> Get()
        {
            return (IEnumerable<T>)_repository.GetAll();
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            return Ok(_repository.GetById(id));
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Post(Guid id, [FromBody] T value)
        {
            await _repository.AddOrUpdate(id, value);
            return CreatedAtAction(nameof(Post), value);
        }
    }
}
