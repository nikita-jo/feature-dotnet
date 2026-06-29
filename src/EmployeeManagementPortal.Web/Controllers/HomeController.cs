using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementPortal.Web.Controllers;

/// <summary>
/// Default landing page controller. Pure read-only, no business logic.
/// </summary>
public sealed class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult Privacy() => View();

    [HttpGet]
    public IActionResult Error() => View();
}