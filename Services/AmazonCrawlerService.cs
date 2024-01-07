using Core.IServices;
using System.Text;

namespace Services
{
    public class AmazonCrawlerService : IAmazonCrawlerService
    {
        public string GenerateAmazonSearchUrlByCategory(string category)
        {
            var url = new StringBuilder();
            url.Append("https://www.amazon.com/s/ref=nb_sb_noss?url=search-alias%3Daps&field-keywords=");
            url.Append(category);

            return url.ToString();
        }
    }
}
