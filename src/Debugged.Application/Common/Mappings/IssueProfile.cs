using AutoMapper;
using Debugged.Application.DTOs.Issues;
using Debugged.Domain.Entities;

namespace Debugged.Application.Common.Mappings;

// AutoMapper profile for Issue → DTOs (one-way only, like ProjectProfile).
// Positional records require ForCtorParam to map computed values into constructor parameters.
public class IssueProfile : Profile
{
    public IssueProfile()
    {
        CreateMap<Issue, IssueDto>()
            .ForCtorParam(nameof(IssueDto.Tags),
                opt => opt.MapFrom(src => src.IssueTags.Select(it => it.Tag.Name).ToList()));

        CreateMap<Issue, IssueListDto>()
            .ForCtorParam(nameof(IssueListDto.Tags),
                opt => opt.MapFrom(src => src.IssueTags.Select(it => it.Tag.Name).ToList()));

        // SimilarIssueDto is built manually in GetSimilarIssuesQueryHandler — no profile needed.
    }
}