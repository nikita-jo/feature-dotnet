using System.Reflection;
using EmployeeManagementPortal.Application.Interfaces;
using EmployeeManagementPortal.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagementPortal.Application;

/// <summary>
/// Registers Application-layer services (use cases, validators, mappers) with DI.
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IEmployeeService, EmployeeService>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);

        return services;
    }
}