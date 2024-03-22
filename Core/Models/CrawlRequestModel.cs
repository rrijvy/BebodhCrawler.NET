using Core.Entities;
using MongoDB.Bson;

namespace Core.Models
{
    public class CrawlRequestModel
    {
        public string TaskId { get; set; }
        public string OutputPath { get; set; }
        public List<CrawlProgress>? Progress { get; set; }
    }
}
