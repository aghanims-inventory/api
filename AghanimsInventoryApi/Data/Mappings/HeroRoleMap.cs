using AghanimsInventoryApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AghanimsInventoryApi.Data.Mappings;

public class HeroRoleMap : IEntityTypeConfiguration<HeroRole>
{
    public void Configure(EntityTypeBuilder<HeroRole> builder)
    {
        builder.ToTable("HeroRoles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityColumn();

        builder.Property(x => x.Intensity)
            .IsRequired();

        builder.Property(x => x.HeroId)
            .IsRequired();

        builder.Property(x => x.RoleId)
            .IsRequired();

        builder.HasIndex(x => new { x.HeroId, x.RoleId })
            .IsUnique();
    }
}