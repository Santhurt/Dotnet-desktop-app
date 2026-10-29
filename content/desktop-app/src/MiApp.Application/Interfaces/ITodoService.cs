namespace MiApp.Application.Interfaces;

/// <summary>
/// Casos de uso de las tareas. Es lo único que conocen los ViewModels.
/// Las operaciones que pueden fallar por una regla de negocio devuelven <see cref="Result"/>.
/// </summary>
public interface ITodoService
{
    Task<IReadOnlyList<TodoItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<TodoItemDto>> CreateAsync(string title, CancellationToken cancellationToken = default);
    Task<Result<TodoItemDto>> SetDoneAsync(Guid id, bool isDone, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
