using AghanimsInventoryApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ApiTests.Settings;

public class TestDbContext : AghanimsInventoryDbContext
{
    public TestDbContext(DbContextOptions<AghanimsInventoryDbContext> options) : base(options) { }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetRowVersions();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        SetRowVersions();

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void SetRowVersions()
    {
        IEnumerable<EntityEntry> addedEntities = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added && e.Metadata.FindProperty("Version") != null);

        foreach (var entry in addedEntities)
        {
            entry.Property("Version").CurrentValue = Guid.NewGuid().ToByteArray();
        }
    }
}
