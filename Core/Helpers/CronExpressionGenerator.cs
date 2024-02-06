using Core.Entities;
using Core.Models;

namespace Core.Helpers
{
    public static class CronExpressionGenerator
    {
        public static string GenerateExpression(ProxySchedule schedule)
        {
            if (schedule == null) return string.Empty;

            switch (schedule.RecurrenceType)
            {
                case RecurrenceType.Daily:
                    return GenerateDailyExpression(schedule);
                case RecurrenceType.Weekly:
                    return GenerateWeeklyExpression(schedule);
                case RecurrenceType.Monthly:
                    return GenerateMonthlyExpression(schedule);
                default:
                    return string.Empty;
            }
        }
        private static string GenerateDailyExpression(ProxySchedule schedule)
        {
            if (schedule != null)
            {
                var now = DateTime.UtcNow;
                var afterRepeatDays = now.AddDays(schedule.RepeatEvery);
                return $"{schedule.Minute} {schedule.Hour} {afterRepeatDays.Day} {afterRepeatDays.Month} ? {afterRepeatDays.Year}";
            }

            return string.Empty;
        }

        private static string GenerateWeeklyExpression(ProxySchedule schedule)
        {
            if (schedule != null)
            {
                string dayOfMonth = "?", month = "*", daysOfWeek = string.Empty, year = "*";
                if (!schedule.WeekSpecificDays.Any()) schedule.WeekSpecificDays.Add(WeekDay.Friday);
                daysOfWeek = string.Join(",", schedule.WeekSpecificDays.Select(x => $"{(int)x}").ToList());
                return $"{schedule.Minute} {schedule.Hour} {dayOfMonth} {month} {daysOfWeek} {year}";
            }
            return string.Empty;
        }

        private static string GenerateMonthlyExpression(ProxySchedule schedule)
        {
            return string.Empty;
        }
    }
}
