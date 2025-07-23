using Application.DTOs.Medication;
using Application.DTOs.MedicationActiveIngredient;
using AutoMapper;
using Domain.Entities;


namespace Application.MappingProfiles
{
    public class MedicationProfile:Profile
    {
        public MedicationProfile()
        {
            CreateMap<CreateMedicationDTO, Medication>()
             .ForMember(
                dest => dest.MedicationActiveIngredients, // The destination property on the entity
                opt => opt.MapFrom(src => src.ActiveIngredients) // The source property on the DTO
            );
            CreateMap<UpdateMedicationDTO, Medication>() 
             .ForMember(dest => dest.MedicationActiveIngredients, opt => opt.Ignore());

            // For mapping the nested ingredient DTO
            CreateMap<MedicationActiveIngredientDTO, MedicationActiveIngredient>();
          

            CreateMap<Medication, GetMedicationDTO>()
                .ForMember(dest => dest.ManufacturerName, opt => opt.MapFrom(src => src.Manufacturer.Name))
                .ForMember(dest => dest.FormName, opt => opt.MapFrom(src => src.Form.Name))
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class.Name))
             .ForMember(dest => dest.ActiveIngredients, opt => opt.MapFrom(src => src.MedicationActiveIngredients));

            // For mapping the list of ingredients within GetMedicationDTO
            CreateMap<MedicationActiveIngredient, GetMedicationIngredientDTO>()
              .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src => src.Ingredient.Name));
        }
    }
    
}
