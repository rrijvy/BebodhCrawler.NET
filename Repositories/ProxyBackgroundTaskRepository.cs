using Core.Entities;
using Core.IRepositories;
using Core.Models;
using Microsoft.Extensions.Options;

namespace Repositories
{
    public class ProxyBackgroundTaskRepository : BaseRepository<ProxyBackgroundTaskHistory>, IProxyBackgroundTaskRepository
    {
        public ProxyBackgroundTaskRepository(IOptions<MongoDBSettings> mongoDBSettings) : base(mongoDBSettings)
        {

        }


    }
}
