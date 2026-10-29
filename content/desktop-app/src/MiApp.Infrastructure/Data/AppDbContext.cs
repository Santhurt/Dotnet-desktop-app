using MiApp.Infrastructure.Configuration;

namespace MiApp.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    // Cada entidad se configura en su propia clase IEntityTypeConfiguration (carpeta Configuration).
    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    // Convenciones globales: cualquier propiedad de estos tipos usa siempre el mismo conversor.
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<TodoItemId>().HaveConversion<StronglyTypedIdConverter<TodoItemId>>();
    }
}
