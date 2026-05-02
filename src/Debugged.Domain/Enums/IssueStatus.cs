namespace Debugged.Domain.Enums;

// Lifecycle states for an Issue. The flow is typically:
// Open -> InProgress -> Resolved -> (optionally) Archived
public enum IssueStatus
{
    // Explicit numeric values prevent the enum from "shifting" if
    // someone reorders the members later. Migrations and stored data
    // would otherwise become misaligned.
    Open = 0,
    InProgress = 1,
    Resolved = 2,
    Archived = 3
}