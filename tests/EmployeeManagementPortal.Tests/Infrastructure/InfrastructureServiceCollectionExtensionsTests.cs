using EmployeeManagementPortal.Application.Interfaces;
using EmployeeManagementPortal.Infrastructure;
using EmployeeManagementPortal.Infrastructure.Persistence;
using EmployeeManagementPortal.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagementPortal.Tests.Infrastructure;

/// <summary>
/// Sanity tests for the Infrastructure-layer DI registration. The SQL Server
/// registration path is exercised through argument validation since we can't
/// stand up a real database in unit tests; the InMemory variant is exercised
/// end-to-end via the service provider.
/// </summary>
public class InfrastructureServiceCollectionExtensionsTests
{
    [Fact]
    public void AddInMemoryPersistence_ShouldRegisterRepositoryAndDbContext()
    {
        var services = new ServiceCollection();

        services.AddInMemoryPersistence("test-db");

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        scope.ServiceProvider.GetRequiredService<IEmployeeRepository>()
            .Should().BeOfType<EmployeeRepository>();

        // Resolve the DbContext via the same scoped root to confirm registration works.
        scope.ServiceProvider.GetRequiredService<AppDbContext>()
            .Database.IsInMemory().Should().BeTrue();
    }

    [Fact]
    public void AddInMemoryPersistence_WithNullServices_ShouldThrow()
    {
        IServiceCollection? services = null;

        var act = () => services!.AddInMemoryPersistence("db");

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddInfrastructure_WithoutConnectionString_ShouldThrow()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var act = () => services.AddInfrastructure(configuration);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*DefaultConnection*");
    }

    [Fact]
    public void AddInfrastructure_WithNullServices_ShouldThrow()
    {
        IServiceCollection? services = null;
        var configuration = new ConfigurationBuilder().Build();

        var act = () => services!.AddInfrastructure(configuration);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddInfrastructure_WithNullConfiguration_ShouldThrow()
    {
        var services = new ServiceCollection();

        var act = () => services.AddInfrastructure(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
