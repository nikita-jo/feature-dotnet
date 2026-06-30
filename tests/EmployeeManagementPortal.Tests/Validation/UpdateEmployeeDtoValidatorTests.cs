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

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("missing@tld")]
    public void Validate_WithBadEmail_ShouldFail(string email)
    {
        var dto = TestDataFactory.CreateValidUpdate();
        dto.Email = email;

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithFutureDateOfJoining_ShouldFail()
    {
        var dto = TestDataFactory.CreateValidUpdate();
        dto.DateOfJoining = DateTime.UtcNow.AddYears(5);

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-1000)]
    public void Validate_WithNegativeSalary_ShouldFail(decimal salary)
    {
        var dto = TestDataFactory.CreateValidUpdate();
        dto.Salary = salary;

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithFirstNameExceedingLimit_ShouldFail()
    {
        var dto = TestDataFactory.CreateValidUpdate();
        dto.FirstName = new string('a', 65);

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithDefaultDateOfJoining_ShouldFail()
    {
        var dto = TestDataFactory.CreateValidUpdate();
        dto.DateOfJoining = default;

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
    }

    // ---------- EmployeeCode ----------

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("EMP 0001")]
    [InlineData("EMP/0001")]
    public void Validate_WithInvalidEmployeeCode_ShouldFail(string code)
    {
        var dto = TestDataFactory.CreateValidUpdate();
        dto.EmployeeCode = code;

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.EmployeeCode));
    }

    [Fact]
    public void Validate_WithEmployeeCodeExceedingLimit_ShouldFail()
    {
        var dto = TestDataFactory.CreateValidUpdate();
        dto.EmployeeCode = new string('a', 33);

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.EmployeeCode));
    }

    // ---------- LastName ----------

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithMissingLastName_ShouldFail(string lastName)
    {
        var dto = TestDataFactory.CreateValidUpdate();
        dto.LastName = lastName;

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.LastName));
    }

    [Fact]
    public void Validate_WithLastNameExceedingLimit_ShouldFail()
    {
        var dto = TestDataFactory.CreateValidUpdate();
        dto.LastName = new string('a', 65);

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.LastName));
    }

    // ---------- Department ----------

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithMissingDepartment_ShouldFail(string department)
    {
        var dto = TestDataFactory.CreateValidUpdate();
        dto.Department = department;

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.Department));
    }

    [Fact]
    public void Validate_WithDepartmentExceedingLimit_ShouldFail()
    {
        var dto = TestDataFactory.CreateValidUpdate();
        dto.Department = new string('a', 65);

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.Department));
    }

    // ---------- Email length ----------

    [Fact]
    public void Validate_WithEmailExceedingLimit_ShouldFail()
    {
        var dto = TestDataFactory.CreateValidUpdate();
        dto.Email = $"{new string('a', 250)}@x.io";

        var result = _sut.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.Email));
    }
}
