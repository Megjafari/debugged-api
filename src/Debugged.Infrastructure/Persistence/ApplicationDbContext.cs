using System.Reflection;
using Debugged.Domain.Entities;
using Debugged.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Debugged.Infrastructure.Persistence;

// Now inherits IdentityDbContext so Identity's tables (AspNetUsers, AspNetRoles, etc.)
// share the same DbContext and database as our domain entities.
// Type args: <TUser, TRole, TKey> — we use Guid keys to stay consistent with BaseEntity.
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Issue> Issues => Set<Issue>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<IssueTag> IssueTags => Set<IssueTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // base.OnModelCreating MUST run FIRST when inheriting IdentityDbContext —
        // it sets up the Identity tables. Skipping it breaks user/role creation.
        base.OnModelCreating(modelBuilder);

        // Picks up all IEntityTypeConfiguration<T> classes in this assembly,
        // so each entity's mapping lives in its own file instead of here.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}