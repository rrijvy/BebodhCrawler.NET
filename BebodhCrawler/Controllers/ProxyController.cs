using Core.Entities;
using Core.IRepositories;
using Core.IServices;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

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
        public async Task<HttpProxy> Get()
        {
            var proxies = await _proxyService.GetUnusedActiveProxy();
            return proxies;
        }

        [HttpGet("{id}")]
        public async Task<HttpProxy> Get(ObjectId id)
        {
            var proxy = await _proxyRepository.FindByIdAsync(id);
            return proxy;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] HttpProxy httpProxy)
        {
            await _proxyRepository.InsertOneAsync(httpProxy);
            return Ok();
        }

        //[HttpPut("{id}")]
        //public async Task<ActionResult> Put(ObjectId id, [FromBody] HttpProxy httpProxy)
        //{
        //    await _proxyRepository.ReplaceOneAsync(id, httpProxy);
        //    return Ok();
        //}

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(ObjectId id)
        {
            await _proxyRepository.DeleteByIdAsync(id);
            return Ok();
        }


    }
}
