using EmployeeManagementPortal.Application.Common;
using EmployeeManagementPortal.Application.DTOs;

namespace EmployeeManagementPortal.Application.Interfaces;

/// <summary>
/// Application service exposing use cases for Employee management.
/// Returns a <see cref="Result{T}"/> so callers can branch on success/failure
/// without relying on exceptions for control flow.
/// </summary>
public interface IEmployeeService
{
    Task<Result<EmployeeDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<EmployeeDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<EmployeeDto>> CreateAsync(CreateEmployeeDto dto, CancellationToken cancellationToken = default);
    Task<Result<EmployeeDto>> UpdateAsync(UpdateEmployeeDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}