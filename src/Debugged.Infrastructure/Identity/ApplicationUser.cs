using Microsoft.AspNetCore.Identity;

namespace Debugged.Infrastructure.Identity;

// Custom Identity user — lives in Infrastructure, NOT Domain.
// Domain stays Identity-free (no framework dependencies). Issue.CreatedByUserId references
// this user's Id as a raw Guid, not a navigation property, to preserve that boundary.
//
// IdentityUser<Guid> means we use Guid primary keys instead of the default string,
// matching our existing BaseEntity convention.
public class ApplicationUser : IdentityUser<Guid>
{
    // Stamped on registration. Distinct from IdentityUser's internal timestamps —
    // this is our domain-level "when did this account join the system" field.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}