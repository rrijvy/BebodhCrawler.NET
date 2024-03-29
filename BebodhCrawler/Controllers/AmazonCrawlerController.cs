﻿using Core.Helpers;
using Core.IRepositories;
using Core.IServices;
using Core.Models;
using Hangfire;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Services;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;

namespace BebodhCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AmazonCrawlerController : ControllerBase
    {
        private readonly IProxyService _proxyService;
        private readonly IAmazonCrawlerService _amazonCrawlerService;
        private readonly IProxyRepository _proxyRepository;
        private readonly PythonScriptService _scriptService;
        private readonly IOptions<CrawlerConfig> _crawlerConfig;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public AmazonCrawlerController(IProxyService proxyService, IAmazonCrawlerService amazonCrawlerService, IProxyRepository proxyRepository,
            PythonScriptService scriptService, IOptions<CrawlerConfig> crawlerConfig, IBackgroundJobClient backgroundJobClient)
        {
            _proxyService = proxyService;
            _amazonCrawlerService = amazonCrawlerService;
            _proxyRepository = proxyRepository;
            _scriptService = scriptService;
            _crawlerConfig = crawlerConfig;
            _backgroundJobClient = backgroundJobClient;
        }

        //[HttpGet("AmazonProducts")]
        //public async Task<ActionResult> AmazonProducts()
        //{
        //    try
        //    {
        //        string htmlAsString = string.Empty;

        //        var proxy = await _proxyService.GetUnusedActiveProxy();

        //        var productUrl = @"https://www.amazon.com/SAMSUNG-Computer-DisplayPort-Adjustable-LF24T454FQNXGO/dp/B08WGLL83S";

        //        var httpClient = HttpClientHelper.GetHttpClient(proxy.IpAddress);

        //        var response = await httpClient.GetAsync(productUrl);

        //        if (response.Content.Headers.ContentEncoding.Contains("gzip"))
        //        {
        //            Stream stream = await response.Content.ReadAsStreamAsync();
        //            GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress);
        //            StreamReader reader = new StreamReader(gzipStream);
        //            htmlAsString = await reader.ReadToEndAsync();
        //            Console.WriteLine(htmlAsString);

        //        }
        //        else
        //        {
        //            htmlAsString = await response.Content.ReadAsStringAsync();
        //        }


        //        var htmlDoc = new HtmlDocument();
        //        htmlDoc.LoadHtml(htmlAsString);

        //        var node = htmlDoc.DocumentNode.SelectNodes("//*[@id=\"productTitle\"]");
        //        var firstNode = node.FirstOrDefault();
        //        var productName = firstNode.InnerText.Trim();

        //        return Ok(productName);
        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest(ex.Message);
        //    }
        //}

        //[Authorize]
        //[HttpGet("GetAmazonProductsByCategory")]
        //public async Task<ActionResult> GetAmazonProductsByCategory(string category)
        //{
        //    try
        //    {
        //        var productNames = await _amazonCrawlerService.GetAmazonProductsByCategory(category);
        //        return Ok(productNames);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //}

        //[HttpGet("RunScript")]
        //public ActionResult RunScript()
        //{
        //    try
        //    {
        //        //string currentDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        //        string pythonPath = @"C:\Users\tarek\source\repos\BebodhCrawler.NET\TestCrawler\env\Scripts\python.exe";

        //        string scriptPath = @"C:\Users\tarek\source\repos\BebodhCrawler.NET\TestCrawler\TestCrawler.py 54564654654";

        //        var startInfo = new ProcessStartInfo
        //        {
        //            FileName = pythonPath,
        //            Arguments = scriptPath,
        //            RedirectStandardOutput = true,
        //            RedirectStandardError = true,
        //            UseShellExecute = false,
        //            CreateNoWindow = true
        //        };

        //        using var process = new Process
        //        {
        //            StartInfo = startInfo
        //        };

        //        process.Start();
        //        process.WaitForExit();

        //        string output = process.StandardOutput.ReadToEnd();
        //        string error = process.StandardError.ReadToEnd();

        //        if (!string.IsNullOrEmpty(output))
        //        {
        //            Console.WriteLine("Output from Python script:");
        //            Console.WriteLine(output);
        //        }

        //        if (!string.IsNullOrEmpty(error))
        //        {
        //            Console.WriteLine("Error from Python script:");
        //            Console.WriteLine(error);
        //        }

        //        return Ok("Python script executed.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //}

        //[HttpGet("RunBackgroundTask")]
        //public ActionResult RunBackgroundTask()
        //{
        //    try
        //    {
        //        var jobId = BackgroundJob.Enqueue(() => _amazonCrawlerService.GetAmazonProductsByCategory("monitor"));

        //        return Ok("Python script executed.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //}

        [HttpPost("RunAmazonCategoryScrapper")]
        public ActionResult<string> RunAmazonCategoryScrapper(AmazonCategoryScrapperRequestModel requestModel)
        {
            requestModel.PythonExecutablePath = _crawlerConfig.Value.PythonExecutablePath;
            requestModel.ScrapperFilePath = _crawlerConfig.Value.AmazonCrawlersFilePath.CategoryCrawler;

            string jobId = _backgroundJobClient.Enqueue(() => _scriptService.RunAmazonCategoryScrapper(requestModel));

            return Ok(jobId);
        }

    }
}
