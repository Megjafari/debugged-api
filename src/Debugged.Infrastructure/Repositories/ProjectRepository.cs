using Debugged.Application.Interfaces;
using Debugged.Domain.Entities;
using Debugged.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Debugged.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _db;

    public ProjectRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _db.Projects
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Project project, CancellationToken cancellationToken = default)
        => await _db.Projects.AddAsync(project, cancellationToken);

    public void Remove(Project project)
        => _db.Projects.Remove(project);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => _db.Projects.AnyAsync(p => p.Id == id, cancellationToken);
}