using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Entities;

public partial class Employee
{
    public int InsuredPersonId { get; set; }

    public int DepartmentId { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<FamilyMember> FamilyMembers { get; set; } = new List<FamilyMember>();

    public virtual InsuredPerson InsuredPerson { get; set; } = null!;
}
