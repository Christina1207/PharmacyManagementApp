using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Medication
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Barcode { get; set; } = null!;

    public string Dose { get; set; } = null!;

    public int MinQuantity { get; set; }

    public int ManufacturerId { get; set; }

    public int FormId { get; set; }

    public int ClassId { get; set; }

    public virtual MedicationClass Class { get; set; } = null!;

    public virtual MedicationForm Form { get; set; } = null!;

    public virtual ICollection<InventoryCheckItem> InventoryCheckItems { get; set; } = new List<InventoryCheckItem>();

    public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();

    public virtual Manufacturer Manufacturer { get; set; } = null!;

    public virtual ICollection<MedicationActiveIngredient> MedicationActiveIngredients { get; set; } = new List<MedicationActiveIngredient>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
}
