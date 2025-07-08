using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Entities;

public partial class MedicationActiveIngredient
{
    public int Id { get; set; }

    public int MedicationId { get; set; }

    public int IngredientId { get; set; }

    public decimal Amount { get; set; }

    public virtual ActiveIngredient Ingredient { get; set; } = null!;

    public virtual Medication Medication { get; set; } = null!;
}
