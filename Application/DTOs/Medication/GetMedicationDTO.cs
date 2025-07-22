using Application.DTOs.MedicationActiveIngredient;


namespace Application.DTOs.Medication
{
    public class GetMedicationDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Barcode { get; set; }
        public string? Dose { get; set; }
        public int MinQuantity { get; set; }
        public string? ManufacturerName { get; set; }
        public string? FormName { get; set; }
        public string? ClassName { get; set; }
        public List<GetMedicationIngredientDTO>? ActiveIngredients { get; set; }
    }
}
