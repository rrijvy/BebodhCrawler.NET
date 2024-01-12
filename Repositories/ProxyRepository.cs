using Core.Entities;
using Core.Helpers;
using Core.IRepositories;
using Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Reflection;

namespace Repositories
{
    public class ProxyRepository : BaseRepository<HttpProxy>, IProxyRepository
    {
        private readonly IMongoCollection<HttpProxy> queryContext;

        public ProxyRepository(IOptions<MongoDBSettings> mongoDBSettings) : base(mongoDBSettings)
        {
            queryContext = this.GetQueryContext();
        }

        public async Task<List<string>> GetActiveProxiesAsync()
        {
            //var filter = Builders<HttpProxy>.Filter.Not(Builders<HttpProxy>.Filter.ElemMatch(z => z.BlockedBy, Builders<CrawlerType>.Filter.Eq(s => s, CrawlerType.AMAZON)));

            var filter = Builders<HttpProxy>.Filter.ElemMatch(x => x.BlockedBy, Builders<CrawlerType>.Filter.Eq(y => y, CrawlerType.AMAZON));

            //var filter = Builders<HttpProxy>.Filter.Eq(x => x.IsActive, false);

            var sort = Builders<HttpProxy>.Sort.Ascending(p => p.UpdatedAt);

            var document = new BsonDocument("BlockedBy", new BsonDocument("$elemMatch", new BsonDocument("$eq", "AMAZON")));

            //var proxies = await queryContext.FindAsync(filter);

            var bsonFilter = new BsonDocument
            {
                { "BlockedBy", new BsonDocument ("$not", new BsonDocument("$elemMatch", new BsonDocument ("$eq", "AMAZON"))) }
            };

            var bsonSort = new BsonDocument("UpdatedOn", 1);

            var proxies = await queryContext.Find(bsonFilter).Sort(bsonSort).ToCursorAsync();

            //var proxies = await queryContext.DistinctAsync(x => x.IpAddress, filter);

            var result = await proxies.ToListAsync();

            var propertyName = Utility.GetPropertyName<HttpProxy>(x => x.IpAddress);

            return new List<string>();
        }



    }
}
