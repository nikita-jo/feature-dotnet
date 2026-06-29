namespace EmployeeManagementPortal.Domain.Common;

/// <summary>
/// Base class for all domain entities. Provides identity and audit metadata.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Timestamp when the entity was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the entity was last modified (UTC).
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}