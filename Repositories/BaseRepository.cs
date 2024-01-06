using Core.IRepositories;
using Core.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

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

        public virtual async Task Add(T obj)
        {
            await DbSet.InsertOneAsync(obj);
        }

        public virtual async Task<T> Get(Guid id)
        {
            var data = await DbSet.FindAsync(Builders<T>.Filter.Eq("_id", id));
            return data.SingleOrDefault();
        }

        public virtual async Task<List<T>> GetAll()
        {
            var all = await DbSet.FindAsync(Builders<T>.Filter.Empty);
            return all.ToList();
        }

        public virtual async Task Remove(Guid id)
        {
            await DbSet.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
        }

        public async Task Update(Guid id, T obj)
        {
            await DbSet.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", id), obj);
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
