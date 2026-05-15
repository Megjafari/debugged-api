using Debugged.Application.DTOs.Projects;
using MediatR;

namespace Debugged.Application.Features.Projects.Commands.CreateProject;

public record CreateProjectCommand(string Name, string? Description) : IRequest<ProjectDto>;