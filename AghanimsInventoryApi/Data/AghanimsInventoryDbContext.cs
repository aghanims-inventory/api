using AghanimsInventoryApi.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AghanimsInventoryApi.Data;

public class AghanimsInventoryDbContext : DbContext
{
    public AghanimsInventoryDbContext(DbContextOptions<AghanimsInventoryDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AttributeMap());

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Entities.Attribute> Attributes { get; set; }
}