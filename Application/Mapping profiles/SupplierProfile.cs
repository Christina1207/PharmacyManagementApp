using AutoMapper;
using Domain.Entities;
using Application.DTOs.Supplier;
namespace Application.MappingProfiles
{
    public class SupplierProfile : Profile
    {
        public SupplierProfile()
        {
            CreateMap<Supplier, GetSupplierDTO>();
            CreateMap<CreateSupplierDTO, Supplier>();
            CreateMap<UpdateSupplierDTO, Supplier>();


        }
    }
}