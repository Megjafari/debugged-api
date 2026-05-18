using Debugged.Domain.Entities;

namespace Debugged.Application.Interfaces;

public interface ITagRepository
{
    // Returns the tag or null if not found.
    Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Used for tag deduplication when issues are tagged by name.
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    // Bulk lookup — used by CreateIssue/UpdateIssue to fetch existing tags in one query
    // instead of N round-trips. Names are expected to be normalized (trimmed + lowercased).
    Task<IReadOnlyList<Tag>> GetByNamesAsync(
        IEnumerable<string> names,
        CancellationToken cancellationToken = default);

    // Returns all tags, sorted alphabetically.
    Task<IReadOnlyList<Tag>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Tag tag, CancellationToken cancellationToken = default);

    // Bulk insert — staged for save, committed via UnitOfWork.
    Task AddRangeAsync(IEnumerable<Tag> tags, CancellationToken cancellationToken = default);

    void Remove(Tag tag);
}