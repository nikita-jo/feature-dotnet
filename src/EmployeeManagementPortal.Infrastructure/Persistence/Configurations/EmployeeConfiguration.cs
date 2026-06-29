using EmployeeManagementPortal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeManagementPortal.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core fluent configuration for the <see cref="Employee"/> aggregate.
/// All schema decisions (table name, indexes, constraints) live here.
/// </summary>
public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.EmployeeCode)
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(e => e.EmployeeCode)
            .IsUnique()
            .HasDatabaseName("UX_Employees_EmployeeCode");

        builder.Property(e => e.FirstName)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(e => e.LastName)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasMaxLength(254)
            .IsRequired();

        builder.HasIndex(e => e.Email)
            .IsUnique()
            .HasDatabaseName("UX_Employees_Email");

        builder.Property(e => e.Department)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(e => e.Salary)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(e => e.DateOfJoining)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("datetime2")
            .IsRequired(false);

        builder.Ignore(e => e.FullName);
    }
}