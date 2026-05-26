using AutoMapper;
using Debugged.Application.Common.Exceptions;
using Debugged.Application.DTOs.Issues;
using Debugged.Application.Interfaces;
using Debugged.Domain.Entities;
using Debugged.Domain.Enums;
using MediatR;

namespace Debugged.Application.Features.Issues.Commands.CreateIssue;

public class CreateIssueCommandHandler : IRequestHandler<CreateIssueCommand, IssueDto>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public CreateIssueCommandHandler(
        IIssueRepository issueRepository,
        IProjectRepository projectRepository,
        ITagRepository tagRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUser)
    {
        _issueRepository = issueRepository;
        _projectRepository = projectRepository;
        _tagRepository = tagRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<IssueDto> Handle(CreateIssueCommand request, CancellationToken cancellationToken)
    {
        // Verify the parent project exists — otherwise we'd hit a FK violation deeper down with a useless error.
        var projectExists = await _projectRepository.ExistsAsync(request.ProjectId, cancellationToken);
        if (!projectExists)
        {
            throw new NotFoundException(nameof(Project), request.ProjectId);
        }

        // Normalize tag names: trim + lowercase to make dedup case-insensitive.
        // "Frontend", "frontend ", " FRONTEND" all collapse to "frontend".
        var normalizedTagNames = request.Tags
            .Select(t => t.Trim().ToLowerInvariant())
            .Where(t => !string.IsNullOrEmpty(t))
            .Distinct()
            .ToList();

        // Find existing tags in one query, create the missing ones — avoids N+1 lookups.
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

        var allTags = existingTags.Concat(newTags).ToList();

        var issue = new Issue
        {
            Title = request.Title,
            Description = request.Description,
            ErrorMessage = request.ErrorMessage,
            Solution = request.Solution,
            Priority = request.Priority,
            Status = IssueStatus.Open, // New issues always start as Open — clients can't dictate this.
            ProjectId = request.ProjectId,
            // Stamped from the JWT — handler is invoked behind [Authorize], so a userId is always present.
            // The null-forgiving '!' is safe here: an unauthenticated request would have been rejected by the pipeline.
            CreatedByUserId = _currentUser.UserId!.Value,
            IssueTags = allTags.Select(tag => new IssueTag { Tag = tag }).ToList()
        };

        await _issueRepository.AddAsync(issue, cancellationToken);

        // Single SaveChanges — both new tags and the issue (with its join rows) commit atomically.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Use the with-tags variant so AutoMapper can flatten IssueTags.Tag.Name into the DTO.
        var created = await _issueRepository.GetByIdWithTagsAsync(issue.Id, cancellationToken);
        
        return _mapper.Map<IssueDto>(created!);
    }
}