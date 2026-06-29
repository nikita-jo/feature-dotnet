namespace EmployeeManagementPortal.Tests.Fakes;

/// <summary>
/// Deterministic <see cref="TimeProvider"/> returning a fixed UTC instant,
/// so audit timestamp assertions stay stable across runs.
/// </summary>
internal sealed class FakeTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _now;

    public FakeTimeProvider(DateTimeOffset now)
    {
        _now = now;
    }

    public override DateTimeOffset GetUtcNow() => _now;
}
