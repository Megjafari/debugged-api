using Debugged.Application.DTOs.Issues;
using MediatR;

namespace Debugged.Application.Features.Issues.Queries.GetIssueById;

public record GetIssueByIdQuery(Guid Id) : IRequest<IssueDto>;