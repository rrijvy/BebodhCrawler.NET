using Core.Entities;
using Core.Helpers;

namespace Core.IRepositories
{
    public interface IProxyRepository : IBaseRepository<HttpProxy>
    {
        Task<List<string>> GetActiveProxiesAsync();
        Task<List<HttpProxy>> GetBlockedProxies(List<CrawlerType> crawlerTypes);
    }
}
