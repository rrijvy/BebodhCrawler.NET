using Core.Helpers;

namespace Core.Entities
{
    public class ProxySchedule : BaseModel
    {
        public ProxySchedule()
        {
            RecurrenceType = RecurrenceType.Daily;
            RepeatEvery = 1;
            WeekSpecificDays = new List<WeekDay>();
        }

        public RecurrenceType RecurrenceType { get; set; }
        public int RepeatEvery { get; set; }
        public List<WeekDay> WeekSpecificDays { get; set; }
        public int? Hour { get; set; }
        public int? Minute { get; set; }
        public TimeZoneInfo TimeZone { get; set; }
        public MonthlyRecurrenceWeek? MonthlyRecurrenceWeek { get; set; }
        public MonthlySelectionType? MonthlySelectionType { get; set; }
        public int? MonthlySpecificDay { get; set; }
        public string CornExpression { get; set; }
    }
}

