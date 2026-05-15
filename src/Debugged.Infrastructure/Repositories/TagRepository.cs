using Debugged.Application.Interfaces;
using Debugged.Domain.Entities;
using Debugged.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Debugged.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _db;

    public TagRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Tags.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    // Case-insensitive match. Tag names are normalized in Application before save,
    // but defensive matching here protects against legacy or seeded data.
    public Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => _db.Tags.FirstOrDefaultAsync(
            t => t.Name.ToLower() == name.ToLower(),
            cancellationToken);

    public async Task<IReadOnlyList<Tag>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _db.Tags
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Tag tag, CancellationToken cancellationToken = default)
        => await _db.Tags.AddAsync(tag, cancellationToken);

    public void Remove(Tag tag)
        => _db.Tags.Remove(tag);
}