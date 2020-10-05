using AnyServe.Models;
using AnyServe.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class BaseController<T> : Controller where T : BaseModel, new()
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
            var result = _storage.Get(id);
            if(result != null)
                return Ok(result);

            return NotFound();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Post(Guid id, [FromBody] T value)
        {
            await _storage.Insert(value);
            return CreatedAtAction(nameof(Post), value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            //create dummy entity
            T entityToDelete = new T() { Id = id };

            try
            {
                await _storage.Delete(entityToDelete);
                return Ok();
            }
            catch(DbUpdateConcurrencyException ex)
            {
                _logger.LogError($"Delete with id = {id} failed. " + ex.Message);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete with id = {id} failed. " + ex.Message);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
