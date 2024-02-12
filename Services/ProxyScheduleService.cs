using Core.Entities;
using Core.Helpers;
using Core.IServices;
using MongoDB.Bson;

namespace Services
{
    public class ProxyScheduleService : IProxyScheduleService
    {
        public bool MatchWithExistingSchedule(List<ProxySchedule> existingProxySchedules, ProxySchedule requestModel)
        {
            bool isUpdate = !requestModel.Id.Equals(ObjectId.Empty);

            if (requestModel.RecurrenceType.Equals(RecurrenceType.Weekly))
            {
                var similarSchedule = existingProxySchedules.FirstOrDefault(
                    t => (!isUpdate || !t.Id.ToString().Equals(requestModel.Id.ToString()))
                         && (t.RecurrenceType.Equals(requestModel.RecurrenceType) &&
                             t.WeekSpecificDays.All(requestModel.WeekSpecificDays.Contains) &&
                             t.WeekSpecificDays.Count == requestModel.WeekSpecificDays.Count &&
                             t.RepeatEvery.Equals(requestModel.RepeatEvery) && t.Hour.Equals(requestModel.Hour) &&
                             t.Minute.Equals(requestModel.Minute))
                );
                if (similarSchedule != null)
                    return true;
            }
            if (requestModel.RecurrenceType.Equals(RecurrenceType.Daily))
            {
                var similarSchedule = existingProxySchedules.FirstOrDefault(
                    t => (!isUpdate || !t.Id.ToString().Equals(requestModel.Id.ToString()))
                         && (t.RecurrenceType.Equals(requestModel.RecurrenceType) &&
                             t.RepeatEvery.Equals(requestModel.RepeatEvery) && t.Hour.Equals(requestModel.Hour) &&
                             t.Minute.Equals(requestModel.Minute))
                );
                if (similarSchedule != null)
                    return true;
            }

            return false;
        }
    }
}
