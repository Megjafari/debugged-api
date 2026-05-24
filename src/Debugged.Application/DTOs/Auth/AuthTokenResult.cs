namespace Debugged.Application.DTOs.Auth;

// Returned from ITokenService. ExpiresAt is included so clients can pre-emptively refresh
// instead of waiting for a 401 from a protected endpoint.
public record AuthTokenResult(string Token, DateTime ExpiresAt);