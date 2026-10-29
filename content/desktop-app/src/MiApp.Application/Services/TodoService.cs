using MiApp.Application.Interfaces;
using MiApp.Core.Errors;
using MiApp.Core.Interfaces;

namespace MiApp.Application.Services;

public sealed class TodoService(ITodoItemRepository repository, IUnitOfWork unitOfWork)
    : TransactionalService(unitOfWork), ITodoService
{
    public async Task<IReadOnlyList<TodoItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await repository.GetAllAsync(cancellationToken);
        return [.. items.OrderBy(i => i.IsDone).ThenBy(i => i.Title, StringComparer.CurrentCultureIgnoreCase).Select(i => i.ToDto())];
    }

    public Task<Result<TodoItemDto>> CreateAsync(string title, CancellationToken cancellationToken = default) =>
        ExecuteAsync(() =>
        {
            var created = TodoItem.Create(title);
            if (created.IsFailure)
            {
                return Result.Failure<TodoItemDto>(created.Error);
            }

            repository.Add(created.Value);
            return Result.Success(created.Value.ToDto());
        }, cancellationToken);

    public Task<Result<TodoItemDto>> SetDoneAsync(Guid id, bool isDone, CancellationToken cancellationToken = default) =>
        ExecuteAsync(async () =>
        {
            var item = await repository.GetByIdAsync(TodoItemId.From(id), cancellationToken);
            if (item is null)
            {
                return Result.Failure<TodoItemDto>(TodoItemErrors.NotFound(id));
            }

            if (isDone)
            {
                item.MarkAsDone();
            }
            else
            {
                item.Reopen();
            }

            return Result.Success(item.ToDto());
        }, cancellationToken);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        ExecuteAsync(async () =>
        {
            var item = await repository.GetByIdAsync(TodoItemId.From(id), cancellationToken);
            if (item is null)
            {
                return TodoItemErrors.NotFound(id);
            }

            repository.Remove(item);
            return Result.Success();
        }, cancellationToken);
}
