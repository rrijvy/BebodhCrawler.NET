using Core.Entities;
using Core.Helpers;
using Core.IRepositories;
using Core.IServices;
using Core.Models;
using HtmlAgilityPack;
using System.Text;

namespace Services
{
    public class AmazonCrawlerService : IAmazonCrawlerService
    {
        private readonly IProxyService _proxyService;
        private readonly IProxyRepository _proxyRepository;

        public AmazonCrawlerService(IProxyService proxyService, IProxyRepository proxyRepository)
        {
            _proxyService = proxyService;
            _proxyRepository = proxyRepository;
        }
        public string GenerateAmazonSearchUrlByCategory(string category)
        {
            var url = new StringBuilder();
            url.Append("https://www.amazon.com/s?k=");
            url.Append(category);

            return url.ToString();
        }

        public string GenerateAmazonPaginatedSearchUrlByCategory(string category, int pageNumber)
        {
            var url = new StringBuilder();
            url.Append($"https://www.amazon.com/s?k={category}&page={pageNumber}");
            url.Append(category);

            return url.ToString();
        }

        public async Task<List<AmazonProduct>> GetAmazonProductsByCategory(string category)
        {
            HttpProxy proxy = null;

            try
            {
                //var category = "monitor";

                var amazonProducts = new List<AmazonProduct>();

                var searchUrl = GenerateAmazonSearchUrlByCategory(category);

                proxy = _proxyService.GetUnusedActiveProxy();

                if (proxy == null) return null;

                var httpClient = HttpClientHelper.GetHttpClient(proxy.IpAddress);

                var response = await httpClient.GetAsync(searchUrl);

                if (response.IsSuccessStatusCode)
                {
                    var htmlString = await Utility.GetHtmlAsStringAsync(response);

                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(htmlString);

                    var productNodes = htmlDoc.DocumentNode.SelectNodes("//div[@data-component-type=\"s-search-result\"]");

                    foreach (var productNode in productNodes)
                    {
                        AmazonProduct amazonProduct = AmazonProductInfoParser(productNode);
                        amazonProducts.Add(amazonProduct);
                    }

                    proxy.IsProxyRunning = false;
                    proxy.UpdatedAt = Utility.GetCurrentUnixTimeAsString();
                    await _proxyRepository.ReplaceOneAsync(proxy.Id, proxy);
                }
                else
                {
                    if (proxy.BlockedBy == null) proxy.BlockedBy = new List<CrawlerType> { CrawlerType.AMAZON };
                    else proxy.BlockedBy.Add(CrawlerType.AMAZON);

                    proxy.IsProxyRunning = false;
                    proxy.UpdatedAt = Utility.GetCurrentUnixTimeAsString();
                    await _proxyRepository.ReplaceOneAsync(proxy.Id, proxy);

                    return await GetAmazonProductsByCategory(category);
                }

                return amazonProducts;
            }
            catch (Exception)
            {
                if (proxy != null)
                {
                    if (proxy.BlockedBy == null) proxy.BlockedBy = new List<CrawlerType> { CrawlerType.AMAZON };
                    else proxy.BlockedBy.Add(CrawlerType.AMAZON);
                    proxy.IsProxyRunning = false;
                    proxy.UpdatedAt = Utility.GetCurrentUnixTimeAsString();
                    await _proxyRepository.ReplaceOneAsync(proxy.Id, proxy);

                    return await GetAmazonProductsByCategory(category);
                }

                return null;
            }

        }

        private static AmazonProduct AmazonProductInfoParser(HtmlNode productNode)
        {
            var amazonProduct = new AmazonProduct
            {
                Link = Utility.GetNodeInnerText(productNode, ".//h2/a | .//a", "href") ?? string.Empty,
                Name = Utility.GetNodeInnerText(productNode, ".//h2/a/span | .//h2") ?? string.Empty,
                Price = Utility.GetNodeInnerTextAsDecimal(productNode, ".//span[@class=\"a-price\"]/span"),
                Image = Utility.GetNodeAttributeValue(productNode, ".//img[@class=\"s-image\"]", "src") ?? string.Empty,
                TotalReviews = Utility.GetNodeInnerTextAsDouble(productNode, ".//span[@class=\"a-size-base s-underline-text\"] | //a[contains(@href,\"customerReviews\")]/span"),
                Rating = Utility.GetNodeInnerText(productNode, ".//span[@class=\"a-icon-alt\"]") ?? string.Empty
            };

            return amazonProduct;
        }
    }
}
