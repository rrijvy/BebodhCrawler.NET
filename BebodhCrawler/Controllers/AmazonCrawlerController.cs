using Core.IServices;
using HtmlAgilityPack;
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

        public AmazonCrawlerController(IProxyService proxyService)
        {
            _proxyService = proxyService;
        }

        [HttpGet("AmazonProducts")]
        public async Task<ActionResult> AmazonProducts()
        {
            //var proxies = await _proxyService.RetriveProxies();

            var productUrl = @"https://www.amazon.com/SAMSUNG-Computer-DisplayPort-Adjustable-LF24T454FQNXGO/dp/B08WGLL83S";
            var jsonUrl = @"https://jsonplaceholder.typicode.com/todos/1";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");
            var handler = new HttpClientHandler
            {
                Proxy = new WebProxy($"http://81.21.234.64:6453"),
                UseProxy = true
            };
            var response = await httpClient.GetAsync(productUrl);
            string htmlAsString = string.Empty;
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
            var productName = firstNode.InnerText;

            return Ok(productName);
        }
    }
}
