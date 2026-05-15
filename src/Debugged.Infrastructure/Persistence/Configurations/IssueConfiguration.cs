using Debugged.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Debugged.Infrastructure.Persistence.Configurations;

public class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(i => i.Description)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(i => i.ErrorMessage)
            .HasMaxLength(5000);

        builder.Property(i => i.Solution)
            .HasMaxLength(5000);

        // Stores enums as int (0,1,2,3) — default behavior, made explicit here.
        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(i => i.Priority)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.CreatedByUserId)
            .IsRequired();

        // Index on Status — the SimilarIssues query filters on Resolved,
        // so this index keeps that scan fast even with many issues.
        builder.HasIndex(i => i.Status);

        // Index on ProjectId — most queries fetch issues by project.
        builder.HasIndex(i => i.ProjectId);
    }
}