using AghanimsInventoryApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AghanimsInventoryApi.Data.Mappings;

public class HeroStatMap : IEntityTypeConfiguration<HeroStat>
{
    public void Configure(EntityTypeBuilder<HeroStat> builder)
    {
        builder.ToTable("HeroStats");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityColumn();

        builder.Property(x => x.Value)
            .IsRequired();

        builder.Property(x => x.StatId)
            .IsRequired();

        builder.Property(x => x.StatTypeId)
            .IsRequired();

        builder.Property(x => x.HeroId)
            .IsRequired();

        builder.HasIndex(x => new { x.StatId, x.StatTypeId, x.HeroId })
            .IsUnique();
    }
}