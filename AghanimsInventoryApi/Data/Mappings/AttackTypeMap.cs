using AghanimsInventoryApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AghanimsInventoryApi.Data.Mappings;

public class AttackTypeMap : IEntityTypeConfiguration<AttackType>
{
    public void Configure(EntityTypeBuilder<AttackType> builder)
    {
        builder.ToTable("AttackTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityColumn();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(20);
    }
}