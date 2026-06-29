using EmployeeManagementPortal.Application.Common;

namespace EmployeeManagementPortal.Tests.Common;

public class ResultTests
{
    [Fact]
    public void Success_ShouldExposeValue()
    {
        var result = Result<int>.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Failure_WithSingleMessage_ShouldExposeIt()
    {
        var result = Result<int>.Failure("nope");

        result.IsSuccess.Should().BeFalse();
        result.Value.Should().Be(default);
        result.Errors.Should().ContainSingle().Which.Should().Be("nope");
    }

    [Fact]
    public void Failure_WithEmptyArray_ShouldExposeDefaultMessage()
    {
        var result = Result<int>.Failure(Array.Empty<string>());

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }
}
