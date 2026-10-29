using Microsoft.Extensions.Logging;

namespace MiApp.Infrastructure.Data;

/// <summary>
/// Crea o actualiza el esquema con las migraciones pendientes y, si la BD está vacía,
/// la rellena con datos de ejemplo.
/// </summary>
public sealed class DbInitializer(AppDbContext db, ILogger<DbInitializer> logger)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await db.Database.MigrateAsync(cancellationToken);

        if (await db.TodoItems.AnyAsync(cancellationToken))
        {
            return;
        }

        logger.LogInformation("Base de datos vacía: generando datos de ejemplo");
        // Datos fijos y válidos: si Create fallara, sería un bug y .Value lanzaría.
        var done = TodoItem.Create("Generar el proyecto desde la plantilla").Value;
        done.MarkAsDone();
        db.TodoItems.AddRange(
            done,
            TodoItem.Create("Sustituir TodoItem por las entidades del proyecto").Value,
            TodoItem.Create("Crear una migración: dotnet ef migrations add ...").Value);
        await db.SaveChangesAsync(cancellationToken);
    }
}
