using EmployeeManagementPortal.Application.Validators;
using EmployeeManagementPortal.Tests.Fakes;

namespace EmployeeManagementPortal.Tests.Validation;

/// <summary>
/// Unit tests for <see cref="UpdateEmployeeDtoValidator"/>.
/// Adds id-specific rules on top of the same constraints exercised for create.
/// </summary>
public class UpdateEmployeeDtoValidatorTests
{
    private readonly UpdateEmployeeDtoValidator _sut = new();

    [Fact]
    public void Validate_WithValidDto_ShouldHaveNoErrors()
    {
        var dto = TestDataFactory.CreateValidUpdate();

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositiveId_ShouldFail(int id)
    {
        var dto = TestDataFactory.CreateValidUpdate(id);

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.Id));
    }

    [Fact]
    public void Validate_WithMissingEmail_ShouldFail()
    {
        var dto = TestDataFactory.CreateValidUpdate();
        dto.Email = string.Empty;

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithMissingDepartment_ShouldFail()
    {
        var dto = TestDataFactory.CreateValidUpdate();
        dto.Department = string.Empty;

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
    }
}
