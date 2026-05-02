namespace Debugged.Domain.Entities;

// Join entity for the many-to-many relationship between Issue and Tag.
// EF Core 7+ can handle many-to-many implicitly without a join entity,
// but we use one explicitly for two reasons:
//   1. The SimilarIssues query needs to query the join directly to count
//      shared tags between issues efficiently.
//   2. It leaves room for future metadata on the relationship itself
//      (e.g. when the tag was applied, by whom).
public class IssueTag
{
    // Composite primary key (IssueId, TagId) — configured in
    // IssueTagConfiguration in the Infrastructure layer.
    // No BaseEntity inheritance: a join entity has no independent identity.
    public Guid IssueId { get; set; }
    public Issue Issue { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}