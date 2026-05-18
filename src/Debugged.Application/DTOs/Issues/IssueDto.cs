using Debugged.Domain.Enums;

namespace Debugged.Application.DTOs.Issues;

// Full representation of an Issue — used for detail views and after Create/Update.
// CreatedByUserId is kept as raw Guid since the Domain layer doesn't reference Identity.
public record IssueDto(
    Guid Id,
    string Title,
    string Description,
    string? ErrorMessage,
    string? Solution,
    IssueStatus Status,
    IssuePriority Priority,
    DateTime CreatedAt,
    DateTime? ResolvedAt,
    Guid ProjectId,
    Guid CreatedByUserId,
    IReadOnlyList<string> Tags
);