using Domain.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public partial class PharmacyDbContext : IdentityDbContext
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

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=CHRISTINA;Initial Catalog=PharmacyDB;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActiveIngredient>(entity =>
        {
            entity.ToTable("ActiveIngredient");

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Diagnosis>(entity =>
        {
            entity.ToTable("Diagnosis");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("Doctor");

            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Speciality)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.InsuredPersonId);

            entity.ToTable("Employee");

            entity.Property(e => e.InsuredPersonId).ValueGeneratedOnAdd();
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Department");

            entity.HasOne(d => d.InsuredPerson).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.InsuredPersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_InsuredPerson");
        });

        modelBuilder.Entity<FamilyMember>(entity =>
        {
            entity.HasKey(e => e.CoveredPersonId);

            entity.ToTable("FamilyMember");

            entity.Property(e => e.CoveredPersonId).ValueGeneratedOnAdd();
            entity.Property(e => e.Relationship)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.CoveredPerson).WithOne(p => p.FamilyMember)
                .HasForeignKey<FamilyMember>(d => d.CoveredPersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FamilyMember_InsuredPerson");

            entity.HasOne(d => d.Employee).WithMany(p => p.FamilyMembers)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FamilyMember_Employee");
        });

        modelBuilder.Entity<InsuredPerson>(entity =>
        {
            entity.ToTable("InsuredPerson");

            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<InventoryCheck>(entity =>
        {
            entity.ToTable("InventoryCheck");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TotalValue).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.InventoryChecks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryCheck_User");
        });

        modelBuilder.Entity<InventoryCheckItem>(entity =>
        {
            entity.ToTable("InventoryCheckItem");

            entity.HasOne(d => d.InventoryCheck).WithMany(p => p.InventoryCheckItems)
                .HasForeignKey(d => d.InventoryCheckId)
                .HasConstraintName("FK_InventoryCheckItem_InventoryCheck");

            entity.HasOne(d => d.Medication).WithMany(p => p.InventoryCheckItems)
                .HasForeignKey(d => d.MedicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryCheckItem_Medication");
        });

        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.ToTable("InventoryItem");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Medication).WithMany(p => p.InventoryItems)
                .HasForeignKey(d => d.MedicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryItem_Medication");
        });

        modelBuilder.Entity<InventoryItemDetail>(entity =>
        {
            entity.ToTable("InventoryItemDetail");

            entity.HasOne(d => d.Item).WithMany(p => p.InventoryItemDetails)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryItemDetail_InventoryItem");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.ToTable("Manufacturer");

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Medication>(entity =>
        {
            entity.ToTable("Medication");

            entity.Property(e => e.Barcode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Dose)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Class).WithMany(p => p.Medications)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Medication_MedicationClass");

            entity.HasOne(d => d.Form).WithMany(p => p.Medications)
                .HasForeignKey(d => d.FormId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Medication_MedicationForm");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Medications)
                .HasForeignKey(d => d.ManufacturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Medication_Manufacturer");
        });

        modelBuilder.Entity<MedicationActiveIngredient>(entity =>
        {
            entity.ToTable("MedicationActiveIngredient");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.MedicationActiveIngredients)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MedicationActiveIngredient_ActiveIngredient");

            entity.HasOne(d => d.Medication).WithMany(p => p.MedicationActiveIngredients)
                .HasForeignKey(d => d.MedicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MedicationActiveIngredient_Medication");
        });

        modelBuilder.Entity<MedicationClass>(entity =>
        {
            entity.ToTable("MedicationClass");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MedicationForm>(entity =>
        {
            entity.ToTable("MedicationForm");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.TotalValue).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Orders)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Supplier");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_User");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItem");

            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Medication).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.MedicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_Medication");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderItem_Order");
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.ToTable("Prescription");

            entity.Property(e => e.DispenseDate).HasColumnType("datetime");
            entity.Property(e => e.TotalValue).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Diagnosis).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.DiagnosisId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prescription_Diagnosis");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prescription_Doctor");

            entity.HasOne(d => d.Patient).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prescription_InsuredPerson");

            entity.HasOne(d => d.User).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prescription_User");
        });

        modelBuilder.Entity<PrescriptionItem>(entity =>
        {
            entity.ToTable("PrescriptionItem");

            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Medication).WithMany(p => p.PrescriptionItems)
                .HasForeignKey(d => d.MedicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrescriptionItem_Medication");

            entity.HasOne(d => d.Prescription).WithMany(p => p.PrescriptionItems)
                .HasForeignKey(d => d.PrescriptionId)
                .HasConstraintName("FK_PrescriptionItem_Prescription");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role", tb => tb.HasComment("User Role"));

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("Sale");

            entity.Property(e => e.AmountReceived).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Discount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Prescription).WithMany(p => p.Sales)
                .HasForeignKey(d => d.PrescriptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sale_Prescription");

            entity.HasOne(d => d.User).WithMany(p => p.Sales)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sale_User");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("Supplier");

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
