using AghanimsInventoryApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AghanimsInventoryApi.Data.Mappings;

public class HeroAttributeMap : IEntityTypeConfiguration<HeroAttribute>
{
    public void Configure(EntityTypeBuilder<HeroAttribute> builder)
    {
        builder.ToTable("HeroAttributes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityColumn();

        builder.Property(x => x.Health)
            .IsRequired();

        builder.Property(x => x.Mana)
            .IsRequired();

        builder.Property(x => x.BaseHealthRegen)
            .IsRequired();

        builder.Property(x => x.BaseManaRegen)
            .IsRequired();

        builder.Property(x => x.Strength)
            .IsRequired();

        builder.Property(x => x.Agility)
            .IsRequired();

        builder.Property(x => x.Intelligence)
            .IsRequired();

        builder.Property(x => x.StrengthPerLevel)
            .IsRequired();

        builder.Property(x => x.AgilityPerLevel)
            .IsRequired();

        builder.Property(x => x.IntelligencePerLevel)
            .IsRequired();

        builder.Property(x => x.HeroId)
            .IsRequired();
    }
}