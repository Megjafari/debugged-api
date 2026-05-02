using Debugged.Domain.Common;

namespace Debugged.Domain.Entities;

// Tags categorize issues by technology, error type, or any label
// the user finds useful (e.g. "EF Core", "NullReferenceException",
// "CORS", "Migration"). Tags are the primary signal for the
// SimilarIssues feature — issues sharing tags are likely related.
public class Tag : BaseEntity
{
    // Tag names should be unique. We'll enforce that at the database
    // level via a unique index in the EF configuration (Infrastructure).
    public string Name { get; set; } = string.Empty;

    // Many-to-many to Issue via the IssueTag join entity. Same reasoning
    // as on Issue.IssueTags — explicit join entity makes the SimilarIssues
    // query simpler.
    public ICollection<IssueTag> IssueTags { get; set; } = new List<IssueTag>();
}