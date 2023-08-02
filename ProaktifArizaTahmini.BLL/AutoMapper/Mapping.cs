using AutoMapper;
using ProaktifArizaTahmini.BLL.Models.DTOs;
using ProaktifArizaTahmini.BLL.Models.RequestModel;
using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.AutoMapper
{
    public class Mapping : Profile
    {
        public Mapping() 
        {
            CreateMap<MyData, MyDataDTO>().ReverseMap().ForAllMembers(x => x.UseDestinationValue());
            CreateMap<MyDataDTO, MyData>().ReverseMap().ForAllMembers(x => x.UseDestinationValue());
            CreateMap<MyData, MyData>().ReverseMap().ForAllMembers(x => x.UseDestinationValue());
            CreateMap<HistoryOfChange, HistoryOfChange>().ReverseMap().ForAllMembers(x => x.UseDestinationValue());
            CreateMap<MyDataFilterParams, MyData>().ReverseMap().ForAllMembers(x => x.UseDestinationValue());
            CreateMap<DisturbanceFilterParams, Disturbance>().ReverseMap().ForAllMembers(x => x.UseDestinationValue());
        }
    }
}
