using Debugged.Application.DTOs.Projects;
using MediatR;

namespace Debugged.Application.Features.Projects.Queries.GetAllProjects;

public record GetAllProjectsQuery() : IRequest<IReadOnlyList<ProjectDto>>;