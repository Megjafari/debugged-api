using Debugged.Domain.Enums;

namespace Debugged.API.Contracts.Issues;

// Request shapes for the controller — separate from MediatR commands so the body
// doesn't carry route parameters (projectId/id come from the URL, not the JSON).
// This avoids the foot-gun of a client putting one projectId in the URL and another in the body.

public record CreateIssueRequest(
    string Title,
    string Description,
    string? ErrorMessage,
    string? Solution,
    IssuePriority Priority,
    IReadOnlyList<string> Tags
);

public record UpdateIssueRequest(
    string Title,
    string Description,
    string? ErrorMessage,
    string? Solution,
    IssueStatus Status,
    IssuePriority Priority,
    IReadOnlyList<string> Tags
);