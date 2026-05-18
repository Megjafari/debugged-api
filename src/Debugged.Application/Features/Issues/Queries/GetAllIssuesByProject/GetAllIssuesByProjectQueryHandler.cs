using AutoMapper;
using Debugged.Application.Common.Exceptions;
using Debugged.Application.DTOs.Issues;
using Debugged.Application.Interfaces;
using Debugged.Domain.Entities;
using MediatR;

namespace Debugged.Application.Features.Issues.Queries.GetAllIssuesByProject;

public class GetAllIssuesByProjectQueryHandler
    : IRequestHandler<GetAllIssuesByProjectQuery, IReadOnlyList<IssueListDto>>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;

    public GetAllIssuesByProjectQueryHandler(
        IIssueRepository issueRepository,
        IProjectRepository projectRepository,
        IMapper mapper)
    {
        _issueRepository = issueRepository;
        _projectRepository = projectRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<IssueListDto>> Handle(
        GetAllIssuesByProjectQuery request,
        CancellationToken cancellationToken)
    {
        // 404 if project doesn't exist — clearer than returning an empty list which could mean "no issues yet".
        var projectExists = await _projectRepository.ExistsAsync(request.ProjectId, cancellationToken);
        if (!projectExists)
        {
            throw new NotFoundException(nameof(Project), request.ProjectId);
        }

        var issues = await _issueRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);
        return _mapper.Map<IReadOnlyList<IssueListDto>>(issues);
    }
}