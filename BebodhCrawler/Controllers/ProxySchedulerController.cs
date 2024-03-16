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
        private readonly IProxyScheduleService _proxyScheduleService;
        private readonly IMapper _mapper;

        public ProxySchedulerController(IProxyScheduleRepository proxyScheduleRepository, IProxyScheduleService proxyScheduleService, IMapper mapper)
        {
            _proxyScheduleRepository = proxyScheduleRepository;
            _proxyScheduleService = proxyScheduleService;
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

                if (_proxyScheduleService.MatchWithExistingSchedule(existingProxySchedules, requestModel))
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

        [HttpGet("GetAllRecurringSchedules")]
        public async Task<IActionResult> GetAllRecurringSchedules()
        {
            try
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
            catch (Exception)
            {
                return Ok(new List<HangfireJob>());

            }
        }

        [HttpGet("RunProxyRetriver")]
        public IActionResult RunProxyRetriver()
        {
            BackgroundJob.Enqueue<IProxyService>(x => x.RetrieveProxies());

            return Ok();
        }

        [HttpGet("RunActiveProxyChecker")]
        public IActionResult RunActiveProxyChecker()
        {
            BackgroundJob.Enqueue<IProxyService>(x => x.RecheckActiveProxies());

            return Ok();
        }

        [HttpPost("DeleteProxySchedule")]
        public async Task<IActionResult> DeleteProxySchedule(string jobId)
        {
            var canParse = ObjectId.TryParse(jobId, out ObjectId id);

            if (canParse)
            {
                await _proxyScheduleRepository.DeleteByIdAsync(id);
                RecurringJob.RemoveIfExists(id.ToString());
                return Ok();
            }

            return BadRequest();
        }
    }
}
