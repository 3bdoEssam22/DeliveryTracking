using AutoMapper;
using DeliveryTracking.Core.Entities.OrderModule;
using Shared.DataTransferObjects.OrderDTOs;

namespace DeliveryTracking.Services.Profiles
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderResponseDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
                .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver != null ? src.Driver.FullName : null));

            CreateMap<OrderItem, OrderItemResponseDTO>();
        }
    }
}
