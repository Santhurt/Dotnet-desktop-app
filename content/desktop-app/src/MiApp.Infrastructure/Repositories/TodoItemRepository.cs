using MiApp.Core.Interfaces;
using MiApp.Infrastructure.Data;

namespace MiApp.Infrastructure.Repositories;

public sealed class TodoItemRepository(AppDbContext db) : ITodoItemRepository
{
    public Task<TodoItem?> GetByIdAsync(TodoItemId id, CancellationToken cancellationToken = default) =>
        db.TodoItems.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IReadOnlyList<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.TodoItems.ToListAsync(cancellationToken);

    public void Add(TodoItem item) => db.TodoItems.Add(item);

    public void Remove(TodoItem item) => db.TodoItems.Remove(item);
}
