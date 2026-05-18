using Debugged.Application.DTOs.Issues;
using MediatR;

namespace Debugged.Application.Features.Issues.Queries.GetSimilarIssues;

// Finds resolved issues similar to the given one — the app's core feature.
// "Similar" = shares tags AND/OR has a matching error message keyword.
// Caller passes an existing issue ID; the handler extracts tags + error message from it.
public record GetSimilarIssuesQuery(Guid IssueId) : IRequest<IReadOnlyList<SimilarIssueDto>>;