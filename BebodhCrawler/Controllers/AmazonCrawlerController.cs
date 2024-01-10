﻿using Core.Helpers;
using Core.IRepositories;
using Core.IServices;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

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

        [Authorize]
        [HttpGet("GetAmazonProductsByCategory")]
        public async Task<ActionResult> GetAmazonProductsByCategory(string category)
        {
            try
            {
                var productNames = await _amazonCrawlerService.GetAmazonProductsByCategory(category);
                return Ok(productNames);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("RunScript")]
        public async Task<ActionResult> RunScript(string category)
        {
            try
            {
                PythonScriptRunner runner = new PythonScriptRunner();
                string result = runner.RunPythonScript("path_to_your_script.py", "arg1 arg2 arg3");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
