using Core.Helpers;

namespace Core.Models
{
    public class ProxyUpdateRequestModel
    {
        public string Id { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsRunning { get; set; }
        public CrawlerType? BlockedBy { get; set; }
    }
}
