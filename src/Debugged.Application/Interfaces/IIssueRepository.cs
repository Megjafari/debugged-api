using Debugged.Domain.Entities;

namespace Debugged.Application.Interfaces;

public interface IIssueRepository
{
    Task<Issue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Eager-loads tags. Use when displaying full issue detail or mutating tags.
    Task<Issue?> GetByIdWithTagsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Issue>> GetByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    // Core feature: returns RESOLVED issues ranked by relevance, paired with their match score.
    // Scoring = number of shared tags + 1 if the error message keyword matches.
    // excludeIssueId prevents an existing issue from matching itself.
    Task<IReadOnlyList<SimilarIssueResult>> FindSimilarResolvedAsync(
        IEnumerable<Guid> tagIds,
        string? errorMessageKeyword,
        Guid? excludeIssueId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(Issue issue, CancellationToken cancellationToken = default);
    void Remove(Issue issue);
}