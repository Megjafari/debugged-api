namespace Debugged.Application.DTOs.Auth;

// Returned by both /register and /login — gives the client everything it needs to authenticate
// subsequent requests: the bearer token, when it expires, and basic user identity.
public record AuthResponseDto(
    string Token,
    DateTime ExpiresAt,
    Guid UserId,
    string Email,
    IReadOnlyList<string> Roles
);