namespace Core.Entities
{
    public class Crawl : BaseModel
    {
        public string CrawlerName { get; set; }
        public string OutputPath { get; set; }
        public List<CrawlProgress> Progress { get; set; }
    }

    public class CrawlProgress
    {
        public string At { get; set; }
        public string Progress { get; set; }
    }
}
