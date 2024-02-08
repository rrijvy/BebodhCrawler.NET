using Core.Entities;
using Core.Models;
using System.Linq;

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
                return $"{schedule.Minute ?? 0} {schedule.Hour ?? 12} */{schedule.RepeatEvery} * *";
            }

            return string.Empty;
        }

        private static string GenerateWeeklyExpression(ProxySchedule schedule)
        {
            if (schedule != null && schedule.WeekSpecificDays.Any())
            {
                var weekDays = schedule.WeekSpecificDays.Select(x => x.ToString().Substring(0, 3));
                return $"{schedule.Minute ?? 0} {schedule.Hour ?? 12} * * {string.Join(",", weekDays)}";
            }

            return string.Empty;
        }

        private static string GenerateMonthlyExpression(ProxySchedule schedule)
        {
            return string.Empty;
        }
    }
}
