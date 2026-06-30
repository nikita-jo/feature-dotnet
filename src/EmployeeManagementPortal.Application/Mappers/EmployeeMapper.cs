using EmployeeManagementPortal.Application.DTOs;
using EmployeeManagementPortal.Domain.Entities;

namespace EmployeeManagementPortal.Application.Mappers;

/// <summary>
/// Pure mapping between <see cref="Employee"/> domain entities and DTOs.
/// No state, no side effects — easy to unit test.
/// </summary>
public static class EmployeeMapper
{
    /// <summary>
    /// Project a domain <see cref="Employee"/> into a read-only <see cref="EmployeeDto"/>.
    /// </summary>
    public static EmployeeDto ToDto(Employee entity) => new()
    {
        Id = entity.Id,
        EmployeeCode = entity.EmployeeCode,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        FullName = entity.FullName,
        Email = entity.Email,
        Department = entity.Department,
        Salary = entity.Salary,
        DateOfJoining = entity.DateOfJoining,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    /// <summary>
    /// Convert a <see cref="CreateEmployeeDto"/> into a new domain entity.
    /// </summary>
    public static Employee ToEntity(CreateEmployeeDto dto) => new()
    {
        EmployeeCode = dto.EmployeeCode.Trim(),
        FirstName = dto.FirstName.Trim(),
        LastName = dto.LastName.Trim(),
        Email = dto.Email.Trim().ToLowerInvariant(),
        Department = dto.Department.Trim(),
        Salary = dto.Salary,
        DateOfJoining = dto.DateOfJoining
    };

    /// <summary>
    /// Apply a <see cref="UpdateEmployeeDto"/> onto an existing tracked entity.
    /// </summary>
    public static void ApplyUpdate(Employee entity, UpdateEmployeeDto dto)
    {
        entity.EmployeeCode = dto.EmployeeCode.Trim();
        entity.FirstName = dto.FirstName.Trim();
        entity.LastName = dto.LastName.Trim();
        entity.Email = dto.Email.Trim().ToLowerInvariant();
        entity.Department = dto.Department.Trim();
        entity.Salary = dto.Salary;
        entity.DateOfJoining = dto.DateOfJoining;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}