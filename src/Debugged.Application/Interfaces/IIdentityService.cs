using Debugged.Application.DTOs.Auth;

namespace Debugged.Application.Interfaces;

// Abstraction over ASP.NET Identity — keeps Application free of Infrastructure dependencies.
// Returns Result-like tuples so handlers can react without catching exceptions.
public interface IIdentityService
{
    // Creates a new user with the given email/password and the default "User" role.
    // Returns the new user's id, or a list of error messages if creation failed.
    Task<(bool Succeeded, Guid UserId, string[] Errors)> RegisterAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    // Validates credentials. Returns the user's id, email and roles for token issuance.
    // Single combined result lets us issue tokens without a second lookup.
    Task<(bool Succeeded, Guid UserId, string Email, IReadOnlyList<string> Roles, string? Error)> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
}