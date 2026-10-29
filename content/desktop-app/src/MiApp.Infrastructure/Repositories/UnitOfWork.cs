using MiApp.Core.Interfaces;
using MiApp.Infrastructure.Data;

namespace MiApp.Infrastructure.Repositories;

public sealed class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);

    // Deja de seguir todas las entidades: la siguiente consulta vuelve a leer el estado real de la BD.
    public void DiscardChanges() => db.ChangeTracker.Clear();
}
