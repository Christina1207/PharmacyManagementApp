using Application.DTOs.Sale;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles
{
    public class SaleProfile : Profile
    {
        public SaleProfile ()
        {
            CreateMap<Sale, GetSaleDTO>()
                .ForMember(dest => dest.PharmacistName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));
            CreateMap<Sale, GetSaleDetailsDTO>()
                .ForMember(dest => dest.PharmacistName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.DispenseDate, opt => opt.MapFrom(src => src.Prescription.DispenseDate))
                .ForMember(dest => dest.Prescription, opt => opt.MapFrom(src => src.Prescription));


        }

    }
}
