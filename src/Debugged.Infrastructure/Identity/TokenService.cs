using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Debugged.Application.DTOs.Auth;
using Debugged.Application.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Debugged.Infrastructure.Identity;

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;

    // IOptions<T> resolves the strongly-typed JwtSettings from configuration.
    public TokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public AuthTokenResult CreateToken(Guid userId, string email, IEnumerable<string> roles)
    {
        // Symmetric key derived from the configured secret. The secret MUST be ≥32 chars for HS256.
        var keyBytes = Encoding.UTF8.GetBytes(_settings.Key);
        var signingKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        // Standard JWT claims plus our own. Sub = subject (the user), Jti = unique token id (replay protection).
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            // NameIdentifier is the claim ASP.NET Core reads to populate User.Identity.Name and
            // ClaimsPrincipal lookups. Including it makes downstream [Authorize] checks straightforward.
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };

        // One claim per role — [Authorize(Roles = "Admin")] inspects these.
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthTokenResult(encodedToken, expiresAt);
    }
}