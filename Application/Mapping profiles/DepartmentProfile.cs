using Application.DTOs.Department;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping_profiles
{
    public class DepartmentProfile: Profile
    {
        public DepartmentProfile() {

            CreateMap<Department, GetDepartmentDTO>();
            CreateMap<CreateDepartmentDTO, Department>();
            CreateMap<UpdateDepartmentDTO, Department>();
        }
        
    }
}
