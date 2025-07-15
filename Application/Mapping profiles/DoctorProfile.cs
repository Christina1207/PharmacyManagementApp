using Application.DTOs.Doctor;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping_profiles
{
    public class DoctorProfile:Profile
    {
        public DoctorProfile()
        {

            CreateMap<Doctor, GetDoctorDTO>();
            CreateMap<CreateDoctorDTO, Doctor>();
            CreateMap<UpdateDoctorDTO, Doctor>();
        }
    }
}
