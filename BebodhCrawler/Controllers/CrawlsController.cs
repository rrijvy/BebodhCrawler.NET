using Core.Entities;
using Core.Helpers;
using Core.IRepositories;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BebodhCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrawlsController : ControllerBase
    {
        private readonly IProxyRepository _proxyRepository;
        private readonly ICrawlRepository _crawlRepository;

        public CrawlsController(IProxyRepository proxyRepository, ICrawlRepository crawlRepository)
        {
            _proxyRepository = proxyRepository;
            _crawlRepository = crawlRepository;
        }

        [HttpPost("GetCrawls")]
        public async Task<ActionResult<List<Crawl>>> GetCrawls(CrawlRequestModel value)
        {
            var result = await _crawlRepository.GetAll();
            return Ok(result);
        }

        [HttpPost("InitCrawl")]
        public async Task<ActionResult> InitCrawl(CrawlRequestModel value)
        {
            await _crawlRepository.InsertOneAsync(new Crawl
            {
                Progress = value.Progress,
                AddedOn = Utility.GetCurrentUnixTime(),
            });

            return Ok();
        }

        [HttpPost("UpdateCrawls")]
        public async Task<ActionResult> UpdateCrawls(CrawlRequestModel requestModel)
        {
            var filterDefination = Builders<Crawl>.Filter.Eq(x => x.Id, requestModel.CrawlId);
            var updateDefination = Builders<Crawl>.Update
                .Set(x => x.Progress, requestModel.Progress)
                .Set(x => x.OutputPath, requestModel.OutputPath);
            var result = await _crawlRepository.UpdateOneAsync(filterDefination, updateDefination);
            if (result.ModifiedCount > 0) return Ok();
            return BadRequest();

        }
    }
}