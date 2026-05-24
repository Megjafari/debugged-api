using Debugged.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Debugged.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(bool Succeeded, Guid UserId, string[] Errors)> RegisterAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        // Identity uses UserName as the primary login identifier — set it equal to email
        // so users log in with email and we don't have to manage two separate fields.
        var user = new ApplicationUser
        {
            Email = email,
            UserName = email,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            // Bubble up Identity's error descriptions (e.g. "Email is already taken") to the handler.
            var errors = createResult.Errors.Select(e => e.Description).ToArray();
            return (false, Guid.Empty, errors);
        }

        // Assign the default User role. If the role doesn't exist yet (first run before seeding),
        // AddToRoleAsync silently fails — the seeder will ensure the role exists from the start.
        var roleResult = await _userManager.AddToRoleAsync(user, "User");
        if (!roleResult.Succeeded)
        {
            var errors = roleResult.Errors.Select(e => e.Description).ToArray();
            return (false, Guid.Empty, errors);
        }

        return (true, user.Id, Array.Empty<string>());
    }

    public async Task<(bool Succeeded, Guid UserId, string Email, IReadOnlyList<string> Roles, string? Error)> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            // Same response shape as bad password — caller turns this into a generic 400.
            return (false, Guid.Empty, string.Empty, Array.Empty<string>(), "Invalid credentials");
        }

        var passwordOk = await _userManager.CheckPasswordAsync(user, password);
        if (!passwordOk)
        {
            return (false, Guid.Empty, string.Empty, Array.Empty<string>(), "Invalid credentials");
        }

        // Roles required for the JWT — claims-based authorization reads these later.
        var roles = await _userManager.GetRolesAsync(user);

        return (true, user.Id, user.Email!, roles.ToList(), null);
    }
}