using Core.Entities;
using Core.IRepositories;
using Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Reflection.Metadata;

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
            var filter = Builders<HttpProxy>.Filter.Not(Builders<HttpProxy>.Filter.ElemMatch("BlockedBy", Builders<string>.Filter.Eq(s => s, "amazon")));

            var sort = Builders<HttpProxy>.Sort.Ascending(p => p.UpdatedAt);

            var proxies = await queryContext.DistinctAsync(x => x.IpAddress, filter);

            var result = await proxies.ToListAsync();

            return result;
        }
    }
}
