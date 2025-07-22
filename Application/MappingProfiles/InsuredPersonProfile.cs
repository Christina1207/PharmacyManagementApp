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
            CreateMap<InsuredPerson, GetInsuredPersonDTO>().ReverseMap();
            CreateMap<CreateInsuredPersonDTO, InsuredPerson>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => InsuredPersonStatus.Active));
        }
    }
}
