using Core.Entities;
using Core.Helpers;
using Core.Models;

namespace Core.IRepositories
{
    public interface IProxyRepository : IBaseRepository<HttpProxy>
    {
        Task<List<HttpProxy>> GetActiveProxiesAsync(int count, List<CrawlerType>? crawlerTypes);
        Task<List<HttpProxy>> GetBlockedProxies(List<CrawlerType> crawlerTypes);
        Task<bool> UpdateProxy(HttpProxy proxy);
        Task<bool> UpdateProxy(ProxyUpdateRequestModel requestModel);
    }
}
