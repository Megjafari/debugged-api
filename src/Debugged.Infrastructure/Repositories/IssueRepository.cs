using Debugged.Application.Interfaces;
using Debugged.Domain.Entities;
using Debugged.Domain.Enums;
using Debugged.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Debugged.Infrastructure.Repositories;

public class IssueRepository : IIssueRepository
{
    private readonly ApplicationDbContext _db;

    public IssueRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<Issue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Issues.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public Task<Issue?> GetByIdWithTagsAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Issues
            .Include(i => i.IssueTags)
                .ThenInclude(it => it.Tag)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Issue>> GetByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
        => await _db.Issues
            .AsNoTracking()
            .Include(i => i.IssueTags)
                .ThenInclude(it => it.Tag)
            .Where(i => i.ProjectId == projectId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);

    // Core feature: returns RESOLVED issues ranked by relevance.
    // Scoring = number of shared tags + 1 if the error message keyword matches.
    // The query runs entirely in SQL — no in-memory filtering.
    public async Task<IReadOnlyList<SimilarIssueResult>> FindSimilarResolvedAsync(
    IEnumerable<Guid> tagIds,
    string? errorMessageKeyword,
    Guid? excludeIssueId = null,
    CancellationToken cancellationToken = default)
    {
    var tagIdList = tagIds.ToList();
    var hasKeyword = !string.IsNullOrWhiteSpace(errorMessageKeyword);

    // No signals to match on — nothing similar.
    if (tagIdList.Count == 0 && !hasKeyword)
        return Array.Empty<SimilarIssueResult>();

    var query = _db.Issues
        .AsNoTracking()
        .Include(i => i.IssueTags)
            .ThenInclude(it => it.Tag)
        .Where(i => i.Status == IssueStatus.Resolved);

    if (excludeIssueId.HasValue)
        query = query.Where(i => i.Id != excludeIssueId.Value);

    // Build the score in the SQL projection so the database does the work.
    var scored = query.Select(i => new
    {
        Issue = i,
        TagMatches = i.IssueTags.Count(it => tagIdList.Contains(it.TagId)),
        ErrorMatches = hasKeyword && i.ErrorMessage != null
                       && EF.Functions.ILike(i.ErrorMessage, $"%{errorMessageKeyword}%")
            ? 1 : 0
    })
    .Where(x => x.TagMatches > 0 || x.ErrorMatches > 0)
    .OrderByDescending(x => x.TagMatches + x.ErrorMatches)
    .ThenByDescending(x => x.Issue.ResolvedAt)
    .Take(10);

    var results = await scored.ToListAsync(cancellationToken);

    // Materialize to the application-facing result type, preserving the SQL-computed score.
    return results
        .Select(x => new SimilarIssueResult(x.Issue, x.TagMatches + x.ErrorMatches))
        .ToList();
    }

    public async Task AddAsync(Issue issue, CancellationToken cancellationToken = default)
        => await _db.Issues.AddAsync(issue, cancellationToken);

    public void Remove(Issue issue)
        => _db.Issues.Remove(issue);
}