using Application.DTOs.Prescription;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class PrescriptionProfile:Profile
    {
        public PrescriptionProfile()
        {
            CreateMap<CreatePrescriptionDTO, Prescription>().ForMember(
                 dest => dest.PrescriptionItems,
                 opt => opt.Ignore()
                  );
            CreateMap<PrescriptionItemDTO, PrescriptionItem>();
            CreateMap<Prescription,GetPrescriptionDTO>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => $"{src.Patient.FirstName} {src.Patient.LastName}"))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => $"{src.Doctor.FirstName} {src.Doctor.LastName}"))
                .ForMember(dest => dest.PharmacistName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.PrescriptionItems));

            CreateMap<PrescriptionItem, GetPrescriptionItemDTO>()
    .ForMember(dest => dest.MedicationName, opt => opt.MapFrom(src => src.Medication.Name));


        }

    }
}
