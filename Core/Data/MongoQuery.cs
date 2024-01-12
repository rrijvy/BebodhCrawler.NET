using Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Core.Data
{
    public class MongoQuery<T> where T : class
    {
        private readonly IOptions<MongoDBSettings> _options;
        private readonly MongoClient _client;
        public MongoQuery(IOptions<MongoDBSettings> options)
        {
            _options = options;
            _client = new MongoClient(_options.Value.ConnectionURI);

        }

        public IMongoCollection<T> GetQueryContext()
        {
            var database = _client.GetDatabase(_options.Value.DatabaseName);
            var dbSet = database.GetCollection<T>(typeof(T).Name);
            return dbSet;
        }
    }
}
