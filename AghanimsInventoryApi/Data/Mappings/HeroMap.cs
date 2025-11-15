using AghanimsInventoryApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AghanimsInventoryApi.Data.Mappings;

public class HeroMap : IEntityTypeConfiguration<Hero>
{
    public void Configure(EntityTypeBuilder<Hero> builder)
    {
        builder.ToTable("Heroes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityColumn();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Complexity)
            .IsRequired();

        builder.Property(x => x.ImageUrl)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(x => x.AttributeId)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}