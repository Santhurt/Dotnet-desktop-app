namespace MiApp.Application.DTOs;

/// <summary>Lo que la UI ve de una tarea: datos planos, sin comportamiento ni dependencias de EF.</summary>
public sealed record TodoItemDto(Guid Id, string Title, bool IsDone);

public static class TodoItemMappings
{
    public static TodoItemDto ToDto(this TodoItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return new TodoItemDto(item.Id.Value, item.Title, item.IsDone);
    }
}
