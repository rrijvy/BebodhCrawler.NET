using Core.Entities;
using Core.Helpers;
using Core.IRepositories;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BebodhCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrawlsController : ControllerBase
    {
        private readonly IProxyRepository proxyRepository;
        private readonly ICrawlRepository crawlRepository;

        public CrawlsController(IProxyRepository proxyRepository, ICrawlRepository crawlRepository)
        {
            this.proxyRepository = proxyRepository;
            this.crawlRepository = crawlRepository;
        }

        [HttpPost("GetCrawls")]
        public async Task<ActionResult<List<Crawl>>> GetCrawls(CrawlRequestModel value)
        {
            var result = await this.crawlRepository.GetAll();
            return Ok(result);
        }

        [HttpPost("UpdateCrawls")]
        public async Task<ActionResult> UpdateCrawls(CrawlRequestModel value)
        {
            await this.crawlRepository.InsertOneAsync(new Crawl
            {
                CrawlerName = value.CrawlerName,
                OutputPath = value.OutputPath,
                Progress = value.Progress,
                AddedOn = Utility.GetCurrentUnixTimeAsString(),
            });

            return Ok();
        }
    }

}
