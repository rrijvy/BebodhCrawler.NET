using BebodhCrawler.Extensions;
using Core.Helpers;
using Core.IRepositories;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BebodhCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GoogleMapCrawlerController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ICrawlRepository _crawlRepository;

        public GoogleMapCrawlerController(IHttpClientFactory clientFactory, ICrawlRepository crawlRepository)
        {
            _clientFactory = clientFactory;
            _crawlRepository = crawlRepository;
        }

        [HttpPost("RunGoogleMapScrapper")]
        public async Task<IActionResult> RunGoogleMapScrapper(GoogleMapScrapperRequestModel requestModel)
        {
            try
            {
                var currentUserId = this.GetCurrentUserId();
                var currentUserName = this.GetCurrentUserName();
                var crawl = Utility.InitCrawl(currentUserId);
                await _crawlRepository.InsertOneAsync(crawl);
                requestModel.TaskId = crawl.Id.ToString();
                var httpClient = _clientFactory.CreateClient();
                var requestBody = HttpClientHelper.GetByteArrayContent(requestModel);
                var response = await httpClient.PostAsync("http://localhost:8000/googlemapscrap/run", requestBody);
                if (response.IsSuccessStatusCode) return Ok(response);
                return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
