using AghanimsInventoryApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AghanimsInventoryApi.Data.Mappings;

public class RarityMap : IEntityTypeConfiguration<Rarity>
{
    public void Configure(EntityTypeBuilder<Rarity> builder)
    {
        builder.ToTable("Rarities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityColumn();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Color)
            .IsRequired()
            .HasMaxLength(6);
    }
}