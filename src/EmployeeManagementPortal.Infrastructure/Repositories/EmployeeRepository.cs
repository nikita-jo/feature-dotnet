using EmployeeManagementPortal.Application.Interfaces;
using EmployeeManagementPortal.Domain.Entities;
using EmployeeManagementPortal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementPortal.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IEmployeeRepository"/>.
/// Hides persistence concerns from the Application layer.
/// </summary>
public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _dbContext;

    public EmployeeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return _dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Email == normalized, cancellationToken);
    }

    public Task<Employee?> GetByEmployeeCodeAsync(string employeeCode, CancellationToken cancellationToken = default)
    {
        var normalized = employeeCode.Trim();
        return _dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EmployeeCode == normalized, cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Employees
            .AsNoTracking()
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employee);

        // Normalize at the persistence boundary so callers don't have to remember.
        // Mirrors the lookup-side normalization in GetByEmailAsync / GetByEmployeeCodeAsync.
        employee.Email = employee.Email.Trim().ToLowerInvariant();
        employee.EmployeeCode = employee.EmployeeCode.Trim();

        var entry = await _dbContext.Employees.AddAsync(employee, cancellationToken).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entry.Entity;
    }

    public async Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employee);

        _dbContext.Employees.Update(employee);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(employee);

        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Employees.AnyAsync(e => e.Id == id, cancellationToken);
    }
}