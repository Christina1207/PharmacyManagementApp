using Application.DTOs.Doctor;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles
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
