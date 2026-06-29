using EmployeeManagementPortal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementPortal.Infrastructure.Persistence;

/// <summary>
/// EF Core <see cref="DbContext"/> for the application.
/// Configures entity mappings and exposes the Employees DbSet.
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>Employees collection.</summary>
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}