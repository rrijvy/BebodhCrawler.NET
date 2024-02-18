using BebodhCrawler.Extensions;
using Core.Helpers;
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

        public GoogleMapCrawlerController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpPost("RunGoogleMapScrapper")]
        public async Task<IActionResult> RunGoogleMapScrapper(GoogleMapScrapperRequestModel requestModel)
        {
            var currentUserId = this.GetCurrentUserId();
            var currentUserName = this.GetCurrentUserName();

            var httpClient = _clientFactory.CreateClient();
            var requestBody = HttpClientHelper.GetByteArrayContent(requestModel);
            var response = await httpClient.PostAsync("http://localhost:8000/googlemapscrap/run", requestBody);
            if (response.IsSuccessStatusCode)
            {
                return Ok(response);
            }
            return BadRequest();
        }

    }
}
