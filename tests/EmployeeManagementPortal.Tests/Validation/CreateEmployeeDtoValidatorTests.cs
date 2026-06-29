using EmployeeManagementPortal.Application.DTOs;
using EmployeeManagementPortal.Application.Validators;
using EmployeeManagementPortal.Tests.Fakes;

namespace EmployeeManagementPortal.Tests.Validation;

/// <summary>
/// Unit tests for <see cref="CreateEmployeeDtoValidator"/>.
/// Asserts both the happy path and every individual validation rule.
/// </summary>
public class CreateEmployeeDtoValidatorTests
{
    private readonly CreateEmployeeDtoValidator _sut = new();

    [Fact]
    public void Validate_WithValidDto_ShouldHaveNoErrors()
    {
        var dto = TestDataFactory.CreateValidDto();

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("EMP 0001")] // spaces not allowed
    [InlineData("EMP/0001")] // slash not allowed
    public void Validate_WithInvalidEmployeeCode_ShouldFail(string code)
    {
        var dto = TestDataFactory.CreateValidDto();
        dto.EmployeeCode = code;

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.EmployeeCode));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithMissingFirstName_ShouldFail(string firstName)
    {
        var dto = TestDataFactory.CreateValidDto();
        dto.FirstName = firstName;

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithFirstNameExceedingLimit_ShouldFail()
    {
        var dto = TestDataFactory.CreateValidDto();
        dto.FirstName = new string('a', 65);

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("missing@tld")]
    public void Validate_WithBadEmail_ShouldFail(string email)
    {
        var dto = TestDataFactory.CreateValidDto();
        dto.Email = email;

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.Email));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-1000)]
    public void Validate_WithNegativeSalary_ShouldFail(decimal salary)
    {
        var dto = TestDataFactory.CreateValidDto();
        dto.Salary = salary;

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithSalaryExceedingLimit_ShouldFail()
    {
        var dto = TestDataFactory.CreateValidDto();
        dto.Salary = 99_999_999m;

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithFutureDateOfJoining_ShouldFail()
    {
        var dto = TestDataFactory.CreateValidDto();
        dto.DateOfJoining = DateTime.UtcNow.AddYears(5);

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithDefaultDateOfJoining_ShouldFail()
    {
        var dto = TestDataFactory.CreateValidDto();
        dto.DateOfJoining = default;

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
    }
}
