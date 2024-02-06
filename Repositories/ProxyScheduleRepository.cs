using Core.Entities;
using Core.IRepositories;
using Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Repositories
{
    public class ProxyScheduleRepository : BaseRepository<ProxySchedule>, IProxyScheduleRepository
    {
        private readonly IMongoCollection<ProxySchedule> queryContext;

        public ProxyScheduleRepository(IOptions<MongoDBSettings> mongoDBSettings) : base(mongoDBSettings)
        {
            queryContext = this.GetQueryContext();
        }
    }
}
