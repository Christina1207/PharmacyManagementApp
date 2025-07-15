using Microsoft.AspNetCore.Identity;


namespace Domain.Entities;

public partial class User :IdentityUser<int>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public int RoleId { get; set; }

    public virtual Role? Role { get; set; }
    public virtual ICollection<InventoryCheck> InventoryChecks { get; set; } = new List<InventoryCheck>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

}
