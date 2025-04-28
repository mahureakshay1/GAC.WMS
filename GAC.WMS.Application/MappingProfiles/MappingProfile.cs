using AutoMapper;
using GAC.WMS.Application.Dtos;
using GAC.WMS.Domain.Entities;

namespace GAC.WMS.Application.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<SaleOrder, SaleOrderDto>()
                .ForMember(dest => dest.SaleOrderLines, opt =>
                    opt.MapFrom(src => src.SaleOrderLines))
                .ReverseMap();
            CreateMap<PurchaseOrder, PurchaseOrderDto>()
                .ForMember(dest => dest.PurchaseOrderLines, opt =>
                    opt.MapFrom(src => src.PurchaseOrderLines))
                .ReverseMap();
            CreateMap<PurchaseOrderLine, PurchaseOrderLineDto>()
                .ReverseMap();
            CreateMap<SaleOrderLine, SaleOrderLineDto>()
                .ReverseMap();
        }
    }
}
