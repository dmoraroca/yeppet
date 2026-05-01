using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zuppeto.Infrastructure.Persistence.Entities;

namespace Zuppeto.Infrastructure.Persistence.Configurations;

public sealed class TagConfiguration : IEntityTypeConfiguration<TagRecord>
{
    public void Configure(EntityTypeBuilder<TagRecord> builder)
    {
        builder.ToTable("tags");

        builder.HasKey(tag => tag.Id);

        builder.Property(tag => tag.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(tag => tag.Code)
            .HasColumnName("code")
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(tag => tag.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(tag => tag.Code)
            .IsUnique()
            .HasDatabaseName("uq_tags_code");
    }
}
