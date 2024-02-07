using Core.Helpers;
using MongoDB.Bson;

namespace Core.Models
{
    public class ProxyScheduleRequestModel
    {
        public ProxyScheduleRequestModel()
        {
            RecurrenceType = RecurrenceType.Daily;
            RepeatEvery = 1;
            WeekSpecificDays = new List<WeekDay>();
        }
        public ObjectId? Id { get; set; }
        public RecurrenceType RecurrenceType { get; set; }
        public int RepeatEvery { get; set; }
        public List<WeekDay> WeekSpecificDays { get; set; }
        public int? Hour { get; set; }
        public int? Minute { get; set; }
        public TimeZoneInfo? TimeZone { get; set; }
        public MonthlyRecurrenceWeek? MonthlyRecurrenceWeek { get; set; }
        public MonthlySelectionType? MonthlySelectionType { get; set; }
        public int? MonthlySpecificDay { get; set; }
    }
}
