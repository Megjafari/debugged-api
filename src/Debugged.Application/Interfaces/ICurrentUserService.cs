namespace Debugged.Application.Interfaces;

// Exposes the authenticated user's identity to Application-layer handlers.
// Concrete implementation reads JWT claims via IHttpContextAccessor (in API/Infrastructure).
// This abstraction lets handlers stamp ownership (CreatedByUserId) without referencing HttpContext directly.
public interface ICurrentUserService
{
    // Returns the user id from the JWT, or null if the request is unauthenticated.
    // Nullable because some flows (Auth endpoints, future seeders) run without a user context.
    Guid? UserId { get; }

    // Convenience flag — handlers can short-circuit when they require an authenticated user.
    bool IsAuthenticated { get; }
}