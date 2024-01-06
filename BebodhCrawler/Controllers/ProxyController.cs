using Core.Entities;
using Core.IRepositories;
using Core.IServices;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BebodhCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private readonly IProxyService _proxyService;
        private readonly IProxyRepository _proxyRepository;

        public ProxyController(IProxyService proxyService, IProxyRepository proxyRepository)
        {
            _proxyService = proxyService;
            _proxyRepository = proxyRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<HttpProxy>> Get()
        {
            var proxies = await _proxyService.GetProxies();
            return proxies;
        }

        [HttpGet("{id}")]
        public async Task<HttpProxy> Get(Guid id)
        {
            var proxy = await _proxyRepository.Get(id);
            return proxy;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] HttpProxy httpProxy)
        {
            await _proxyRepository.Add(httpProxy);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(Guid id, [FromBody] HttpProxy httpProxy)
        {
            await _proxyRepository.Update(id, httpProxy);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _proxyRepository.Remove(id);
            return Ok();
        }
    }
}
