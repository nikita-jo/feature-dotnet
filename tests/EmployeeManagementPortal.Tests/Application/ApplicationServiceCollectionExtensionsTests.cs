using EmployeeManagementPortal.Application;
using EmployeeManagementPortal.Application.DTOs;
using EmployeeManagementPortal.Application.Interfaces;
using EmployeeManagementPortal.Application.Services;
using EmployeeManagementPortal.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagementPortal.Tests.Application;

/// <summary>
/// Sanity tests for the Application-layer DI registration. Catches regressions
/// in service lifetimes or missing registrations without spinning up a host.
/// </summary>
public class ApplicationServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationServices_ShouldRegisterEmployeeService()
    {
        var services = new ServiceCollection();

        services.AddApplicationServices();

        using var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IEmployeeService>();

        service.Should().BeOfType<EmployeeService>();
    }

    [Fact]
    public void AddApplicationServices_ShouldRegisterValidatorsFromAssembly()
    {
        var services = new ServiceCollection();

        services.AddApplicationServices();

        using var provider = services.BuildServiceProvider();
        var createValidator = provider.GetRequiredService<IValidator<CreateEmployeeDto>>();
        var updateValidator = provider.GetRequiredService<IValidator<UpdateEmployeeDto>>();

        createValidator.Should().BeOfType<CreateEmployeeDtoValidator>();
        updateValidator.Should().BeOfType<UpdateEmployeeDtoValidator>();
    }

    [Fact]
    public void AddApplicationServices_WithNullServices_ShouldThrow()
    {
        IServiceCollection? services = null;

        var act = () => services!.AddApplicationServices();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddApplicationServices_ShouldReturnSameCollectionForChaining()
    {
        var services = new ServiceCollection();

        var result = services.AddApplicationServices();

        result.Should().BeSameAs(services);
    }
}
