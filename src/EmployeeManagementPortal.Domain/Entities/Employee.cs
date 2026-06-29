using EmployeeManagementPortal.Domain.Common;

namespace EmployeeManagementPortal.Domain.Entities;

/// <summary>
/// Represents an Employee within the organization.
/// Inherits identity and audit columns from <see cref="BaseEntity"/>.
/// </summary>
public class Employee : BaseEntity
{
    /// <summary>
    /// Business code for the employee (e.g. EMP-0001). Must be unique.
    /// </summary>
    public string EmployeeCode { get; set; } = string.Empty;

    /// <summary>
    /// First (given) name of the employee.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last (family) name of the employee.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Corporate / personal email address. Must be unique.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Department the employee belongs to (e.g. Engineering, Finance).
    /// </summary>
    public string Department { get; set; } = string.Empty;

    /// <summary>
    /// Gross annual salary in USD. Must be non-negative.
    /// </summary>
    public decimal Salary { get; set; }

    /// <summary>
    /// The day the employee joined the organization.
    /// </summary>
    public DateTime DateOfJoining { get; set; }

    /// <summary>
    /// Computed convenience: first + last name with a single space.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
}