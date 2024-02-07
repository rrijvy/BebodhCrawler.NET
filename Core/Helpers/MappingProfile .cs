using AutoMapper;
using Core.Entities;
using Core.Models;

namespace Core.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProxySchedule, ProxyScheduleRequestModel>().ReverseMap();
        }
    }
}
