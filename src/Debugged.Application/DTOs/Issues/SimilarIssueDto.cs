using Debugged.Domain.Enums;

namespace Debugged.Application.DTOs.Issues;

// Returned by GetSimilarIssuesQuery — includes the Solution since that's the whole point
// of finding similar issues (learn from how they were fixed). MatchScore lets the UI rank results.
public record SimilarIssueDto(
    Guid Id,
    string Title,
    string? ErrorMessage,
    string? Solution,
    IssueStatus Status,
    DateTime? ResolvedAt,
    Guid ProjectId,
    IReadOnlyList<string> Tags,
    int MatchScore
);