using EmployeeManagementPortal.Application.Common;
using EmployeeManagementPortal.Application.DTOs;
using EmployeeManagementPortal.Application.Interfaces;
using EmployeeManagementPortal.Web.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace EmployeeManagementPortal.Tests.Controllers;

/// <summary>
/// Unit tests for <see cref="EmployeesController"/>.
/// Each action is exercised for the success and the failure branch using mocked service results.
/// </summary>
public class EmployeesControllerTests
{
    private readonly Mock<IEmployeeService> _service = new(MockBehavior.Strict);
    private readonly EmployeesController _sut;

    public EmployeesControllerTests()
    {
        _sut = new EmployeesController(_service.Object, NullLogger<EmployeesController>.Instance);

        // Wire a real HttpContext so Controller.TempData resolves. The dictionary
        // is backed by a tiny in-memory provider so success-path tests can write
        // TempData without a real session.
        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, new InMemoryTempDataProvider());
        _sut.ControllerContext = new ControllerContext { HttpContext = httpContext };
        _sut.TempData = tempData;
    }

    /// <summary>
    /// Minimal in-memory <see cref="ITempDataProvider"/> so unit tests can read/write TempData
    /// without standing up a session. Production still uses SessionStateTempDataProvider.
    /// </summary>
    private sealed class InMemoryTempDataProvider : ITempDataProvider
    {
        private readonly Dictionary<string, object?> _store = new();

        public IDictionary<string, object?> LoadTempData(HttpContext context) => new Dictionary<string, object?>(_store);

        public void SaveTempData(HttpContext context, IDictionary<string, object?> values)
        {
            _store.Clear();
            foreach (var kvp in values)
            {
                _store[kvp.Key] = kvp.Value;
            }
        }
    }

    [Fact]
    public async Task Index_ShouldReturnViewWithEmployees()
    {
        var employees = new List<EmployeeDto> { new() { Id = 1, FirstName = "Ada", LastName = "Lovelace" } };
        _service.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<EmployeeDto>>.Success(employees));

        var result = await _sut.Index(CancellationToken.None);

        result.Should().BeOfType<ViewResult>();
        var view = (ViewResult)result;
        view.Model.Should().BeAssignableTo<IReadOnlyList<EmployeeDto>>()
            .Which.Should().HaveCount(1);
    }

    [Fact]
    public async Task Index_WhenServiceFails_ShouldReturnEmptyList()
    {
        _service.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<EmployeeDto>>.Failure("boom"));

        var result = await _sut.Index(CancellationToken.None);

        result.Should().BeOfType<ViewResult>();
        var model = ((ViewResult)result).Model;
        model.Should().BeAssignableTo<IReadOnlyList<EmployeeDto>>()
            .Which.Should().BeEmpty();
        _sut.ModelState.ErrorCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Details_WhenFound_ShouldReturnView()
    {
        var dto = new EmployeeDto { Id = 1, FirstName = "Ada" };
        _service.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EmployeeDto>.Success(dto));

        var result = await _sut.Details(1, CancellationToken.None);

        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().Be(dto);
    }

    [Fact]
    public async Task Details_WhenMissing_ShouldReturnNotFound()
    {
        _service.Setup(s => s.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EmployeeDto>.Failure("missing"));

        var result = await _sut.Details(99, CancellationToken.None);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Create_Get_ShouldReturnEmptyForm()
    {
        var result = _sut.Create();

        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<CreateEmployeeDto>();
    }

    [Fact]
    public async Task Create_Post_OnSuccess_ShouldRedirectToIndex()
    {
        var dto = new CreateEmployeeDto { FirstName = "A" };
        _service.Setup(s => s.CreateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EmployeeDto>.Success(new EmployeeDto { Id = 1, FirstName = "A" }));

        var result = await _sut.Create(dto, CancellationToken.None);

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be(nameof(EmployeesController.Index));
    }

    [Fact]
    public async Task Create_Post_OnFailure_ShouldReturnView()
    {
        var dto = new CreateEmployeeDto();
        _service.Setup(s => s.CreateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EmployeeDto>.Failure("invalid"));

        var result = await _sut.Create(dto, CancellationToken.None);

        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().Be(dto);
        _sut.ModelState.ErrorCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Edit_Get_WhenFound_ShouldReturnView()
    {
        var dto = new EmployeeDto { Id = 5, FirstName = "A", LastName = "B", EmployeeCode = "X", Email = "x@x.com", Department = "D", Salary = 1m, DateOfJoining = DateTime.UtcNow };
        _service.Setup(s => s.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EmployeeDto>.Success(dto));

        var result = await _sut.Edit(5, CancellationToken.None);

        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<UpdateEmployeeDto>();
    }

    [Fact]
    public async Task Edit_Get_WhenMissing_ShouldReturnNotFound()
    {
        _service.Setup(s => s.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EmployeeDto>.Failure("missing"));

        var result = await _sut.Edit(5, CancellationToken.None);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Edit_Post_WhenIdMismatch_ShouldReturnBadRequest()
    {
        var dto = new UpdateEmployeeDto { Id = 5 };

        var result = await _sut.Edit(99, dto, CancellationToken.None);

        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task Edit_Post_OnSuccess_ShouldRedirect()
    {
        var dto = new UpdateEmployeeDto { Id = 5, FirstName = "X", LastName = "Y", EmployeeCode = "C", Email = "e@e.com", Department = "D", Salary = 1m, DateOfJoining = DateTime.UtcNow };
        _service.Setup(s => s.UpdateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EmployeeDto>.Success(new EmployeeDto { Id = 5, FirstName = "X" }));

        var result = await _sut.Edit(5, dto, CancellationToken.None);

        result.Should().BeOfType<RedirectToActionResult>();
    }

    [Fact]
    public async Task Delete_Get_WhenFound_ShouldReturnView()
    {
        var dto = new EmployeeDto { Id = 1, FirstName = "A" };
        _service.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<EmployeeDto>.Success(dto));

        var result = await _sut.Delete(1, CancellationToken.None);

        result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(dto);
    }

    [Fact]
    public async Task DeleteConfirmed_OnSuccess_ShouldRedirect()
    {
        _service.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _sut.DeleteConfirmed(1, CancellationToken.None);

        result.Should().BeOfType<RedirectToActionResult>();
    }

    [Fact]
    public async Task DeleteConfirmed_OnFailure_ShouldStillRedirectWithModelError()
    {
        _service.Setup(s => s.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("not found"));

        var result = await _sut.DeleteConfirmed(1, CancellationToken.None);

        result.Should().BeOfType<RedirectToActionResult>();
    }
}
