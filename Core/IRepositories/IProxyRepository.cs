using Core.Entities;

namespace Core.IRepositories
{
    public interface IProxyRepository : IBaseRepository<HttpProxy>
    {
        Task<List<string>> GetActiveProxiesAsync();
    }
}
