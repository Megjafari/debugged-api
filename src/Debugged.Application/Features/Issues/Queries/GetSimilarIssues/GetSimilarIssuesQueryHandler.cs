using Debugged.Application.Common.Exceptions;
using Debugged.Application.DTOs.Issues;
using Debugged.Application.Interfaces;
using Debugged.Domain.Entities;
using MediatR;

namespace Debugged.Application.Features.Issues.Queries.GetSimilarIssues;

public class GetSimilarIssuesQueryHandler
    : IRequestHandler<GetSimilarIssuesQuery, IReadOnlyList<SimilarIssueDto>>
{
    private readonly IIssueRepository _issueRepository;

    public GetSimilarIssuesQueryHandler(IIssueRepository issueRepository)
    {
        _issueRepository = issueRepository;
    }

    public async Task<IReadOnlyList<SimilarIssueDto>> Handle(
        GetSimilarIssuesQuery request,
        CancellationToken cancellationToken)
    {
        // Load the source issue with its tags — these are the matching signals.
        var source = await _issueRepository.GetByIdWithTagsAsync(request.IssueId, cancellationToken);
        if (source is null)
        {
            throw new NotFoundException(nameof(Issue), request.IssueId);
        }

        var tagIds = source.IssueTags.Select(it => it.TagId).ToList();

        // Exclude the source issue itself — it would always be the top match against itself.
        var results = await _issueRepository.FindSimilarResolvedAsync(
            tagIds,
            source.ErrorMessage,
            excludeIssueId: source.Id,
            cancellationToken);

        // Manual projection to DTO — SimilarIssueDto includes MatchScore which isn't on the entity.
        // AutoMapper can't help here since the score lives on the repository result wrapper.
        return results
            .Select(r => new SimilarIssueDto(
                r.Issue.Id,
                r.Issue.Title,
                r.Issue.ErrorMessage,
                r.Issue.Solution,
                r.Issue.Status,
                r.Issue.ResolvedAt,
                r.Issue.ProjectId,
                r.Issue.IssueTags.Select(it => it.Tag.Name).ToList(),
                r.MatchScore))
            .ToList();
    }
}