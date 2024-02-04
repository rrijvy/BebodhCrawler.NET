namespace Core.Models
{
    public class CrawlerConfig
    {
        public string PythonExecutablePath { get; set; }
        public AmazonCrawlers AmazonCrawlersFilePath { get; set; }
    }

    public class AmazonCrawlers
    {
        public string CategoryCrawler { get; set; }
        public string ReviewCrawler { get; set; }
    }
}
