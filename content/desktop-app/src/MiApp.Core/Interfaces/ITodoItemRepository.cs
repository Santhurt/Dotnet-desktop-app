using MiApp.Core.Entities;
using MiApp.Core.ValueObjects;

namespace MiApp.Core.Interfaces;

/// <summary>El dominio define el contrato; Infrastructure lo implementa con EF Core.</summary>
public interface ITodoItemRepository
{
    Task<TodoItem?> GetByIdAsync(TodoItemId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(TodoItem item);
    void Remove(TodoItem item);
}
