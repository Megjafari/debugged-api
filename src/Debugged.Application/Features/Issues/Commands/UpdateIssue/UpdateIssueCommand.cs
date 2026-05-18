using Debugged.Application.DTOs.Issues;
using Debugged.Domain.Enums;
using MediatR;

namespace Debugged.Application.Features.Issues.Commands.UpdateIssue;

// Id comes from the route, the rest from the body. The controller wires them together.
// Status is updatable here so users can mark an issue Resolved (which sets ResolvedAt in the handler).
public record UpdateIssueCommand(
    Guid Id,
    string Title,
    string Description,
    string? ErrorMessage,
    string? Solution,
    IssueStatus Status,
    IssuePriority Priority,
    IReadOnlyList<string> Tags
) : IRequest<IssueDto>;