using EmployeeManagementPortal.Application.DTOs;
using FluentValidation;
using FluentValidation.Validators;

namespace EmployeeManagementPortal.Application.Validators;

/// <summary>
/// FluentValidation rules for updating an employee. The id must be positive,
/// all other rules mirror the create validator so the two paths stay symmetric.
/// </summary>
public sealed class UpdateEmployeeDtoValidator : AbstractValidator<UpdateEmployeeDto>
{
    public UpdateEmployeeDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("A valid employee id is required.");

        RuleFor(x => x.EmployeeCode)
            .NotEmpty().WithMessage("Employee code is required.")
            .MaximumLength(32).WithMessage("Employee code must not exceed 32 characters.")
            .Matches("^[A-Za-z0-9_-]+$").WithMessage("Employee code may contain letters, digits, '-' or '_' only.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(64).WithMessage("First name must not exceed 64 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(64).WithMessage("Last name must not exceed 64 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress(mode: EmailValidationMode.AspNetCoreCompatible)
                .WithMessage("A valid email address is required.")
            .Must(email => email!.Contains('@') && email.Split('@')[1].Contains('.'))
                .WithMessage("Email must include a domain with a dot.")
            .MaximumLength(254).WithMessage("Email must not exceed 254 characters.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.")
            .MaximumLength(64).WithMessage("Department must not exceed 64 characters.");

        RuleFor(x => x.Salary)
            .GreaterThanOrEqualTo(0).WithMessage("Salary must be non-negative.")
            .LessThanOrEqualTo(10_000_000m).WithMessage("Salary is unrealistically high.");

        RuleFor(x => x.DateOfJoining)
            .NotEqual(default(DateTime)).WithMessage("Date of joining is required.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
                .WithMessage("Date of joining cannot be in the future.");
    }
}