using AnyServe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnyServe.Storage
{
    public interface IDataRepository<T> where T : BaseModel
    {
        IEnumerable<T> GetAll();
        T Get(Guid id);
        Task<T> Insert(T entity);
        void Update(T entity);
        Task<bool> Delete(T entity);

    }
}
