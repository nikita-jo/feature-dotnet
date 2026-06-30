using EmployeeManagementPortal.Application.Common;
using EmployeeManagementPortal.Web.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EmployeeManagementPortal.Tests.Extensions;

/// <summary>
/// Unit tests for <see cref="ResultExtensions.AddErrorsToModelState{T}"/>.
/// Covers the success short-circuit, the failure-with-errors branch, the
/// keyPrefix handling, and the null modelState guard.
/// </summary>
public class ResultExtensionsTests
{
    [Fact]
    public void AddErrorsToModelState_OnSuccess_ShouldReturnFalseAndNotAdd()
    {
        var result = Result<int>.Success(42);
        var modelState = new ModelStateDictionary();

        var added = result.AddErrorsToModelState(modelState);

        added.Should().BeFalse();
        modelState.ErrorCount.Should().Be(0);
    }

    [Fact]
    public void AddErrorsToModelState_OnFailureWithZeroArgParams_ShouldAddUnknownError()
    {
        // Result.Failure() with no args collapses to ["Unknown error"] per Result<T>'s contract.
        // That single error must still flow into ModelState.
        var result = Result<int>.Failure();
        var modelState = new ModelStateDictionary();

        var added = result.AddErrorsToModelState(modelState);

        added.Should().BeTrue();
        modelState.ErrorCount.Should().Be(1);
        modelState.Should().Contain(e => e.Value!.Errors.Any(x => x.ErrorMessage == "Unknown error"));
    }

    [Fact]
    public void AddErrorsToModelState_OnFailureWithErrors_ShouldAddAndReturnTrue()
    {
        var result = Result<int>.Failure("first", "second");
        var modelState = new ModelStateDictionary();

        var added = result.AddErrorsToModelState(modelState);

        added.Should().BeTrue();
        modelState.ErrorCount.Should().Be(2);
        modelState.Should().Contain(e => e.Value!.Errors.Any(x => x.ErrorMessage == "first"));
        modelState.Should().Contain(e => e.Value!.Errors.Any(x => x.ErrorMessage == "second"));
    }

    [Fact]
    public void AddErrorsToModelState_WithKeyPrefix_ShouldUsePrefixAsKey()
    {
        var result = Result<int>.Failure("boom");
        var modelState = new ModelStateDictionary();

        var added = result.AddErrorsToModelState(modelState, "Employee");

        added.Should().BeTrue();
        modelState.Should().ContainKey("Employee");
        modelState["Employee"]!.Errors.Should().ContainSingle().Which.ErrorMessage.Should().Be("boom");
    }

    [Fact]
    public void AddErrorsToModelState_WithNullModelState_ShouldThrow()
    {
        var result = Result<int>.Failure("boom");

        var act = () => result.AddErrorsToModelState(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}