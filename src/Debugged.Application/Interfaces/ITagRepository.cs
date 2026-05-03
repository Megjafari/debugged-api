using Debugged.Domain.Entities;

namespace Debugged.Application.Interfaces;

public interface ITagRepository
{
    // Returns the tag or null if not found.
    Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Used for tag deduplication when issues are tagged by name.
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    // Returns all tags, sorted alphabetically.
    Task<IReadOnlyList<Tag>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Tag tag, CancellationToken cancellationToken = default);
    void Remove(Tag tag);
}