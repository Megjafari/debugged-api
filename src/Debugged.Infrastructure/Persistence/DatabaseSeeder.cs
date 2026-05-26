using Debugged.Domain.Entities;
using Debugged.Domain.Enums;
using Debugged.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Debugged.Infrastructure.Persistence;

// Runs at application startup — ensures roles, demo users and sample data exist.
// Idempotent: each section checks for existence before inserting, so re-runs are safe.
public static class DatabaseSeeder
{
    // Hard-coded demo credentials — fine for a local demo, replace before any real deployment.
    private const string AdminEmail = "admin@debugged.local";
    private const string AdminPassword = "Admin1234";
    private const string DemoUserEmail = "demo@debugged.local";
    private const string DemoUserPassword = "Demo1234";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var dbContext = services.GetRequiredService<ApplicationDbContext>();

        await SeedRolesAsync(roleManager);
        var adminId = await SeedAdminAsync(userManager);
        var demoUserId = await SeedDemoUserAsync(userManager);

        // Demo content uses the demo user as author so admin-only mutations stay separate from regular data.
        // Both returns are nullable; if neither user was created we skip demo data.
        var authorUserId = demoUserId ?? adminId;
        if (authorUserId.HasValue)
        {
            await SeedDemoDataAsync(dbContext, authorUserId.Value);
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        string[] roles = ["User", "Admin"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }

    private static async Task<Guid?> SeedAdminAsync(UserManager<ApplicationUser> userManager)
    {
        var existing = await userManager.FindByEmailAsync(AdminEmail);
        if (existing is not null)
        {
            return existing.Id;
        }

        var admin = new ApplicationUser
        {
            Email = AdminEmail,
            UserName = AdminEmail,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(admin, AdminPassword);
        if (!result.Succeeded)
        {
            return null;
        }

        await userManager.AddToRoleAsync(admin, "Admin");
        return admin.Id;
    }

    private static async Task<Guid?> SeedDemoUserAsync(UserManager<ApplicationUser> userManager)
    {
        var existing = await userManager.FindByEmailAsync(DemoUserEmail);
        if (existing is not null)
        {
            return existing.Id;
        }

        var demo = new ApplicationUser
        {
            Email = DemoUserEmail,
            UserName = DemoUserEmail,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(demo, DemoUserPassword);
        if (!result.Succeeded)
        {
            return null;
        }

        await userManager.AddToRoleAsync(demo, "User");
        return demo.Id;
    }

    private static async Task SeedDemoDataAsync(ApplicationDbContext db, Guid authorUserId)
    {
        // Only seed if there are no projects yet — preserves manually created data across restarts.
        if (await db.Projects.AnyAsync())
        {
            return;
        }

        // Tags first — issues reference them via the join table.
        var tagNginx = new Tag { Name = "nginx" };
        var tagWebsocket = new Tag { Name = "websocket" };
        var tagTimeout = new Tag { Name = "timeout" };
        var tagDocker = new Tag { Name = "docker" };
        var tagPostgres = new Tag { Name = "postgres" };
        var tagEfcore = new Tag { Name = "efcore" };

        db.Tags.AddRange(tagNginx, tagWebsocket, tagTimeout, tagDocker, tagPostgres, tagEfcore);

        var project = new Project
        {
            Name = "Debugged Demo Project",
            Description = "Sample project showing the bug knowledge base in action."
        };

        db.Projects.Add(project);

        // Resolved issue — has a real solution. This is what /similar matches will surface.
        var resolvedIssue = new Issue
        {
            Title = "Websocket disconnect through nginx reverse proxy",
            Description = "Long-lived websocket connections drop after exactly 60 seconds when going through the nginx proxy.",
            ErrorMessage = "upstream prematurely closed connection while reading response header from upstream",
            Solution = "Add proxy_read_timeout 86400s and proxy_send_timeout 86400s to the nginx location block. " +
                       "Also set proxy_http_version 1.1 and the Upgrade/Connection headers for websocket support.",
            Status = IssueStatus.Resolved,
            Priority = IssuePriority.High,
            ResolvedAt = DateTime.UtcNow.AddDays(-3),
            ProjectId = project.Id,
            CreatedByUserId = authorUserId,
            IssueTags = new List<IssueTag>
            {
                new() { Tag = tagNginx },
                new() { Tag = tagWebsocket },
                new() { Tag = tagTimeout }
            }
        };

        // Open issue with overlapping tags + similar error message — exists so /similar
        // has a clear match to demonstrate the core feature.
        var openIssue = new Issue
        {
            Title = "Realtime updates dropping in production",
            Description = "Users report that the live dashboard freezes after a minute or so when the app sits behind our edge proxy.",
            ErrorMessage = "upstream prematurely closed connection",
            Solution = null,
            Status = IssueStatus.Open,
            Priority = IssuePriority.High,
            ProjectId = project.Id,
            CreatedByUserId = authorUserId,
            IssueTags = new List<IssueTag>
            {
                new() { Tag = tagNginx },
                new() { Tag = tagWebsocket }
            }
        };

        // Unrelated open issue — shows that /similar correctly ignores non-matching content.
        var unrelatedIssue = new Issue
        {
            Title = "EF Core migration fails on Postgres 17",
            Description = "Initial migration crashes with a syntax error when targeting Postgres 17.",
            ErrorMessage = "syntax error at or near \"GENERATED\"",
            Solution = null,
            Status = IssueStatus.Open,
            Priority = IssuePriority.Medium,
            ProjectId = project.Id,
            CreatedByUserId = authorUserId,
            IssueTags = new List<IssueTag>
            {
                new() { Tag = tagPostgres },
                new() { Tag = tagEfcore },
                new() { Tag = tagDocker }
            }
        };

        db.Issues.AddRange(resolvedIssue, openIssue, unrelatedIssue);

        await db.SaveChangesAsync();
    }
}