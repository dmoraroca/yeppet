using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zuppeto.Infrastructure.Persistence.Entities;

namespace Zuppeto.Infrastructure.Persistence.Configurations;

public sealed class FeatureConfiguration : IEntityTypeConfiguration<FeatureRecord>
{
    public void Configure(EntityTypeBuilder<FeatureRecord> builder)
    {
        builder.ToTable("features");

        builder.HasKey(feature => feature.Id);

        builder.Property(feature => feature.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(feature => feature.Code)
            .HasColumnName("code")
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(feature => feature.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(feature => feature.Code)
            .IsUnique()
            .HasDatabaseName("uq_features_code");
    }
}
