namespace Core.IServices
{
    public interface IAmazonCrawlerService : IBaseService
    {
        string GenerateAmazonSearchUrlByCategory(string category);
    }
}
