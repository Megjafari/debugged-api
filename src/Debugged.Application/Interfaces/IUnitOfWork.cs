namespace Debugged.Application.Interfaces;

public interface IUnitOfWork
{
    // Commits all staged changes (from any repository) in a single transaction.
    // Returns the number of affected rows.
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}