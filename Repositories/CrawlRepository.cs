using Core.Entities;
using Core.Helpers;
using Core.IRepositories;
using Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Repositories
{
    public class CrawlRepository : BaseRepository<Crawl>, ICrawlRepository
    {
        private readonly IMongoCollection<Crawl> queryContext;

        public CrawlRepository(IOptions<MongoDBSettings> mongoDBSettings) : base(mongoDBSettings)
        {
            queryContext = this.GetQueryContext();
        }
    }
}
