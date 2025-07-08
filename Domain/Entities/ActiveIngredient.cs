using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Entities;

public partial class ActiveIngredient
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<MedicationActiveIngredient> MedicationActiveIngredients { get; set; } = new List<MedicationActiveIngredient>();
}
