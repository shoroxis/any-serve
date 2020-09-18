﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnyServe.Storage
{
    /// <summary>
    /// Basic entities storage
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Storage<T> where T : class
    {
        private Dictionary<Guid, T> storage = new Dictionary<Guid, T>();

        public IEnumerable<T> GetAll() => storage.Values;

        public T GetById(Guid id)
        {
            return storage.FirstOrDefault(x => x.Key == id).Value;
        }

        public void AddOrUpdate(Guid id, T item)
        {
            storage[id] = item;
        }
    }
}