using Amazon.Runtime.Internal;
using AutoMapper;
using Core.Entities;
using Core.Helpers;
using Core.IRepositories;
using Core.IServices;
using Core.Models;
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using Repositories;
using Services;

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

                bool isUpdate = !requestModel.Id.Equals(Guid.Empty);

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
        public IActionResult GetAllRecurringSchedules()
        {
            var connection = JobStorage.Current.GetConnection();
            var recurringJobs = connection.GetRecurringJobs();
            connection.Dispose();
            return Ok(recurringJobs);
        }

        [HttpGet("RunProxyRetriver")]
        public IActionResult RunProxyRetriver()
        {
            BackgroundJob.Enqueue<IProxyService>(x => x.RetrieveProxies());

            return Ok();
        }
    }
}
