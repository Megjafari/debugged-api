using AutoMapper;
using Debugged.Application.DTOs.Projects;
using Debugged.Domain.Entities;

namespace Debugged.Application.Common.Mappings;

public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        CreateMap<Project, ProjectDto>();
    }
}