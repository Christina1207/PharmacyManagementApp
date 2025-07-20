using Application.DTOs.FamilyMember;
using AutoMapper;
using Domain.Entities;


namespace Application.MappingProfiles
{
    public class FamilyMemberProfile :Profile
    {
        public FamilyMemberProfile() {

            CreateMap<FamilyMember, GetFamilyMemberDTO>()
                       .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.InsuredPersonId))
                         .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.InsuredPerson.FirstName))
                         .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.InsuredPerson.LastName))
                         .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.InsuredPerson.DateOfBirth))
                         .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.InsuredPerson.Status));
            CreateMap<CreateFamilyMemberDTO, FamilyMember>()
                   .ForMember(dest => dest.InsuredPersonId, opt => opt.Ignore());


        }
    }
}
