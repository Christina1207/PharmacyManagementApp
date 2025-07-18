using Application.DTOs.MedicationForm;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles
{
    public class MedicationFormProfile:Profile
    {
        public MedicationFormProfile()
        {

            CreateMap<MedicationForm, GetMedicationFormDTO>();
            CreateMap<CreateMedicationFormDTO, MedicationForm>();
            CreateMap<UpdateMedicationFormDTO, MedicationForm>();
        }
    }
}
