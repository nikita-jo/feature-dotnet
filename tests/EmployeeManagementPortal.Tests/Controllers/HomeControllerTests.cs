using EmployeeManagementPortal.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementPortal.Tests.Controllers;

/// <summary>
/// Unit tests for <see cref="HomeController"/>. Each action just returns a ViewResult,
/// but the actions still count as uncovered lines otherwise.
/// </summary>
public class HomeControllerTests
{
    private readonly HomeController _sut = new();

    [Fact]
    public void Index_ShouldReturnView()
    {
        var result = _sut.Index();

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Privacy_ShouldReturnView()
    {
        var result = _sut.Privacy();

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Error_ShouldReturnView()
    {
        var result = _sut.Error();

        result.Should().BeOfType<ViewResult>();
    }
}