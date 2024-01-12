using Core.Data;
using Core.Entities;
using Core.IRepositories;
using Core.IServices;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BebodhCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private readonly IProxyService _proxyService;
        private readonly IProxyRepository _proxyRepository;
        private readonly IOptions<MongoDBSettings> _options;
        private readonly IMongoCollection<HttpProxy> _proxyQuery;

        public ProxyController(IProxyService proxyService, IProxyRepository proxyRepository, IOptions<MongoDBSettings> options)
        {
            _proxyService = proxyService;
            _proxyRepository = proxyRepository;
            _options = options;
            _proxyQuery = new MongoQuery<HttpProxy>(_options).GetQueryContext();
        }

        [HttpGet]
        public async Task<HttpProxy> Get()
        {
            var proxies = await _proxyService.GetUnusedActiveProxy();
            return proxies;
        }

        [HttpGet("GetActiveProxies")]
        public async Task<List<string>> GetActiveProxies()
        {
            try
            {
                var proxies = await _proxyRepository.GetActiveProxiesAsync();
                return proxies;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
