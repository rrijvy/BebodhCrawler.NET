using Core.Helpers;
using Core.IServices;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Net;

namespace BebodhCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmazonCrawlerController : ControllerBase
    {
        private readonly IProxyService _proxyService;
        private readonly IAmazonCrawlerService _amazonCrawlerService;

        public AmazonCrawlerController(IProxyService proxyService, IAmazonCrawlerService amazonCrawlerService)
        {
            _proxyService = proxyService;
            _amazonCrawlerService = amazonCrawlerService;
        }

        [HttpGet("AmazonProducts")]
        public async Task<ActionResult> AmazonProducts()
        {
            try
            {
                string htmlAsString = string.Empty;

                var proxy = await _proxyService.GetUnsedActiveProxy();

                var productUrl = @"https://www.amazon.com/SAMSUNG-Computer-DisplayPort-Adjustable-LF24T454FQNXGO/dp/B08WGLL83S";

                var httpClient = HttpClientHelper.GetHttpClient(proxy.IpAddress);

                var response = await httpClient.GetAsync(productUrl);

                if (response.Content.Headers.ContentEncoding.Contains("gzip"))
                {
                    Stream stream = await response.Content.ReadAsStreamAsync();
                    GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress);
                    StreamReader reader = new StreamReader(gzipStream);
                    htmlAsString = await reader.ReadToEndAsync();
                    Console.WriteLine(htmlAsString);

                }
                else
                {
                    htmlAsString = await response.Content.ReadAsStringAsync();
                }


                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlAsString);

                var node = htmlDoc.DocumentNode.SelectNodes("//*[@id=\"productTitle\"]");
                var firstNode = node.FirstOrDefault();
                var productName = firstNode.InnerText.Trim();

                return Ok(productName);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAmazonProductsByCategory")]
        public async Task<ActionResult> GetAmazonProductsByCategory()
        {
            var category = "monitor";

            var searchUrl = _amazonCrawlerService.GenerateAmazonSearchUrlByCategory(category);

            var proxy = await _proxyService.GetUnsedActiveProxy();

            var httpClient = HttpClientHelper.GetHttpClient(proxy.IpAddress);

            var response = await httpClient.GetAsync(searchUrl);

            var htmlString = await Utility.GetHtmlAsStringAsync(response);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlString);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//*[@id=\"productTitle\"]");

            var productNames = new List<string>();

            foreach (var node in nodes)
            {
                var productName = node.InnerText.Trim();
                productNames.Add(productName);
            }

            return Ok(productNames);
        }
    }
}
