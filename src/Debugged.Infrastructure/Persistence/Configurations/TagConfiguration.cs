using Debugged.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Debugged.Infrastructure.Persistence.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        // Tag names must be unique. Enforced at the database level so
        // even concurrent inserts can't create duplicates.
        builder.HasIndex(t => t.Name)
            .IsUnique();
    }
}