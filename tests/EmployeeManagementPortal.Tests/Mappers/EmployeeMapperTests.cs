using EmployeeManagementPortal.Application.DTOs;
using EmployeeManagementPortal.Application.Mappers;
using EmployeeManagementPortal.Domain.Entities;

namespace EmployeeManagementPortal.Tests.Mappers;

/// <summary>
/// Unit tests for the pure <see cref="EmployeeMapper"/> functions.
/// </summary>
public class EmployeeMapperTests
{
    [Fact]
    public void ToDto_ShouldCopyAllFields()
    {
        var entity = new Employee
        {
            Id = 1,
            EmployeeCode = "EMP-1",
            FirstName = "Ada",
            LastName = "Lovelace",
            Email = "Ada@Example.com",
            Department = "Engineering",
            Salary = 123_456.78m,
            DateOfJoining = new DateTime(2024, 1, 15),
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 2, 1)
        };

        var dto = EmployeeMapper.ToDto(entity);

        dto.Id.Should().Be(1);
        dto.EmployeeCode.Should().Be("EMP-1");
        dto.FullName.Should().Be("Ada Lovelace");
        dto.Email.Should().Be("Ada@Example.com");
    }

    [Fact]
    public void ToEntity_ShouldTrimAndLowercaseEmail()
    {
        var dto = new CreateEmployeeDto
        {
            EmployeeCode = "  EMP-1  ",
            FirstName = "  Ada  ",
            LastName = "  Lovelace  ",
            Email = "  ADA@Example.COM  ",
            Department = "  Eng  ",
            Salary = 1m,
            DateOfJoining = DateTime.UtcNow
        };

        var entity = EmployeeMapper.ToEntity(dto);

        entity.EmployeeCode.Should().Be("EMP-1");
        entity.FirstName.Should().Be("Ada");
        entity.LastName.Should().Be("Lovelace");
        entity.Email.Should().Be("ada@example.com");
        entity.Department.Should().Be("Eng");
    }

    [Fact]
    public void ApplyUpdate_ShouldStampUpdatedAt()
    {
        var entity = new Employee { Id = 1, Email = "old@x.com" };
        var dto = new UpdateEmployeeDto
        {
            Id = 1,
            EmployeeCode = "EMP-1",
            FirstName = "A",
            LastName = "B",
            Email = "new@x.com",
            Department = "D",
            Salary = 1m,
            DateOfJoining = DateTime.UtcNow
        };
        var before = DateTime.UtcNow;

        EmployeeMapper.ApplyUpdate(entity, dto);

        entity.Email.Should().Be("new@x.com");
        entity.UpdatedAt.Should().NotBeNull().And.BeOnOrAfter(before);
    }
}
