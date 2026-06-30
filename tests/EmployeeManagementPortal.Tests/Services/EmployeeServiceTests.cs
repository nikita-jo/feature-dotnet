using EmployeeManagementPortal.Application.Interfaces;
using EmployeeManagementPortal.Application.Services;
using EmployeeManagementPortal.Application.Validators;
using EmployeeManagementPortal.Domain.Entities;
using EmployeeManagementPortal.Tests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;

namespace EmployeeManagementPortal.Tests.Services;

/// <summary>
/// Unit tests for <see cref="EmployeeService"/>.
/// Uses mocked repositories to isolate the orchestration logic from EF Core.
/// </summary>
public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _repo = new(MockBehavior.Strict);
    private readonly CreateEmployeeDtoValidator _createValidator = new();
    private readonly UpdateEmployeeDtoValidator _updateValidator = new();
    private readonly TimeProvider _timeProvider = new FakeTimeProvider(new DateTimeOffset(2024, 1, 15, 12, 0, 0, TimeSpan.Zero));
    private readonly EmployeeService _sut;

    public EmployeeServiceTests()
    {
        _sut = new EmployeeService(
            _repo.Object,
            _createValidator,
            _updateValidator,
            NullLogger<EmployeeService>.Instance,
            _timeProvider);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnDto()
    {
        var entity = TestDataFactory.CreateEmployee(id: 42);
        _repo.Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var result = await _sut.GetByIdAsync(42);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(42);
        result.Value.FullName.Should().Be("Ada Lovelace");
    }

    [Fact]
    public async Task GetByIdAsync_WithMissingId_ShouldFail()
    {
        _repo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Employee?)null);

        var result = await _sut.GetByIdAsync(99);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("not found"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetByIdAsync_WithNonPositiveId_ShouldFail(int id)
    {
        var result = await _sut.GetByIdAsync(id);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_ShouldMapAllEmployees()
    {
        var entities = new[] { TestDataFactory.CreateEmployee(1), TestDataFactory.CreateEmployee(2) };
        _repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(entities);

        var result = await _sut.GetAllAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_ShouldPersistAndReturnDto()
    {
        var dto = TestDataFactory.CreateValidDto();
        _repo.Setup(r => r.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>())).ReturnsAsync((Employee?)null);
        _repo.Setup(r => r.GetByEmployeeCodeAsync(dto.EmployeeCode, It.IsAny<CancellationToken>())).ReturnsAsync((Employee?)null);
        _repo.Setup(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee e, CancellationToken _) => { e.Id = 7; return e; });

        var result = await _sut.CreateAsync(dto);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(7);
        result.Value.Email.Should().Be(dto.Email.ToLowerInvariant());
        _repo.Verify(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidDto_ShouldNotPersist()
    {
        var dto = TestDataFactory.CreateValidDto();
        dto.Email = "not-an-email";

        var result = await _sut.CreateAsync(dto);

        result.IsSuccess.Should().BeFalse();
        _repo.Verify(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateEmail_ShouldFail()
    {
        var dto = TestDataFactory.CreateValidDto();
        _repo.Setup(r => r.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestDataFactory.CreateEmployee(99));

        var result = await _sut.CreateAsync(dto);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("email already exists"));
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateEmployeeCode_ShouldFail()
    {
        var dto = TestDataFactory.CreateValidDto();
        _repo.Setup(r => r.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>())).ReturnsAsync((Employee?)null);
        _repo.Setup(r => r.GetByEmployeeCodeAsync(dto.EmployeeCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestDataFactory.CreateEmployee(101));

        var result = await _sut.CreateAsync(dto);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("code already exists"));
    }

    [Fact]
    public async Task UpdateAsync_WithValidDto_ShouldUpdateAndStamp()
    {
        var dto = TestDataFactory.CreateValidUpdate(id: 5);
        var entity = TestDataFactory.CreateEmployee(id: 5);
        _repo.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        _repo.Setup(r => r.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        _repo.Setup(r => r.GetByEmployeeCodeAsync(dto.EmployeeCode, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        _repo.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _sut.UpdateAsync(dto);

        result.IsSuccess.Should().BeTrue();
        entity.UpdatedAt.Should().Be(_timeProvider.GetUtcNow().UtcDateTime);
    }

    [Fact]
    public async Task UpdateAsync_WithMissingEntity_ShouldFail()
    {
        var dto = TestDataFactory.CreateValidUpdate(id: 5);
        _repo.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync((Employee?)null);

        var result = await _sut.UpdateAsync(dto);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_WhenAnotherEmployeeOwnsTheEmail_ShouldFail()
    {
        var dto = TestDataFactory.CreateValidUpdate(id: 5);
        var entity = TestDataFactory.CreateEmployee(id: 5);
        var other = TestDataFactory.CreateEmployee(id: 6);
        _repo.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        _repo.Setup(r => r.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(other);

        var result = await _sut.UpdateAsync(dto);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("another employee already uses this email"));
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldDelete()
    {
        var entity = TestDataFactory.CreateEmployee(id: 1);
        _repo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        _repo.Setup(r => r.DeleteAsync(entity, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _sut.DeleteAsync(1);

        result.IsSuccess.Should().BeTrue();
        _repo.Verify(r => r.DeleteAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithMissingId_ShouldFail()
    {
        _repo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Employee?)null);

        var result = await _sut.DeleteAsync(99);

        result.IsSuccess.Should().BeFalse();
    }
}
