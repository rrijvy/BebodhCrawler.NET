﻿using Core.Data;
using Core.Entities;
using Core.IRepositories;
using Core.IServices;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Core.Helpers;

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

        [HttpPost("GetActiveProxies")]
        public async Task<List<HttpProxy>> GetActiveProxies(ProxyRequestModel proxyRequest)
        {
            try
            {
                var proxies = await _proxyRepository.GetActiveProxiesAsync(proxyRequest.Count, proxyRequest.CrawlerTypes);
                return proxies;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("GetBlockedProxies")]
        public async Task<List<HttpProxy>> GetBlockedProxies()
        {
            try
            {
                var proxies = await _proxyRepository.GetBlockedProxies(new List<CrawlerType> { CrawlerType.AMAZON });
                return proxies;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("UpdateProxy")]
        public async Task<bool> UpdateProxy(ProxyUpdateRequestModel requestModel)
        {
            try
            {
                var result = await _proxyRepository.UpdateProxy(requestModel);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
