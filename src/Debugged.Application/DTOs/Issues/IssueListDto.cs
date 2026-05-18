using Debugged.Domain.Enums;

namespace Debugged.Application.DTOs.Issues;

// Lightweight DTO for list endpoints — omits long fields (Description, Solution, ErrorMessage)
// to keep payloads small. Clients fetch full details via GetById when needed.
public record IssueListDto(
    Guid Id,
    string Title,
    IssueStatus Status,
    IssuePriority Priority,
    DateTime CreatedAt,
    DateTime? ResolvedAt,
    Guid ProjectId,
    IReadOnlyList<string> Tags
);