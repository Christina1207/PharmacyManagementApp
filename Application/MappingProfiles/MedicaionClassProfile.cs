using Application.DTOs.MedicationClass;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles
{
    public class MedicaionClassProfile
    {
        public class MedicationClassProfile : Profile
        {
            public MedicationClassProfile()
            {
                CreateMap<MedicationClass, GetMedicationClassDTO>();
                CreateMap<CreateMedicationClassDTO, MedicationClass>();
                CreateMap<UpdateMedicationClassDTO, MedicationClass>();
            }
        }
    }
}
