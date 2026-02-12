using Microsoft.EntityFrameworkCore;

using var db = new DemoDbContext();
await db.Database.MigrateAsync();

class DemoDbContext : DbContext
{
    public DbSet<DemoEntity> Entities { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<DemoEntity>().ComplexProperty(e => e.Value);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite("Data Source=demo.sqlite");
    }
}

class DemoEntity
{
    public int Id { get; set; }
    public DemoValueObject Value { get; set; }
}

struct DemoValueObject
{
    public int Value { get; set; }
}
