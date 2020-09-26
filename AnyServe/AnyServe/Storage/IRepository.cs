using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnyServe.Storage
{
    public interface IRepository<T>
    {
        T GetAll();

        T GetById(Guid id);

        Task<T> AddOrUpdate(Guid id, T item);
    }
}
