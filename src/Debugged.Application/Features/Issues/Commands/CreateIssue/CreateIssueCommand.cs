using Debugged.Application.DTOs.Issues;
using Debugged.Domain.Enums;
using MediatR;

namespace Debugged.Application.Features.Issues.Commands.CreateIssue;

// Tags arrive as a list of names — the handler resolves them to entities,
// creating new Tag rows for any that don't exist yet.
public record CreateIssueCommand(
    string Title,
    string Description,
    string? ErrorMessage,
    string? Solution,
    IssuePriority Priority,
    Guid ProjectId,
    IReadOnlyList<string> Tags
) : IRequest<IssueDto>;