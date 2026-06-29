using EmployeeManagementPortal.Application.Interfaces;
using EmployeeManagementPortal.Infrastructure.Persistence;
using EmployeeManagementPortal.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagementPortal.Infrastructure;

/// <summary>
/// Composition root for the Infrastructure layer.
/// Wires EF Core (SQL Server by default, InMemory for tests) and the Employee repository.
/// </summary>
public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name);
                sql.EnableRetryOnFailure(maxRetryCount: 3);
            }));

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        return services;
    }

    /// <summary>
    /// InMemory variant used by automated tests; avoids needing a real SQL instance.
    /// </summary>
    public static IServiceCollection AddInMemoryPersistence(this IServiceCollection services, string dbName)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(dbName));
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        return services;
    }
}