using AutoMapper;
using GAC.WMS.Application.Common.IntegrationModels;
using GAC.WMS.Application.Dtos;
using GAC.WMS.Domain.Enums;

namespace GAC.WMS.Infrastructure.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PurchaseOrderXmlModel, PurchaseOrderDto>()
                 .ForMember(dest => dest.PurchaseOrderLines, opt =>
                  opt.MapFrom(src => src.PurchaseOrderLines))
                 .ForMember(dest => dest.Status, opt =>
                  opt.MapFrom(src => (OrderStatus)src.Status));
            CreateMap<PurchaseOrderLineXmlModel, PurchaseOrderLineDto>();
        }
    }
}
