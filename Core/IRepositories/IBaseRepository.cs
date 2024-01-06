using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.IRepositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task Add(T obj);
        Task<T> Get(Guid id);
        Task<List<T>> GetAll();
        Task Update(Guid id, T obj);
        Task Remove(Guid id);
    }
}