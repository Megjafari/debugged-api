using Debugged.Application.DTOs.Issues;
using MediatR;

namespace Debugged.Application.Features.Issues.Queries.GetAllIssuesByProject;

// Issues are always scoped under a project — listing "all issues globally" doesn't
// make sense for this domain. The route will be GET /api/projects/{projectId}/issues.
public record GetAllIssuesByProjectQuery(Guid ProjectId) : IRequest<IReadOnlyList<IssueListDto>>;