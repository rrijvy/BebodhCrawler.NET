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

        public async Task<List<AmazonProduct>> GetAmazonProductsByCategory()
        {
            HttpProxy proxy = null;

            try
            {
                var category = "monitor";

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
                        var productLink = productNode.SelectNodes(".//h2/a")[0].GetAttributeValue("href", "").Trim();
                        var productName = productNode.SelectNodes(".//h2/a/span")[0].InnerText.Trim();
                        var productPrice = productNode.SelectNodes(".//span[@class=\"a-price\"]/span")[0].InnerText.Trim();
                        var productImage = productNode.SelectNodes(".//img[@class=\"s-image\"]")[0].GetAttributeValue("src", "").Trim(); ;
                        var totalReviews = productNode.SelectNodes(".//span[@class=\"a-size-base s-underline-text\"]")[0].InnerText.Trim();
                        var ratings = productNode.SelectNodes(".//span[@class=\"a-icon-alt\"]")[0].InnerText.Trim();

                        var amazonProduct = new AmazonProduct
                        {
                            Name = productName,
                            Link = productLink,
                            Image = productImage,
                        };

                        if (decimal.TryParse(productPrice, out decimal amazonProductPrice))
                            amazonProduct.Price = amazonProductPrice;

                        if (double.TryParse(totalReviews, out double amazonProductTotalReviews))
                            amazonProduct.TotalReviews = amazonProductTotalReviews;

                        if (double.TryParse(ratings, out double amazonProductRatings))
                            amazonProduct.Rating = amazonProductRatings;

                        amazonProducts.Add(amazonProduct);
                    }

                    proxy.IsProxyRunning = false;
                    proxy.UpdatedAt = Utility.GetCurrentUnixTime();
                    await _proxyRepository.ReplaceOneAsync(proxy.Id, proxy);
                }
                else
                {
                    if (proxy.BlockedBy == null) proxy.BlockedBy = new List<CrawlerType> { CrawlerType.AMAZON };
                    else proxy.BlockedBy.Add(CrawlerType.AMAZON);

                    proxy.IsProxyRunning = false;
                    proxy.UpdatedAt = Utility.GetCurrentUnixTime();
                    await _proxyRepository.ReplaceOneAsync(proxy.Id, proxy);

                    return await GetAmazonProductsByCategory();
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
                    proxy.UpdatedAt = Utility.GetCurrentUnixTime();
                    await _proxyRepository.ReplaceOneAsync(proxy.Id, proxy);

                    return await GetAmazonProductsByCategory();
                }

                return null;
            }

        }
    }
}
