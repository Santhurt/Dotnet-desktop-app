using MiApp.Core.Interfaces;

namespace MiApp.Application.Services;

/// <summary>
/// Base para servicios que modifican datos: ejecuta el caso de uso y
/// - si devuelve éxito, guarda los cambios;
/// - si devuelve un fallo o lanza una excepción, descarta los cambios pendientes para no dejar el estado a medias.
/// </summary>
public abstract class TransactionalService(IUnitOfWork unitOfWork)
{
    protected async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> work, CancellationToken cancellationToken)
        where TResult : Result
    {
        ArgumentNullException.ThrowIfNull(work);
        try
        {
            var result = await work();
            if (result.IsSuccess)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            else
            {
                unitOfWork.DiscardChanges();
            }

            return result;
        }
        catch
        {
            unitOfWork.DiscardChanges();
            throw;
        }
    }

    /// <summary>Variante para casos de uso que no necesitan esperar nada antes de devolver el resultado.</summary>
    protected Task<TResult> ExecuteAsync<TResult>(Func<TResult> work, CancellationToken cancellationToken)
        where TResult : Result
    {
        ArgumentNullException.ThrowIfNull(work);
        return ExecuteAsync(() => Task.FromResult(work()), cancellationToken);
    }
}
