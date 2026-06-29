using EmployeeManagementPortal.Application.DTOs;
using EmployeeManagementPortal.Domain.Entities;

namespace EmployeeManagementPortal.Tests.Fakes;

/// <summary>
/// Tiny test data factory. Centralizes default values so individual tests
/// only override what they care about, keeping arrange blocks compact.
/// </summary>
internal static class TestDataFactory
{
    public static CreateEmployeeDto CreateValidDto() => new()
    {
        EmployeeCode = "EMP-0001",
        FirstName = "Ada",
        LastName = "Lovelace",
        Email = "ada@example.com",
        Department = "Engineering",
        Salary = 150_000m,
        DateOfJoining = new DateTime(2024, 1, 15)
    };

    public static UpdateEmployeeDto CreateValidUpdate(int id = 1) => new()
    {
        Id = id,
        EmployeeCode = "EMP-0001",
        FirstName = "Ada",
        LastName = "Lovelace",
        Email = "ada@example.com",
        Department = "Engineering",
        Salary = 150_000m,
        DateOfJoining = new DateTime(2024, 1, 15)
    };

    public static Employee CreateEmployee(int id = 1) => new()
    {
        Id = id,
        EmployeeCode = $"EMP-{id:0000}",
        FirstName = "Ada",
        LastName = "Lovelace",
        Email = $"ada{id}@example.com",
        Department = "Engineering",
        Salary = 150_000m,
        DateOfJoining = new DateTime(2024, 1, 15),
        CreatedAt = DateTime.UtcNow.AddDays(-30)
    };
}