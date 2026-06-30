using EmployeeManagementPortal.Domain.Entities;

namespace EmployeeManagementPortal.Tests.Domain;

/// <summary>
/// Unit tests for the <see cref="Employee"/> aggregate and its base class.
/// </summary>
public class EmployeeTests
{
    [Fact]
    public void FullName_WithBothNames_ShouldJoinWithSpace()
    {
        var employee = new Employee { FirstName = "Ada", LastName = "Lovelace" };

        employee.FullName.Should().Be("Ada Lovelace");
    }

    [Fact]
    public void FullName_WithOnlyFirstName_ShouldReturnItTrimmed()
    {
        var employee = new Employee { FirstName = "Ada", LastName = string.Empty };

        employee.FullName.Should().Be("Ada");
    }

    [Fact]
    public void FullName_WithOnlyLastName_ShouldReturnItTrimmed()
    {
        var employee = new Employee { FirstName = string.Empty, LastName = "Lovelace" };

        employee.FullName.Should().Be("Lovelace");
    }

    [Fact]
    public void BaseEntity_Defaults_ShouldHaveZeroIdAndUtcCreatedAt()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var entity = new Employee();

        entity.Id.Should().Be(0);
        entity.UpdatedAt.Should().BeNull();
        entity.CreatedAt.Should().BeOnOrAfter(before);
    }
}
