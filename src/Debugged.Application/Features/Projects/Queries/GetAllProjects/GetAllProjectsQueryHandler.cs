using AutoMapper;
using Debugged.Application.DTOs.Projects;
using Debugged.Application.Interfaces;
using MediatR;

namespace Debugged.Application.Features.Projects.Queries.GetAllProjects;

public class GetAllProjectsQueryHandler
    : IRequestHandler<GetAllProjectsQuery, IReadOnlyList<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;

    public GetAllProjectsQueryHandler(
        IProjectRepository projectRepository,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ProjectDto>> Handle(
        GetAllProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ProjectDto>>(projects);
    }
}