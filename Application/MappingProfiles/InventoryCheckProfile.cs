using Application.DTOs.InventoryCheck;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles
{
    public class InventoryCheckProfile : Profile
    {
        public InventoryCheckProfile()
        {
            CreateMap<CreateInventoryCheckDTO, InventoryCheck>();
            CreateMap<CreateInventoryCheckItemDTO, InventoryCheckItem>();

            CreateMap<InventoryCheck, GetInventoryCheckDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            CreateMap<InventoryCheckItem, GetInventoryCheckItemDTO>()
                .ForMember(dest => dest.MedicationName, opt => opt.MapFrom(src => src.Medication.Name));
        }
    }
}
