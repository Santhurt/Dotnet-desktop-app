using MiApp.Core.Interfaces;
using MiApp.Infrastructure.Data;
using MiApp.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MiApp.Infrastructure;

public static class DependencyInjection
{
    public const string ConnectionStringName = "Default";
    private const string DataFolderName = "MiApp";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlite(ResolveConnectionString(configuration)));

        services.AddScoped<ITodoItemRepository, TodoItemRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<DbInitializer>();

        return services;
    }

    /// <summary>Crea o actualiza la base de datos (migraciones) y siembra los datos iniciales.</summary>
    public static async Task InitializeDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<DbInitializer>().InitializeAsync(cancellationToken);
    }

    /// <summary>
    /// Si no hay cadena de conexión configurada, la BD se guarda en la carpeta de datos del usuario
    /// (~/.local/share en Linux, %LOCALAPPDATA% en Windows).
    /// </summary>
    private static string ResolveConnectionString(IConfiguration configuration)
    {
        var configured = configuration.GetConnectionString(ConnectionStringName);
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured;
        }

        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DataFolderName);
        Directory.CreateDirectory(folder);
        return $"Data Source={Path.Combine(folder, "datos.db")}";
    }
}
