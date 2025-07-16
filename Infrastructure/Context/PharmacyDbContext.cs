using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public partial class PharmacyDbContext : IdentityDbContext<User, Role, int>
{
    public PharmacyDbContext()
    {
    }

    public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActiveIngredient> ActiveIngredients { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<Diagnosis> Diagnoses { get; set; }
    public virtual DbSet<Doctor> Doctors { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<FamilyMember> FamilyMembers { get; set; }
    public virtual DbSet<InsuredPerson> InsuredPeople { get; set; }
    public virtual DbSet<InventoryCheck> InventoryChecks { get; set; }
    public virtual DbSet<InventoryCheckItem> InventoryCheckItems { get; set; }
    public virtual DbSet<InventoryItem> InventoryItems { get; set; }
    public virtual DbSet<InventoryItemDetail> InventoryItemDetails { get; set; }
    public virtual DbSet<Manufacturer> Manufacturers { get; set; }
    public virtual DbSet<Medication> Medications { get; set; }
    public virtual DbSet<MedicationActiveIngredient> MedicationActiveIngredients { get; set; }
    public virtual DbSet<MedicationClass> MedicationClasses { get; set; }
    public virtual DbSet<MedicationForm> MedicationForms { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }
    public virtual DbSet<Prescription> Prescriptions { get; set; }
    public virtual DbSet<PrescriptionItem> PrescriptionItems { get; set; }
    public virtual DbSet<Sale> Sales { get; set; }
    public virtual DbSet<Supplier> Suppliers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ActiveIngredient>(entity =>
        {
            entity.ToTable("ActiveIngredient");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Diagnosis>(entity =>
        {
            entity.ToTable("Diagnosis");

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("Doctor");

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.PhoneNumber)
                .IsRequired(false)
                .HasMaxLength(10)
                .IsFixedLength()
                .IsUnicode(false);

            entity.Property(e => e.Speciality)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.InsuredPersonId);

            entity.ToTable("Employee");

            entity.HasKey(e => e.InsuredPersonId);
            entity.Property(e => e.InsuredPersonId).ValueGeneratedOnAdd();
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsRequired(false);

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Department");

            entity.HasOne(d => d.InsuredPerson).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.InsuredPersonId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_InsuredPerson");
        });

        modelBuilder.Entity<FamilyMember>(entity =>
        {
            entity.HasKey(e => e.InsuredPersonId);

            entity.ToTable("FamilyMember");

            entity.HasKey(e => e.InsuredPerson);

            entity.Property(e => e.InsuredPersonId).ValueGeneratedOnAdd();
            entity.Property(e => e.Relationship)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.InsuredPerson).WithOne(p => p.FamilyMember)
                .HasForeignKey<FamilyMember>(d => d.InsuredPersonId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FamilyMember_InsuredPerson");

            entity.HasOne(d => d.Employee).WithMany(p => p.FamilyMembers)
                .HasForeignKey(d => d.EmployeeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FamilyMember_Employee");
        });

        modelBuilder.Entity<InsuredPerson>(entity =>
        {
            entity.ToTable("InsuredPerson");

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DateOfBirth)
                .IsRequired()
                .HasColumnType("date");


        });

        modelBuilder.Entity<InventoryCheck>(entity =>
        {
            entity.ToTable("InventoryCheck");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .IsUnicode(false)
                .IsRequired(false);
                
            entity.Property(e => e.TotalValue).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.InventoryChecks)
                .HasForeignKey(d => d.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryCheck_User");
        });

        modelBuilder.Entity<InventoryCheckItem>(entity =>
        {
            entity.ToTable("InventoryCheckItem");

            entity.Property(e => e.ExpectedQuantity)
                  .IsRequired(); 
            entity.Property(e => e.CountedQuantity)
                  .IsRequired(); 

                entity.HasOne(d => d.InventoryCheck).WithMany(p => p.InventoryCheckItems)
                .HasForeignKey(d => d.InventoryCheckId)
                .HasConstraintName("FK_InventoryCheckItem_InventoryCheck");

            entity.HasOne(d => d.Medication).WithMany(p => p.InventoryCheckItems)
                .HasForeignKey(d => d.MedicationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryCheckItem_Medication");
        });

        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.ToTable("InventoryItem");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)")
                  .IsRequired();

            entity.HasOne(d => d.Medication).WithMany(p => p.InventoryItems)
                .HasForeignKey(d => d.MedicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .IsRequired()
                .HasConstraintName("FK_InventoryItem_Medication");
        });

        modelBuilder.Entity<InventoryItemDetail>(entity =>
        {
            entity.ToTable("InventoryItemDetail");

            entity.Property(e => e.Quantity).IsRequired();

            entity.Property(e => e.ExpirationDate)
                .IsRequired()
                .HasColumnType("date");

            entity.HasOne(d => d.Item).WithMany(p => p.InventoryItemDetails)
                .HasForeignKey(d => d.ItemId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryItemDetail_InventoryItem");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.ToTable("Manufacturer");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .IsRequired(false)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Medication>(entity =>
        {
            entity.ToTable("Medication");

            entity.Property(e => e.MinQuantity)
                .IsRequired();

            entity.Property(e => e.Barcode)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Dose)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired()
                .IsUnicode(false);

            entity.HasOne(d => d.Class).WithMany(p => p.Medications)
                .HasForeignKey(d => d.ClassId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Medication_MedicationClass");

            entity.HasOne(d => d.Form).WithMany(p => p.Medications)
                .HasForeignKey(d => d.FormId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Medication_MedicationForm");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Medications)
                .HasForeignKey(d => d.ManufacturerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Medication_Manufacturer");
        });

        modelBuilder.Entity<MedicationActiveIngredient>(entity =>
        {
            entity.ToTable("MedicationActiveIngredient");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)")
                .IsRequired();


            entity.HasOne(d => d.Ingredient).WithMany(p => p.MedicationActiveIngredients)
                .HasForeignKey(d => d.IngredientId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MedicationActiveIngredient_ActiveIngredient");

            entity.HasOne(d => d.Medication).WithMany(p => p.MedicationActiveIngredients)
                .HasForeignKey(d => d.MedicationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MedicationActiveIngredient_Medication");
        });

        modelBuilder.Entity<MedicationClass>(entity =>
        {
            entity.ToTable("MedicationClass");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsRequired(false)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MedicationForm>(entity =>
        {
            entity.ToTable("MedicationForm");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Unit)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.OrderDate).HasColumnType("datetime")
                  .IsRequired();
            entity.Property(e => e.TotalValue).HasColumnType("decimal(18, 2)")
                  .IsRequired();

            entity.HasOne(d => d.Supplier).WithMany(p => p.Orders)
                .HasForeignKey(d => d.SupplierId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Supplier");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_User");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItem");

            entity.Property(e => e.Quantity)
                  .IsRequired();

            entity.Property(e => e.ExpirationDate)
                .IsRequired()
                .HasColumnType("date");

            entity.Property(e => e.UnitPrice)
                 .HasColumnType("decimal(18, 2)")
                 .IsRequired();

            entity.HasOne(d => d.Medication).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.MedicationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_Medication");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .IsRequired()
                .HasConstraintName("FK_OrderItem_Order");
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.ToTable("Prescription");

            entity.Property(e => e.DispenseDate)
                .IsRequired()
                .HasColumnType("datetime");
            entity.Property(e => e.TotalValue)
                .IsRequired()
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.IssueDate)
                  .IsRequired(false)
                  .HasColumnType("date");

            entity.HasOne(d => d.Diagnosis).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.DiagnosisId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prescription_Diagnosis");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.DoctorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prescription_Doctor");

            entity.HasOne(d => d.Patient).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.PatientId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prescription_InsuredPerson");

            entity.HasOne(d => d.User).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prescription_User");
        });

        modelBuilder.Entity<PrescriptionItem>(entity =>
        {
            entity.ToTable("PrescriptionItem");

            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)")
                  .IsRequired();

            entity.Property(e => e.Quantity).IsRequired();

            entity.HasOne(d => d.Medication).WithMany(p => p.PrescriptionItems)
                .HasForeignKey(d => d.MedicationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrescriptionItem_Medication");

            entity.HasOne(d => d.Prescription).WithMany(p => p.PrescriptionItems)
                .IsRequired()
                .HasForeignKey(d => d.PrescriptionId)
                .HasConstraintName("FK_PrescriptionItem_Prescription");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("Sale");

            entity.Property(e => e.AmountReceived).HasColumnType("decimal(18, 2)")
                  .IsRequired();
            entity.Property(e => e.Discount).HasColumnType("decimal(18, 2)")
                  .IsRequired();
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)")
                  .IsRequired();

            entity.HasOne(d => d.Prescription).WithMany(p => p.Sales)
                .HasForeignKey(d => d.PrescriptionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sale_Prescription");

            entity.HasOne(d => d.User).WithMany(p => p.Sales)
                .HasForeignKey(d => d.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sale_User");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("Supplier");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode(false);

           
        });

        modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
        modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");



        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
