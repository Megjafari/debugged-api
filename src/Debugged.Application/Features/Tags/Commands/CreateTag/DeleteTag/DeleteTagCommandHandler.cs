using Debugged.Application.Common.Exceptions;
using Debugged.Application.Interfaces;
using Debugged.Domain.Entities;
using MediatR;

namespace Debugged.Application.Features.Tags.Commands.DeleteTag;

public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand>
{
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTagCommandHandler(ITagRepository tagRepository, IUnitOfWork unitOfWork)
    {
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetByIdAsync(request.Id, cancellationToken);
        if (tag is null)
        {
            throw new NotFoundException(nameof(Tag), request.Id);
        }

        _tagRepository.Remove(tag);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}