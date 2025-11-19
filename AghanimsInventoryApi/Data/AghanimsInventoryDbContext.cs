using AghanimsInventoryApi.Data.Entities;
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

        modelBuilder.ApplyConfiguration(new AttackTypeMap());
        modelBuilder.ApplyConfiguration(new AttributeMap());
        modelBuilder.ApplyConfiguration(new HeroAttributeMap());
        modelBuilder.ApplyConfiguration(new HeroMap());
        modelBuilder.ApplyConfiguration(new HeroRoleMap());
        modelBuilder.ApplyConfiguration(new HeroStatMap());
        modelBuilder.ApplyConfiguration(new RoleMap());
        modelBuilder.ApplyConfiguration(new StatMap());
        modelBuilder.ApplyConfiguration(new StatTypeMap());
        modelBuilder.ApplyConfiguration(new RarityMap());

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<AttackType> AttackTypes { get; set; }

    public DbSet<Entities.Attribute> Attributes { get; set; }

    public DbSet<HeroAttribute> HeroAttributes { get; set; }

    public DbSet<Hero> Heroes { get; set; }

    public DbSet<HeroRole> HeroRoles { get; set; }

    public DbSet<HeroStat> HeroStats { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Stat> Stats { get; set; }

    public DbSet<StatType> StatTypes { get; set; }

    public DbSet<Rarity> Rarities{ get; set; }
}