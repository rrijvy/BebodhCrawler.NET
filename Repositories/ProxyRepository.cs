using Core.Entities;
using Core.Helpers;
using Core.IRepositories;
using Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Repositories
{
    public class ProxyRepository : BaseRepository<HttpProxy>, IProxyRepository
    {
        private readonly IMongoCollection<HttpProxy> queryContext;

        public ProxyRepository(IOptions<MongoDBSettings> mongoDBSettings) : base(mongoDBSettings)
        {
            queryContext = this.GetQueryContext();
        }

        public async Task<List<HttpProxy>> GetActiveProxiesAsync(int count, List<CrawlerType>? crawlerTypes)
        {
            BsonDocument bsonFilter;

            if (crawlerTypes == null)
            {
                bsonFilter = new BsonDocument();
            }
            else
            {
                bsonFilter = new BsonDocument
                {
                    { "BlockedBy", new BsonDocument ("$not", new BsonDocument("$elemMatch", new BsonDocument ("$eq", "AMAZON"))) }
                };
            }

            var bsonSort = new BsonDocument("UpdatedOn", 1);

            var proxies = await queryContext.Find(bsonFilter).Sort(bsonSort).Limit(count).ToCursorAsync();

            var result = await proxies.ToListAsync();

            //var propertyName = Utility.GetPropertyName<HttpProxy>(x => x.IpAddress);

            return result;
        }

        public async Task<List<HttpProxy>> GetBlockedProxies(List<CrawlerType>? crawlerTypes)
        {
            if (crawlerTypes != null & crawlerTypes.Any())
            {
                var filter = new BsonDocument("BlockedBy", new BsonDocument { { "$exists", true }, { "$size", 0 } });
                var sort = new BsonDocument("UpdatedOn", 1);
                var proxies = await queryContext.Find(filter).Sort(sort).ToCursorAsync();
                var result = await proxies.ToListAsync();
                return result;
            }

            else
            {
                var filter = new BsonDocument("BlockedBy", new BsonDocument("$elemMatch", new BsonDocument("$in", new BsonArray(crawlerTypes))));
                var sort = new BsonDocument("UpdatedOn", 1);
                var proxies = await queryContext.Find(filter).Sort(sort).ToCursorAsync();
                var result = await proxies.ToListAsync();
                return result;
            }
        }

    }
}
