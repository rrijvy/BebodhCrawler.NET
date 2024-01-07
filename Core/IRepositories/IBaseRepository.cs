using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Core.IRepositories
{
    public interface IBaseRepository<T> where T : class
    {
        IQueryable<T> AsQueryable();
        Task<List<T>> GetAll();
        Task<IEnumerable<T>> FilterByAsync(Expression<Func<T, bool>> filterExpression);
        Task<IEnumerable<TProjected>> FilterByAsync<TProjected>(Expression<Func<T, bool>> filterExpression, Expression<Func<T, TProjected>> projectionExpression);
        Task<T> FindOneAsync(Expression<Func<T, bool>> filterExpression);
        Task<T> FindByIdAsync(ObjectId id);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> filterExpression);
        Task InsertOneAsync(T document);
        Task InsertManyAsync(ICollection<T> documents);
        Task ReplaceOneAsync(ObjectId id, T document);
        Task DeleteByIdAsync(ObjectId id);
        Task DeleteOneAsync(Expression<Func<T, bool>> filterExpression);
        Task DeleteManyAsync(Expression<Func<T, bool>> filterExpression);
    }
}