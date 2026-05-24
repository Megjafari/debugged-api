using AutoMapper;
using Debugged.Application.DTOs.Tags;
using Debugged.Application.Interfaces;
using MediatR;

namespace Debugged.Application.Features.Tags.Queries.GetAllTags;

public class GetAllTagsQueryHandler : IRequestHandler<GetAllTagsQuery, IReadOnlyList<TagDto>>
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;

    public GetAllTagsQueryHandler(ITagRepository tagRepository, IMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TagDto>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<TagDto>>(tags);
    }
}