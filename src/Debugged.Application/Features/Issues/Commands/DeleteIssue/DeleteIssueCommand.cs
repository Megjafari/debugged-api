using MediatR;

namespace Debugged.Application.Features.Issues.Commands.DeleteIssue;

// Soft delete — sets Status = Archived. Returns Unit since the API responds with 204 No Content.
public record DeleteIssueCommand(Guid Id) : IRequest;