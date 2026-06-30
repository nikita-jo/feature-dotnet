using EmployeeManagementPortal.Domain.Entities;
using EmployeeManagementPortal.Infrastructure.Persistence;
using EmployeeManagementPortal.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementPortal.Tests.Repositories;

/// <summary>
/// Integration-style tests for <see cref="EmployeeRepository"/> using the EF Core InMemory provider.
/// Verifies the persistence boundary behaves as expected for real DB-shaped operations.
/// </summary>
public class EmployeeRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly EmployeeRepository _sut;
    private readonly string _databaseName = Guid.NewGuid().ToString();

    public EmployeeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(_databaseName)
            .Options;
        _context = new AppDbContext(options);
        _sut = new EmployeeRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task AddAsync_ShouldPersistEmployee()
    {
        var employee = new Employee
        {
            EmployeeCode = "EMP-9001",
            FirstName = "Grace",
            LastName = "Hopper",
            Email = "grace@example.com",
            Department = "Research",
            Salary = 200_000m,
            DateOfJoining = new DateTime(2023, 6, 1)
        };

        var saved = await _sut.AddAsync(employee);

        saved.Id.Should().BeGreaterThan(0);
        (await _sut.GetByIdAsync(saved.Id)).Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ShouldReturnNull()
    {
        var result = await _sut.GetByIdAsync(999);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldBeCaseInsensitive()
    {
        await _sut.AddAsync(new Employee
        {
            EmployeeCode = "EMP-1",
            FirstName = "A",
            LastName = "B",
            Email = "Ada@Example.com",
            Department = "Engineering",
            Salary = 1m,
            DateOfJoining = DateTime.UtcNow
        });

        var result = await _sut.GetByEmailAsync("ada@example.com");

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAll()
    {
        await _sut.AddAsync(new Employee { EmployeeCode = "A", FirstName = "A", LastName = "A", Email = "a@x.com", Department = "X", Salary = 1m, DateOfJoining = DateTime.UtcNow });
        await _sut.AddAsync(new Employee { EmployeeCode = "B", FirstName = "B", LastName = "B", Email = "b@x.com", Department = "Y", Salary = 1m, DateOfJoining = DateTime.UtcNow });

        var list = await _sut.GetAllAsync();

        list.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReflectChanges()
    {
        var saved = await _sut.AddAsync(new Employee
        {
            EmployeeCode = "EMP-2",
            FirstName = "Old",
            LastName = "Name",
            Email = "o@x.com",
            Department = "D",
            Salary = 100m,
            DateOfJoining = DateTime.UtcNow
        });
        saved.FirstName = "New";

        await _sut.UpdateAsync(saved);

        var reloaded = await _sut.GetByIdAsync(saved.Id);
        reloaded!.FirstName.Should().Be("New");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        var saved = await _sut.AddAsync(new Employee
        {
            EmployeeCode = "EMP-3",
            FirstName = "X",
            LastName = "Y",
            Email = "x@x.com",
            Department = "Z",
            Salary = 50m,
            DateOfJoining = DateTime.UtcNow
        });

        await _sut.DeleteAsync(saved);

        (await _sut.GetByIdAsync(saved.Id)).Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrueForKnownId()
    {
        var saved = await _sut.AddAsync(new Employee
        {
            EmployeeCode = "EMP-4",
            FirstName = "X",
            LastName = "Y",
            Email = "x4@x.com",
            Department = "Z",
            Salary = 50m,
            DateOfJoining = DateTime.UtcNow
        });

        (await _sut.ExistsAsync(saved.Id)).Should().BeTrue();
        (await _sut.ExistsAsync(999_999)).Should().BeFalse();
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldBeCaseAndTrimInsensitive()
    {
        // Insert via raw _context to verify the *lookup* path (not the AddAsync normalizer)
        // handles mixed-case and surrounding whitespace.
        _context.Employees.Add(new Employee
        {
            EmployeeCode = "EMP-CASE",
            FirstName = "A",
            LastName = "B",
            Email = "MIXED@Example.COM",
            Department = "X",
            Salary = 1m,
            DateOfJoining = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var lower = await _sut.GetByEmailAsync("mixed@example.com");
        var trimmed = await _sut.GetByEmailAsync("  MIXED@Example.COM  ");

        lower.Should().NotBeNull();
        trimmed.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByEmployeeCodeAsync_WhenPresent_ShouldReturn()
    {
        _context.Employees.Add(new Employee
        {
            EmployeeCode = "EMP-CODE-1",
            FirstName = "A",
            LastName = "B",
            Email = "code1@x.com",
            Department = "X",
            Salary = 1m,
            DateOfJoining = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var result = await _sut.GetByEmployeeCodeAsync("  EMP-CODE-1  ");

        result.Should().NotBeNull();
        result!.Email.Should().Be("code1@x.com");
    }

    [Fact]
    public async Task GetByEmployeeCodeAsync_WhenMissing_ShouldReturnNull()
    {
        var result = await _sut.GetByEmployeeCodeAsync("NOPE-0000");

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldNormalizeEmailAndCode()
    {
        var saved = await _sut.AddAsync(new Employee
        {
            EmployeeCode = "  EMP-NORM  ",
            FirstName = "A",
            LastName = "B",
            Email = "  MIXED@Example.COM  ",
            Department = "X",
            Salary = 1m,
            DateOfJoining = DateTime.UtcNow
        });

        saved.EmployeeCode.Should().Be("EMP-NORM");
        saved.Email.Should().Be("mixed@example.com");
    }

    [Fact]
    public async Task AddAsync_WithNullEmployee_ShouldThrow()
    {
        var act = () => _sut.AddAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateAsync_WithNullEmployee_ShouldThrow()
    {
        var act = () => _sut.UpdateAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeleteAsync_WithNullEmployee_ShouldThrow()
    {
        var act = () => _sut.DeleteAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
