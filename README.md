Demo project for https://github.com/dotnet/efcore/issues/37683

### Bug description
Initial migrations created with EF Core 9.0.13. After updating to 10.0 with no other code changes, EF Core incorrectly detects a model change for (struct-type) ComplexProperty columns.

Demo db context:
```cs
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
```

Initial migration (with Microsoft.EntityFrameworkCore.Sqlite & Microsoft.EntityFrameworkCore.Design 9.0.13 and `dotnet ef migrations add EfCore09`):
```cs
public partial class EfCore09 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value_Value = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entities", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entities");
        }
    }
```

After updating to Microsoft.EntityFrameworkCore.Sqlite & Microsoft.EntityFrameworkCore.Design 10.0.5, `MigrateAsync` fails with `PendingModelChangesWarning`, see stack trace below.

EF Core seems to detect model changes for the "Value_Value" column, even though there aren't any.

Running the migration tool again (`dotnet ef migrations add EfCore10`) adds the following:
```cs
public partial class EfCore10 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "Value_Value",
            table: "Entities",
            type: "INTEGER",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "INTEGER",
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "Value_Value",
            table: "Entities",
            type: "INTEGER",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "INTEGER");
    }
}
```

Detected in production with Npgsql.EntityFrameworkCore.PostgreSQL. The demo shows the same error occurring with Microsoft.EntityFrameworkCore.Sqlite.
