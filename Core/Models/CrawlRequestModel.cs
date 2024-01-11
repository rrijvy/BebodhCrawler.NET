using Core.Entities;

namespace Core.Models
{
    public class CrawlRequestModel
    {
        public string CrawlerName { get; set; }
        public string OutputPath { get; set; }
        public List<CrawlProgress> Progress { get; set; }
    }
}
