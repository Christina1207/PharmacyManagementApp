using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Entities;

public partial class Prescription
{
    public int Id { get; set; }

    public decimal TotalValue { get; set; }

    public DateTime DispenseDate { get; set; }

    public DateOnly? IssueDate { get; set; }

    public int DiagnosisId { get; set; }

    public int UserId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public virtual Diagnosis Diagnosis { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual InsuredPerson Patient { get; set; } = null!;

    public virtual ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual User User { get; set; } = null!;
}
