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
            CreateMap<CreatePrescriptionDTO, Prescription>();
            CreateMap<PrescriptionItemDTO, PrescriptionItem>();
        }

    }
}
