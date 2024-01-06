using Core.IRepositories;
using Core.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using MongoDB.Bson;

namespace Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private MongoClient MongoClient { get; set; }
        private IMongoDatabase Database { get; set; }


        private IClientSessionHandle Session { get; set; }

        private readonly List<Func<Task>> _commands;
        private readonly IOptions<MongoDBSettings> _mongoDBSettings;
        protected IMongoCollection<T> DbSet;

        public BaseRepository(IOptions<MongoDBSettings> mongoDBSettings)
        {
            _mongoDBSettings = mongoDBSettings;
            _commands = new List<Func<Task>>();
            MongoClient = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            Database = MongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            DbSet = string.IsNullOrEmpty(mongoDBSettings.Value.CollectionName)
                ? Database.GetCollection<T>(typeof(T).Name)
                : Database.GetCollection<T>(mongoDBSettings.Value.CollectionName);
        }

        public IQueryable<T> AsQueryable()
        {
            return DbSet.AsQueryable();
        }

        public virtual async Task<List<T>> GetAll()
        {
            var all = await DbSet.FindAsync(Builders<T>.Filter.Empty);
            return all.ToList();
        }

        public virtual async Task<IEnumerable<T>> FilterByAsync(Expression<Func<T, bool>> filterExpression)
        {
            var result = await DbSet.FindAsync(filterExpression);
            return await result.ToListAsync();
        }

        public virtual async Task<IEnumerable<TProjected>> FilterByAsync<TProjected>(Expression<Func<T, bool>> filterExpression, Expression<Func<T, TProjected>> projectionExpression)
        {
            return await DbSet.Find(filterExpression).Project(projectionExpression).ToListAsync();
        }

        public virtual async Task<T> FindOneAsync(Expression<Func<T, bool>> filterExpression)
        {
            var result = await DbSet.FindAsync(filterExpression);
            return await result.FirstOrDefaultAsync();
        }

        public virtual async Task<T> FindByIdAsync(ObjectId id)
        {
            var result = await DbSet.FindAsync(Builders<T>.Filter.Eq("_id", id));
            return result.FirstOrDefault();
        }

        public virtual async Task InsertOneAsync(T document)
        {
            await DbSet.InsertOneAsync(document);
        }

        public virtual async Task InsertManyAsync(ICollection<T> documents)
        {
            await DbSet.InsertManyAsync(documents);
        }

        public virtual async Task ReplaceOneAsync(ObjectId id, T document)
        {
            await DbSet.FindOneAndReplaceAsync(Builders<T>.Filter.Eq("_id", id), document);
        }

        public virtual async Task DeleteByIdAsync(ObjectId id)
        {
            await DbSet.FindOneAndDeleteAsync(Builders<T>.Filter.Eq("_id", id));
        }

        public virtual async Task DeleteOneAsync(Expression<Func<T, bool>> filterExpression)
        {
            await DbSet.DeleteOneAsync(filterExpression);
        }

        public virtual async Task DeleteManyAsync(Expression<Func<T, bool>> filterExpression)
        {
            await DbSet.DeleteManyAsync(filterExpression);
        }

        private void ConfigureMongo()
        {
            if (MongoClient != null)
            {
                return;
            }

            MongoClient = new MongoClient(_mongoDBSettings.Value.ConnectionURI);
            Database = MongoClient.GetDatabase(_mongoDBSettings.Value.DatabaseName);
        }

        private async Task<int> SaveChanges()
        {
            ConfigureMongo();

            using (Session = await MongoClient.StartSessionAsync())
            {
                Session.StartTransaction();

                var commandTasks = _commands.Select(c => c());

                await Task.WhenAll(commandTasks);

                await Session.CommitTransactionAsync();
            }

            return _commands.Count;
        }
    }
}
