namespace MiApp.Core.Interfaces;

/// <summary>Confirma o descarta en bloque los cambios hechos a través de los repositorios.</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>Olvida los cambios pendientes (por ejemplo, tras un error a mitad de un caso de uso).</summary>
    void DiscardChanges();
}
