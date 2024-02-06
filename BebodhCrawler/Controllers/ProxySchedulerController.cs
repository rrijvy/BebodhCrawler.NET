using Amazon.Runtime.Internal;
using Core.Entities;
using Core.Helpers;
using Core.IRepositories;
using Core.IServices;
using Core.Models;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Services;

namespace BebodhCrawler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxySchedulerController : ControllerBase
    {
        private readonly IProxyScheduleRepository _proxyScheduleRepository;

        public ProxySchedulerController(IProxyScheduleRepository proxyScheduleRepository)
        {
            _proxyScheduleRepository = proxyScheduleRepository;
        }

        [HttpPost("AddOrUpdateProxyRetriverSchedule")]
        public async Task<IActionResult> AddOrUpdateProxyRetriverSchedule(ProxySchedule requestModel)
        {
            bool isUpdate = requestModel.Id != null;

            var cornExpression = CronExpressionGenerator.GenerateExpression(requestModel);

            if (string.IsNullOrEmpty(cornExpression)) return BadRequest();

            requestModel.CornExpression = cornExpression;

            if (isUpdate)
                await _proxyScheduleRepository.ReplaceOneAsync(requestModel.Id, requestModel);
            else
                await _proxyScheduleRepository.InsertOneAsync(requestModel);

            //RecurringJob.AddOrUpdate<IProxyService>(requestModel.Id, x => { Console.WriteLine(); }, cornExpression);

            return Ok();
        }
    }
}
