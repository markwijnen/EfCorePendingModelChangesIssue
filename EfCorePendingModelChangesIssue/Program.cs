using Microsoft.EntityFrameworkCore;

/*
Add efcore 9.0.13 and create initial migration:
    dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 9.0.13
    dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.13
    dotnet ef migrations add EfCore09

Application runs without exceptions.

Update to efcore 10.0.5 and run project:
    dotnet package update
    dotnet run

Application throws:
    Unhandled exception. System.InvalidOperationException: An error was generated for warning 'Microsoft.EntityFrameworkCore.Migrations.PendingModelChangesWarning': The model for context 'DemoDbContext' has pending changes. Add a new migration before updating the database. See https://aka.ms/efcore-docs-pending-changes. This exception can be suppressed or logged by passing event ID 'RelationalEventId.PendingModelChangesWarning' to the 'ConfigureWarnings' method in 'DbContext.OnConfiguring' or 'AddDbContext'.
       at Microsoft.EntityFrameworkCore.Diagnostics.EventDefinition`1.Log[TLoggerCategory](IDiagnosticsLogger`1 logger, TParam arg)
       at Microsoft.EntityFrameworkCore.Diagnostics.RelationalLoggerExtensions.PendingModelChangesWarning(IDiagnosticsLogger`1 diagnostics, Type contextType)
       at Microsoft.EntityFrameworkCore.Migrations.Internal.Migrator.ValidateMigrations(Boolean useTransaction, String targetMigration)
       at Microsoft.EntityFrameworkCore.Migrations.Internal.Migrator.MigrateAsync(String targetMigration, CancellationToken cancellationToken)
       at Program.<Main>$(String[] args) in D:\scratch\dotnet\EfCorePendingModelChangesIssue\EfCorePendingModelChangesIssue\Program.cs:line 4
       at Program.<Main>(String[] args)
 */

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
