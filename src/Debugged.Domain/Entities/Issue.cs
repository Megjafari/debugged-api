using Debugged.Domain.Common;
using Debugged.Domain.Enums;

namespace Debugged.Domain.Entities;

// The core entity of the system. An Issue represents a bug or problem,
// optionally with its solution. Once resolved, it becomes a piece of
// archived knowledge that the "Similar Issues" feature can match against.
public class Issue : BaseEntity
{
    // Required core fields
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Optional — a bug might not have a stack trace, and an unresolved
    // issue won't have a solution yet.
    public string? ErrorMessage { get; set; }
    public string? Solution { get; set; }

    // Default to Open — a brand-new issue starts in the Open state
    // and moves through the lifecycle as work progresses.
    public IssueStatus Status { get; set; } = IssueStatus.Open;

    // Default to Medium — most issues are not Low or Critical, so this
    // is the most defensive middle-ground default.
    public IssuePriority Priority { get; set; } = IssuePriority.Medium;

    // Nullable — only set when the issue is actually resolved.
    // Setting both Status = Resolved and ResolvedAt = UtcNow happens
    // together, ideally via a domain method (we'll wire that up in handlers).
    public DateTime? ResolvedAt { get; set; }

    // Foreign key to Project — every issue belongs to exactly one project.
    // EF Core convention: a property named "<EntityName>Id" is automatically
    // recognized as the FK for a navigation property of type <EntityName>.
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    // Many-to-many relation to Tag, via the IssueTag join entity.
    // We use a join entity (rather than EF's implicit join table) because
    // the GetSimilarIssues query needs to query the join directly to count
    // shared tags between issues.
    public ICollection<IssueTag> IssueTags { get; set; } = new List<IssueTag>();

    // Tracks who created the issue. Stored as a plain Guid rather than a
    // navigation property to ApplicationUser — Domain must not depend on
    // ASP.NET Identity. The API enforces ownership via ICurrentUserService.
    public Guid CreatedByUserId { get; set; }
}