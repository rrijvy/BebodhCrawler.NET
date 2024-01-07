using Core.Entities;
using Core.Helpers;
using Core.IRepositories;
using Core.IServices;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Net;
using System.Xml.Linq;

namespace BebodhCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmazonCrawlerController : ControllerBase
    {
        private readonly IProxyService _proxyService;
        private readonly IAmazonCrawlerService _amazonCrawlerService;
        private readonly IProxyRepository _proxyRepository;

        public AmazonCrawlerController(IProxyService proxyService, IAmazonCrawlerService amazonCrawlerService, IProxyRepository proxyRepository)
        {
            _proxyService = proxyService;
            _amazonCrawlerService = amazonCrawlerService;
            _proxyRepository = proxyRepository;
        }

        [HttpGet("AmazonProducts")]
        public async Task<ActionResult> AmazonProducts()
        {
            try
            {
                string htmlAsString = string.Empty;

                var proxy = _proxyService.GetUnusedActiveProxy();

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

            var proxy = _proxyService.GetUnusedActiveProxy();

            var httpClient = HttpClientHelper.GetHttpClient(proxy.IpAddress);

            var response = await httpClient.GetAsync(searchUrl);

            if (response.StatusCode.ToString() == "503")
            {
                proxy.IsProxyRunning = false;
                proxy.UpdatedAt = Utility.GetCurrentUnixTime();

                await _proxyRepository.ReplaceOneAsync(proxy.Id, proxy);
            }

            var htmlString = await Utility.GetHtmlAsStringAsync(response);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlString);

            var productNodes = htmlDoc.DocumentNode.SelectNodes("//div[@data-component-type=\"s-search-result\"]");

            var productNames = new List<string>();

            foreach (var productNode in productNodes)
            {
                var productLink = productNode.SelectNodes(".//h2/a")[0].GetAttributeValue("href", "").Trim();
                var productName = productNode.SelectNodes(".//h2/a/span")[0].InnerText.Trim();
                var productPrice = productNode.SelectNodes(".//span[@class=\"a-price\"]/span")[0].InnerText.Trim();
                var productImage = productNode.SelectNodes(".//img[@class=\"s-image\"]")[0].GetAttributeValue("src", "").Trim(); ;
                var totalReviews = productNode.SelectNodes(".//span[@class=\"a-size-base s-underline-text\"]")[0].InnerText.Trim();
                var ratings = productNode.SelectNodes(".//span[@class=\"a-icon-alt\"]")[0].InnerText.Trim();

                productNames.Add(productName);
            }
            return Ok(productNames);
        }
    }
}
