using Debugged.Domain.Entities;

namespace Debugged.Application.Interfaces;

public interface IProjectRepository
{
    // Returns the project or null if not found.
    Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Returns all projects.
    Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default);

    // Stages a new project for insertion. UnitOfWork commits on SaveChangesAsync.
    Task AddAsync(Project project, CancellationToken cancellationToken = default);

    // Marks a tracked project for deletion.
    void Remove(Project project);

    // Lightweight check — avoids loading the full entity.
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}