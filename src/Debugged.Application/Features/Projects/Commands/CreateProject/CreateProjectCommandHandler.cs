using AutoMapper;
using Debugged.Application.DTOs.Projects;
using Debugged.Application.Interfaces;
using Debugged.Domain.Entities;
using MediatR;

namespace Debugged.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProjectDto> Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = new Project
        {
            Name = request.Name,
            Description = request.Description
        };

        await _projectRepository.AddAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProjectDto>(project);
    }
}