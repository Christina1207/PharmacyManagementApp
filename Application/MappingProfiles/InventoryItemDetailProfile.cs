using Application.DTOs.InventoryItemDetail;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles
{
    public class InventoryItemDetailProfile :Profile
    {
        public InventoryItemDetailProfile()
        {

            CreateMap<InventoryItemDetail, GetInventoryItemDetailDTO>();
            CreateMap<CreateInventoryItemDetailDTO, InventoryItemDetail>();
            CreateMap<UpdateInventoryItemDetailDTO, InventoryItemDetail>();
        }
    }
}
