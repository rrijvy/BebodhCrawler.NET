using Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.IServices
{
    public interface IProxyService : IBaseService
    {
        Task<List<HttpProxy>> GetProxies();
        Task<List<string>> RetriveProxies();
    }
}
