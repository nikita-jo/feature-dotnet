using EmployeeManagementPortal.Application.DTOs;
using EmployeeManagementPortal.Application.Interfaces;
using EmployeeManagementPortal.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementPortal.Web.Controllers;

/// <summary>
/// MVC controller exposing CRUD endpoints for the Employee aggregate.
/// All endpoints live under /Employees and use POST-Redirect-GET to avoid duplicate form submissions.
/// </summary>
public sealed class EmployeesController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
    {
        _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // GET: /Employees
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetAllAsync(cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccess)
        {
            result.AddErrorsToModelState(ModelState);
        }
        return View(result.IsSuccess ? result.Value : Array.Empty<EmployeeDto>());
    }

    // GET: /Employees/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccess)
        {
            return NotFound();
        }
        return View(result.Value);
    }

    // GET: /Employees/Create
    [HttpGet]
    public IActionResult Create() => View(new CreateEmployeeDto { DateOfJoining = DateTime.UtcNow.Date });

    // POST: /Employees/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateEmployeeDto dto, CancellationToken cancellationToken)
    {
        var result = await _employeeService.CreateAsync(dto, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccess)
        {
            result.AddErrorsToModelState(ModelState);
            return View(dto);
        }

        TempData["SuccessMessage"] = $"Employee '{result.Value!.FullName}' was created.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Employees/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        var existing = result.Value!;
        var updateDto = new UpdateEmployeeDto
        {
            Id = existing.Id,
            EmployeeCode = existing.EmployeeCode,
            FirstName = existing.FirstName,
            LastName = existing.LastName,
            Email = existing.Email,
            Department = existing.Department,
            Salary = existing.Salary,
            DateOfJoining = existing.DateOfJoining
        };
        return View(updateDto);
    }

    // POST: /Employees/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateEmployeeDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        var result = await _employeeService.UpdateAsync(dto, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccess)
        {
            result.AddErrorsToModelState(ModelState);
            return View(dto);
        }

        TempData["SuccessMessage"] = $"Employee '{result.Value!.FullName}' was updated.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Employees/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccess)
        {
            return NotFound();
        }
        return View(result.Value);
    }

    // POST: /Employees/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccess)
        {
            result.AddErrorsToModelState(ModelState);
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "Employee was deleted.";
        return RedirectToAction(nameof(Index));
    }
}