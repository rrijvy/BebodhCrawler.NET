using Core.Entities;
using Core.IRepositories;
using Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Repositories
{
    public class ProxyRepository : BaseRepository<HttpProxy>, IProxyRepository
    {
        public ProxyRepository(IOptions<MongoDBSettings> mongoDBSettings) : base(mongoDBSettings)
        {
        }
    }
}
