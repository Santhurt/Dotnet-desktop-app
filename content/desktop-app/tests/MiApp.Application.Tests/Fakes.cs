namespace MiApp.Application.Tests;

/// <summary>Repositorio en memoria: más legible que un mock para un agregado con estado.</summary>
public sealed class InMemoryTodoItemRepository : ITodoItemRepository
{
    public List<TodoItem> Items { get; } = [];

    public Task<TodoItem?> GetByIdAsync(TodoItemId id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Items.Find(i => i.Id == id));

    public Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<TodoItem>>(Items);

    public void Add(TodoItem item) => Items.Add(item);

    public void Remove(TodoItem item) => Items.Remove(item);
}
