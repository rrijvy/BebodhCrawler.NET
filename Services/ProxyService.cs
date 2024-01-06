using Core.Entities;
using Core.IRepositories;
using Core.IServices;

namespace Services
{
    public class ProxyService : IProxyService
    {
        private readonly IProxyRepository _proxyRepository;

        public ProxyService(IProxyRepository proxyRepository)
        {
            _proxyRepository = proxyRepository;
        }

        public async Task<List<HttpProxy>> GetProxies()
        {
            var proxies = await _proxyRepository.GetAll();
            return proxies;
        }
    }
}
