namespace Debugged.Domain.Enums;

// Severity levels for an Issue. Used for filtering/sorting and
// could later drive notifications (e.g. notify on Critical).
public enum IssuePriority
{
    // Same explicit-values rule as IssueStatus — protects stored data
    // if the enum is reordered or extended later.
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}