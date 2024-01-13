using Core.Data;
using Core.Entities;
using Core.IRepositories;
using Core.IServices;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Repositories;
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

        [HttpGet("GetBlockedProxies")]
        public async Task<List<string>> GetBlockedProxies()
        {
            try
            {
                var proxies = await _proxyRepository.GetBlockedProxies();
                return proxies;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] HttpProxy httpProxy)
        {
            try
            {
                await _proxyRepository.InsertOneAsync(httpProxy);
                return Ok();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }
    }
}
