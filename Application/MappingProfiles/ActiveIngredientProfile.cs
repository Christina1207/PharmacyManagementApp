using Application.DTOs.ActiveIngredient;
using AutoMapper;
using Domain.Entities;

namespace Application.MappingProfiles
{
    public class ActiveIngredientProfile : Profile
    {
       public ActiveIngredientProfile()
       {
            CreateMap<ActiveIngredient, GetActiveIngredientDTO>();
            CreateMap<CreateActiveIngredientDTO, ActiveIngredient>();
            CreateMap<UpdateActiveIngredientDTO, ActiveIngredient>();
       }
        
    }
}
