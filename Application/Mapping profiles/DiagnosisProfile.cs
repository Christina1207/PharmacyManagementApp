using Application.DTOs.Diagnosis;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping_profiles
{
    public class DiagnosisProfile:Profile
    {
        public DiagnosisProfile()
        {

            CreateMap<Diagnosis, GetDiagnosisDTO>();
            CreateMap<CreateDiagnosisDTO, Diagnosis>();
            CreateMap<UpdateDiagnosisDTO, Diagnosis>();
        }
    }
}
