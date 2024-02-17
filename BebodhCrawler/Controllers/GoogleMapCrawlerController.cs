using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace BebodhCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var httpClient = _clientFactory.CreateClient();

            var myContent = JsonConvert.SerializeObject(requestModel);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await httpClient.PostAsync("http://localhost:8001/googlemapscrap/run", byteContent);
            if (response.IsSuccessStatusCode)
            {
                return Ok(response);
            }
            return BadRequest();
        }

    }
}
