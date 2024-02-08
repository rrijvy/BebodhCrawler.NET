using AutoMapper;
using Core.Entities;
using Core.Models;
using Hangfire.Storage;

namespace Core.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProxySchedule, ProxyScheduleRequestModel>().ReverseMap();
            CreateMap<HangfireJob, RecurringJobDto>().ReverseMap();
        }
    }
}
