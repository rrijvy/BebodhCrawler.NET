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
        public TimeZoneInfo? TimeZoneInfo { get; set; }
        public MonthlyRecurrenceWeek? MonthlyRecurrenceWeek { get; set; }
        public MonthlySelectionType? MonthlySelectionType { get; set; }
        public int? MonthlySpecificDay { get; set; }
        public string? CornExpression { get; set; }
        public string Title
        {
            get
            {
                try
                {
                    var dateWithTime = new DateTime(DateTime.Now.Year, 1, 1, Hour ?? 0, Minute ?? 0, 0);
                    var timeString = dateWithTime.ToString("h:mm tt");
                    if (RecurrenceType.Equals(RecurrenceType.Daily))
                        if (Minute != null)
                            return
                                $"Every {RepeatEvery} {(RepeatEvery == 1 ? "day" : "days")} {timeString}";

                    if (RecurrenceType.Equals(RecurrenceType.Weekly))
                    {
                        if (Minute != null)
                        {
                            string weeklySpecificDays = string.Join(", ", WeekSpecificDays.Select(x => x.ToString()).ToList());
                            return $"Every {RepeatEvery} {(RepeatEvery == 1 ? "week" : "weeks")} on {weeklySpecificDays} {timeString}";
                        }
                    }

                    //if (RecurrenceType.Equals(RecurrenceType.Monthly))
                    //{
                    //    if (MonthlySelectionType.Equals(MonthlySelectionType.OnSpecificDay))
                    //        if (Minute != null)
                    //            return
                    //                $"Every {RepeatEvery} {(RepeatEvery == 1 ? "month" : "months")} on day {MonthlySpecificDay} {timeString}";
                    //    if (Minute != null)
                    //        return
                    //            $"Every {RepeatEvery} {(RepeatEvery == 1 ? "month" : "months")} on the {MonthlyRecurrenceWeek} {WeeklySpecificDay} {timeString}";
                    //}
                }
                catch (Exception)
                {
                    Console.WriteLine($"Error in getting title of UpdateSchedule");
                }

                return string.Empty;
            }
        }
    }
}

