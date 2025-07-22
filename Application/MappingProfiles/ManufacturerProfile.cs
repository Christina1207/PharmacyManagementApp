using Application.DTOs.Manufacturer;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles
{
    public class ManufacturerProfile : Profile
    {
        public ManufacturerProfile()
        {
            CreateMap<Manufacturer, GetManufacturerDTO>();
            CreateMap<CreateManufacturerDTO, Manufacturer>();
            CreateMap<UpdateManufacturerDTO, Manufacturer>();
        }
    }
}
