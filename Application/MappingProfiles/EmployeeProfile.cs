using Application.DTOs.Employee;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.MappingProfiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {

            CreateMap<Employee, GetEmployeeDTO>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.InsuredPersonId))
                      .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.InsuredPerson.FirstName))
                      .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.InsuredPerson.LastName))
                      .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.InsuredPerson.DateOfBirth))
                      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.InsuredPerson.Status))

            CreateMap<CreateEmployeeDTO, Employee>()
                   .ForMember(dest => dest.InsuredPersonId, opt => opt.Ignore());

        }
    }
}