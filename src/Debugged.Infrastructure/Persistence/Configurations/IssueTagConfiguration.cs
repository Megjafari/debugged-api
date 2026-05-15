using Debugged.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Debugged.Infrastructure.Persistence.Configurations;

public class IssueTagConfiguration : IEntityTypeConfiguration<IssueTag>
{
    public void Configure(EntityTypeBuilder<IssueTag> builder)
    {
        // Composite primary key — same issue cannot have the same tag twice.
        builder.HasKey(it => new { it.IssueId, it.TagId });

        builder.HasOne(it => it.Issue)
            .WithMany(i => i.IssueTags)
            .HasForeignKey(it => it.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(it => it.Tag)
            .WithMany(t => t.IssueTags)
            .HasForeignKey(it => it.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index on TagId — speeds up "find issues with this tag" queries
        // (used by FindSimilarResolvedAsync to count shared tags).
        builder.HasIndex(it => it.TagId);
    }
}