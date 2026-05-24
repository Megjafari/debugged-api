using AutoMapper;
using Debugged.Application.DTOs.Tags;
using Debugged.Domain.Entities;

namespace Debugged.Application.Common.Mappings;

// One-way mapping Tag → TagDto. Tag entities are never created from DTOs;
// creation goes through CreateTagCommand which handles normalization.
public class TagProfile : Profile
{
    public TagProfile()
    {
        CreateMap<Tag, TagDto>();
    }
}