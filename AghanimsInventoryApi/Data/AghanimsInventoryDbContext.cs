using Microsoft.EntityFrameworkCore;

namespace AghanimsInventoryApi.Data;

public class AghanimsInventoryDbContext : DbContext
{
    public AghanimsInventoryDbContext(DbContextOptions<AghanimsInventoryDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}