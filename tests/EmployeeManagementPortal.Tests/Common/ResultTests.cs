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
        result.Errors.Should().ContainSingle().Which.Should().Be("Unknown error");
    }

    [Fact]
    public void Failure_WithMultipleMessages_ShouldExposeAll()
    {
        var result = Result<int>.Failure(new[] { "first", "second", "third" });

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().Contain(new[] { "first", "second", "third" });
    }

    [Fact]
    public void Failure_WithReadOnlyList_ShouldExposeIt()
    {
        IReadOnlyList<string> errors = new[] { "alpha", "beta" };

        var result = Result<string>.Failure(errors);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void Failure_WithEmptyReadOnlyList_ShouldExposeDefaultMessage()
    {
        var result = Result<int>.Failure(Array.Empty<string>() as IReadOnlyList<string>);

        result.Errors.Should().ContainSingle().Which.Should().Be("Unknown error");
    }
}
