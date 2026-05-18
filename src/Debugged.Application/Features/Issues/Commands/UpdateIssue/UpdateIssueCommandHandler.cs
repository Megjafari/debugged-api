using AutoMapper;
using Debugged.Application.Common.Exceptions;
using Debugged.Application.DTOs.Issues;
using Debugged.Application.Interfaces;
using Debugged.Domain.Entities;
using Debugged.Domain.Enums;
using MediatR;

namespace Debugged.Application.Features.Issues.Commands.UpdateIssue;

public class UpdateIssueCommandHandler : IRequestHandler<UpdateIssueCommand, IssueDto>
{
    private readonly IIssueRepository _issueRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateIssueCommandHandler(
        IIssueRepository issueRepository,
        ITagRepository tagRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _issueRepository = issueRepository;
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IssueDto> Handle(UpdateIssueCommand request, CancellationToken cancellationToken)
    {
        // Load with tags — EF needs to track the join rows so we can diff old vs new tags.
        var issue = await _issueRepository.GetByIdWithTagsAsync(request.Id, cancellationToken);
        if (issue is null)
        {
            throw new NotFoundException(nameof(Issue), request.Id);
        }

        // Detect Resolved transition BEFORE mutating, so we know whether to stamp ResolvedAt.
        var becomingResolved = issue.Status != IssueStatus.Resolved
                               && request.Status == IssueStatus.Resolved;
        var leavingResolved = issue.Status == IssueStatus.Resolved
                              && request.Status != IssueStatus.Resolved;

        issue.Title = request.Title;
        issue.Description = request.Description;
        issue.ErrorMessage = request.ErrorMessage;
        issue.Solution = request.Solution;
        issue.Status = request.Status;
        issue.Priority = request.Priority;

        if (becomingResolved)
        {
            issue.ResolvedAt = DateTime.UtcNow;
        }
        else if (leavingResolved)
        {
            // Reopening an issue clears the resolution timestamp.
            issue.ResolvedAt = null;
        }

        // Same normalization as CreateIssue — keep the rules in one place mentally even if duplicated.
        var normalizedTagNames = request.Tags
            .Select(t => t.Trim().ToLowerInvariant())
            .Where(t => !string.IsNullOrEmpty(t))
            .Distinct()
            .ToList();

        var existingTags = await _tagRepository.GetByNamesAsync(normalizedTagNames, cancellationToken);
        var existingNames = existingTags.Select(t => t.Name).ToHashSet();

        var newTags = normalizedTagNames
            .Where(name => !existingNames.Contains(name))
            .Select(name => new Tag { Name = name })
            .ToList();

        if (newTags.Count > 0)
        {
            await _tagRepository.AddRangeAsync(newTags, cancellationToken);
        }

        // Replace the join collection wholesale — EF detects removed/added rows from the diff.
        // Simpler than computing add/remove sets manually, and the IssueTags collection is small.
        var allTags = existingTags.Concat(newTags).ToList();
        issue.IssueTags = allTags.Select(tag => new IssueTag { IssueId = issue.Id, Tag = tag }).ToList();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Re-fetch to ensure the returned DTO reflects the persisted state with fresh tag IDs.
        var updated = await _issueRepository.GetByIdWithTagsAsync(issue.Id, cancellationToken);
        return _mapper.Map<IssueDto>(updated!);
    }
}