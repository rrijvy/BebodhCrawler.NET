using AutoMapper;
using Core.Entities;
using Core.Helpers;
using Core.IRepositories;
using Core.IServices;
using Core.Models;
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace BebodhCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxySchedulerController : ControllerBase
    {
        private readonly IProxyScheduleRepository _proxyScheduleRepository;
        private readonly IMapper _mapper;

        public ProxySchedulerController(IProxyScheduleRepository proxyScheduleRepository, IMapper mapper)
        {
            _proxyScheduleRepository = proxyScheduleRepository;
            _mapper = mapper;
        }

        [HttpPost("AddOrUpdateProxyRetriverSchedule")]
        public async Task<IActionResult> AddOrUpdateProxyRetriverSchedule(ProxyScheduleRequestModel requestBody)
        {
            try
            {
                var requestModel = _mapper.Map<ProxySchedule>(requestBody);

                if (requestModel == null) return BadRequest();

                bool isUpdate = !requestModel.Id.Equals(ObjectId.Empty);

                var existingProxySchedules = await _proxyScheduleRepository.GetAll();

                if (MatchWithExistingSchedule(existingProxySchedules, requestModel))
                {
                    return BadRequest("Schedule already exist.");
                }

                var cornExpression = CronExpressionGenerator.GenerateExpression(requestModel);

                if (string.IsNullOrEmpty(cornExpression)) return BadRequest();

                requestModel.CornExpression = cornExpression;

                if (isUpdate)
                {
                    requestModel.UpdatedOn = Utility.GetCurrentUnixTime();
                    await _proxyScheduleRepository.ReplaceOneAsync(requestModel.Id, requestModel);
                }
                else
                {
                    requestModel.Id = ObjectId.GenerateNewId();
                    requestModel.AddedOn = Utility.GetCurrentUnixTime();
                    await _proxyScheduleRepository.InsertOneAsync(requestModel);
                }

                RecurringJob.AddOrUpdate<IProxyService>(requestModel.Id.ToString(), x => x.RetrieveProxies(), cornExpression);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private bool MatchWithExistingSchedule(List<ProxySchedule> existingProxySchedules, ProxySchedule requestModel)
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

            //if (requestModel.RecurrenceType.Equals(RecurrenceType.Monthly))
            //{
            //    var similarSchedule = existingProxySchedules.FirstOrDefault(
            //        t => (!isUpdate || !t.P1stonType.Equals(requestModel.P1stonType))
            //             && (t.RecurrenceType.Equals(requestModel.RecurrenceType) &&
            //                 t.MonthlySelectionType.Equals(requestModel.MonthlySelectionType) &&
            //                 t.WeeklySpecificDay.Equals(requestModel.WeeklySpecificDay) &&
            //                 t.MonthlyRecurrenceWeek.Equals(requestModel.MonthlyRecurrenceWeek) &&
            //                 t.MonthlySpecificDay.Equals(requestModel.MonthlySpecificDay) &&
            //                 t.RepeatEvery.Equals(requestModel.RepeatEvery) && t.Hour.Equals(requestModel.Hour) &&
            //                 t.Minute.Equals(requestModel.Minute) && t.TimeZone.Equals(requestModel.TimeZone))
            //    );
            //    if (similarSchedule != null)
            //        return true;
            //}

            return false;
        }

        [HttpGet("GetAllRecurringSchedules")]
        public async Task<IActionResult> GetAllRecurringSchedules()
        {
            var connection = JobStorage.Current.GetConnection();
            var recurringJobs = connection.GetRecurringJobs();
            var proxySchedules = await _proxyScheduleRepository.GetAll();
            var hangfireRecurringJobs = new List<HangfireJob>();
            foreach (var job in recurringJobs)
            {
                var schedule = proxySchedules.FirstOrDefault(x => x.Id.ToString().Equals(job.Id));
                var hangfireJob = new HangfireJob
                {
                    JobId = job.Id,
                    Cron = job.Cron,
                    Queue = job.Queue,
                    NextExecution = job.NextExecution,
                    LastJobId = job.LastJobId,
                    LastJobState = job.LastJobState,
                    LastExecution = job.LastExecution,
                    CreatedAt = job.CreatedAt,
                    Removed = job.Removed,
                    TimeZoneId = job.TimeZoneId,
                    Error = job.Error,
                    RetryAttempt = job.RetryAttempt,
                    JobTitle = schedule?.Title ?? string.Empty
                };
                hangfireRecurringJobs.Add(hangfireJob);
            }
            connection.Dispose();
            return Ok(hangfireRecurringJobs);
        }

        [HttpGet("RunProxyRetriver")]
        public IActionResult RunProxyRetriver()
        {
            BackgroundJob.Enqueue<IProxyService>(x => x.RetrieveProxies());

            return Ok();
        }

        [HttpPost("DeleteProxySchedule")]
        public async Task<IActionResult> DeleteProxySchedule(ObjectId jobId)
        {
            await _proxyScheduleRepository.DeleteByIdAsync(jobId);
            RecurringJob.RemoveIfExists(jobId.ToString());
            return Ok();
        }
    }
}
