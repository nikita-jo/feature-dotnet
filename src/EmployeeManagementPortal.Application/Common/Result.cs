namespace EmployeeManagementPortal.Application.Common;

/// <summary>
/// Discriminated union that conveys success or a list of failures.
/// Lightweight alternative to throwing for predictable business-rule violations.
/// </summary>
/// <typeparam name="T">The success payload type.</typeparam>
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public IReadOnlyList<string> Errors { get; }

    private Result(bool isSuccess, T? value, IReadOnlyList<string> errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors;
    }

    public static Result<T> Success(T value) => new(true, value, Array.Empty<string>());

    public static Result<T> Failure(params string[] errors) =>
        new(false, default, errors.Length == 0 ? new[] { "Unknown error" } : errors);

    public static Result<T> Failure(IReadOnlyList<string> errors) =>
        new(false, default, errors.Count == 0 ? new[] { "Unknown error" } : errors);
}