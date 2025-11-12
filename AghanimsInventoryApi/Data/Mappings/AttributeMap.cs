using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AghanimsInventoryApi.Data.Mappings;

public class AttributeMap : IEntityTypeConfiguration<Entities.Attribute>
{
    public void Configure(EntityTypeBuilder<Entities.Attribute> builder)
    {
        builder.ToTable("Attributes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityColumn();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(30);
    }
}
