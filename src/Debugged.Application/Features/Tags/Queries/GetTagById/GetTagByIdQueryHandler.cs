using AutoMapper;
using Debugged.Application.Common.Exceptions;
using Debugged.Application.DTOs.Tags;
using Debugged.Application.Interfaces;
using Debugged.Domain.Entities;
using MediatR;

namespace Debugged.Application.Features.Tags.Queries.GetTagById;

public class GetTagByIdQueryHandler : IRequestHandler<GetTagByIdQuery, TagDto>
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;

    public GetTagByIdQueryHandler(ITagRepository tagRepository, IMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }

    public async Task<TagDto> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetByIdAsync(request.Id, cancellationToken);
        if (tag is null)
        {
            throw new NotFoundException(nameof(Tag), request.Id);
        }

        return _mapper.Map<TagDto>(tag);
    }
}