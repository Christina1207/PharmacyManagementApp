using AutoMapper;
using System;
using Domain.Entities;
using Application.DTOs.Supplier;
namespace Application.MappingProfiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Supplier, GetSupplierDTO>();
            CreateMap<CreateSupplierDTO, Supplier>();
            CreateMap<UpdateSupplierDTO, Supplier>();


        }
    }
}