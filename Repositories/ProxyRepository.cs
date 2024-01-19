using Core.Entities;
using Core.Helpers;
using Core.IRepositories;
using Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

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
            FilterDefinition<HttpProxy> filter;
            var builder = Builders<HttpProxy>.Filter;

            if (crawlerTypes == null)
            {
                bsonFilter = new BsonDocument();
                filter = builder.Empty;
            }
            else
            {
                var filters = new List<FilterDefinition<HttpProxy>>()
                {
                    builder.Eq(x => x.IsActive, true),
                    builder.ElemMatch(x => x.BlockedBy, y => y == CrawlerType.AMAZON)
                };

                filter = builder.And(filters);

                bsonFilter = new BsonDocument("$and", new BsonArray
                {
                    new BsonDocument("IsActive", new BsonDocument("$eq", true)),
                    new BsonDocument("BlockedBy", new BsonDocument ("$not", new BsonDocument("$elemMatch", new BsonDocument ("$in", new BsonArray(crawlerTypes.Select(x=>x.ToString()))))))
                });
            }

            var bsonSort = new BsonDocument("UpdatedOn", 1);

            var proxies = await queryContext.Find(bsonFilter).Sort(bsonSort).Limit(count).ToCursorAsync();

            var result = await proxies.ToListAsync();

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

        public async Task<HttpProxy> UpdateProxy(HttpProxy proxy)
        {
            try
            {
                var singleProxyFilter = new BsonDocument("_id", new BsonDocument("$eq", proxy.Id));
                var proxyRecord = await queryContext.Find(singleProxyFilter).FirstOrDefaultAsync();
                if (proxyRecord == null)
                {
                    await this.InsertOneAsync(proxy);
                }
                else
                {
                    await queryContext.ReplaceOneAsync(singleProxyFilter, proxy);
                }

                return proxy;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

    }
}
