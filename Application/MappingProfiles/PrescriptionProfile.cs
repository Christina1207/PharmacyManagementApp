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
        }

    }
}
