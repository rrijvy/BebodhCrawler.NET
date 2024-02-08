using Core.Helpers;

namespace Core.Models
{
    public class HangfireJob
    {
        public string JobId { get; set; }

        public string Cron { get; set; }

        public string Queue { get; set; }

        public DateTime? NextExecution { get; set; }

        public string LastJobId { get; set; }

        public string LastJobState { get; set; }

        public DateTime? LastExecution { get; set; }

        public DateTime? CreatedAt { get; set; }

        public bool Removed { get; set; }

        public string TimeZoneId { get; set; }

        public string Error { get; set; }

        public int RetryAttempt { get; set; }
        public string JobTitle { get; set; }
    }
}
