using Application.DTOs.InsuredPerson;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.MappingProfiles
{
    public class InsuredPersonProfile : Profile
    {
        public InsuredPersonProfile()
        {
            CreateMap<InsuredPerson, GetInsuredPersonDTO>();
            CreateMap<CreateInsuredPersonDTO, InsuredPerson>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => InsuredPersonStatus.Active))                ;
        }
    }
}
