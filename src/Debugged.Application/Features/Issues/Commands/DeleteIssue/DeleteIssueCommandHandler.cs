using Debugged.Application.Common.Exceptions;
using Debugged.Application.Interfaces;
using Debugged.Domain.Entities;
using Debugged.Domain.Enums;
using MediatR;

namespace Debugged.Application.Features.Issues.Commands.DeleteIssue;

public class DeleteIssueCommandHandler : IRequestHandler<DeleteIssueCommand>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteIssueCommandHandler(
        IIssueRepository issueRepository,
        IUnitOfWork unitOfWork)
    {
        _issueRepository = issueRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteIssueCommand request, CancellationToken cancellationToken)
    {
        var issue = await _issueRepository.GetByIdAsync(request.Id, cancellationToken);
        if (issue is null)
        {
            throw new NotFoundException(nameof(Issue), request.Id);
        }

        // Soft delete — issue stays in DB but is hidden from active workflows.
        // Keeps the knowledge base intact: archived bugs (and their solutions) remain searchable.
        // Idempotent: deleting an already-archived issue is a no-op, not an error.
        if (issue.Status == IssueStatus.Archived)
        {
            return;
        }

        issue.Status = IssueStatus.Archived;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}