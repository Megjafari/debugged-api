namespace Debugged.Domain.Common;

// Shared base for all entities. Centralizes Id and CreatedAt so we
// don't repeat them on every entity, and makes it easy to add cross-cutting
// concerns later (audit fields, soft delete flags, etc.).
public abstract class BaseEntity
{
    // Guid over int — better for distributed systems and hides
    // record counts in public APIs (sequential ids leak how many
    // records exist, which is bad practice for public-facing APIs).
    public Guid Id { get; set; } = Guid.NewGuid();

    // UTC always. Local time creates bugs across time zones and DST changes.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}