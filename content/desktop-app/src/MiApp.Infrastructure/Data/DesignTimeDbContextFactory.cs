using Microsoft.EntityFrameworkCore.Design;

namespace MiApp.Infrastructure.Data;

/// <summary>
/// Lo usa "dotnet ef" para crear el contexto sin arrancar la app.
/// La BD indicada aquí no se toca al generar migraciones; solo se usaría con "dotnet ef database update".
/// </summary>
internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=design.db")
            .Options;
        return new AppDbContext(options);
    }
}
