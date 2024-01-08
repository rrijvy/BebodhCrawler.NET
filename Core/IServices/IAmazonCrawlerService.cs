using Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.IServices
{
    public interface IAmazonCrawlerService : IBaseService
    {
        string GenerateAmazonSearchUrlByCategory(string category);
        Task<List<AmazonProduct>> GetAmazonProductsByCategory(string category);
    }
}
