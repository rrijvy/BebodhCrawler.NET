using Core.IServices;
using System.Text;

namespace Services
{
    public class AmazonCrawlerService : IAmazonCrawlerService
    {
        public string GenerateAmazonSearchUrlByCategory(string category)
        {
            var url = new StringBuilder();
            url.Append("https://www.amazon.com/s?k=");
            url.Append(category);

            return url.ToString();
        }
    }
}
