using Application.DTOs.Order;
using AutoMapper;
using Domain.Entities;
namespace Application.MappingProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            // From DTO to Entity
            CreateMap<CreateOrderDTO, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
            CreateMap<CreateOrderItemDTO, OrderItem>();

            // From Entity to DTO
            CreateMap<Order, GetOrderDTO>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            CreateMap<OrderItem, GetOrderItemDTO>()
                .ForMember(dest => dest.MedicationName, opt => opt.MapFrom(src => src.Medication.Name));
        }
    }
}
