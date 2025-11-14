using AghanimsInventoryApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AghanimsInventoryApi.Data.Mappings;

public class StatMap : IEntityTypeConfiguration<Stat>
{
    public void Configure(EntityTypeBuilder<Stat> builder)
    {
        builder.ToTable("Stats");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityColumn();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(x => x.IsPercentage)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.ImageUrl)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(x => x.StatTypeId)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}