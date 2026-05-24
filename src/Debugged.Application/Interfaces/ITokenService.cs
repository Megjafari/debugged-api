using Debugged.Application.DTOs.Auth;

namespace Debugged.Application.Interfaces;

// Issues JWT tokens for authenticated users. The concrete implementation lives in Infrastructure
// (it depends on JwtBearer + Identity), but handlers depend on this abstraction.
public interface ITokenService
{
    // Builds a signed JWT containing the user's id, email and roles as claims.
    // Returns the token string and the absolute expiry time so the API response can carry both.
    AuthTokenResult CreateToken(Guid userId, string email, IEnumerable<string> roles);
}