using Application.DTOs.Inventory;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles
{
    public class InventoryProfile:Profile
    {
        public InventoryProfile()
        {
            CreateMap<InventoryItem, GetInventoryItemDTO>()
                .ForMember(dest => dest.MedicationName, opt => opt.MapFrom(src => src.Medication.Name))
                .ForMember(dest => dest.ManufacturerName, opt => opt.MapFrom(src => src.Medication.Manufacturer.Name)) // <-- Add this
                .ForMember(dest => dest.MinQuantity, opt => opt.MapFrom(src => src.Medication.MinQuantity)) // <-- Add this
                .ForMember(dest => dest.TotalQuantity, opt => opt.MapFrom(src => src.InventoryItemDetails.Sum(d => d.Quantity)))
                .ForMember(dest => dest.Batches, opt => opt.MapFrom(src => src.InventoryItemDetails));

            CreateMap<InventoryItemDetail, GetInventoryBatchDTO>();
        }
    }
}
