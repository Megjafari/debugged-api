using AutoMapper;
using Debugged.Application.Common.Exceptions;
using Debugged.Application.DTOs.Tags;
using Debugged.Application.Interfaces;
using Debugged.Domain.Entities;
using MediatR;

namespace Debugged.Application.Features.Tags.Commands.CreateTag;

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, TagDto>
{
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateTagCommandHandler(
        ITagRepository tagRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TagDto> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        // Same normalization rule as in CreateIssueCommandHandler — keeps the tag table consistent.
        var normalized = request.Name.Trim().ToLowerInvariant();

        // Check for duplicates before insert — the unique index on Name would catch it,
        // but a clean 400/409 is a much better DX than a wrapped DbUpdateException.
        var existing = await _tagRepository.GetByNameAsync(normalized, cancellationToken);
        if (existing is not null)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                [nameof(CreateTagCommand.Name)] = new[] { $"Tag '{normalized}' already exists." }
            });
        }

        var tag = new Tag { Name = normalized };

        await _tagRepository.AddAsync(tag, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TagDto>(tag);
    }
}