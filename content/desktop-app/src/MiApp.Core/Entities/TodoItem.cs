using MiApp.Core.Errors;
using MiApp.Core.Results;
using MiApp.Core.ValueObjects;

namespace MiApp.Core.Entities;

/// <summary>
/// Entidad de ejemplo. Muestra el patrón a seguir: constructor privado, método de fábrica
/// que valida las reglas y setters privados para que el estado nunca quede inválido.
/// Las reglas rotas se devuelven como <see cref="Result"/>, no como excepciones.
/// </summary>
public sealed class TodoItem : Entity<TodoItemId>
{
    public const int TitleMaxLength = 120;

    private TodoItem(string title) => Title = title;

    public string Title { get; private set; }
    public bool IsDone { get; private set; }

    public static Result<TodoItem> Create(string title) =>
        ValidateTitle(title).Map(valid => new TodoItem(valid));

    public Result Rename(string title)
    {
        var validated = ValidateTitle(title);
        if (validated.IsFailure)
        {
            return validated.Error;
        }

        Title = validated.Value;
        return Result.Success();
    }

    public void MarkAsDone() => IsDone = true;

    public void Reopen() => IsDone = false;

    private static Result<string> ValidateTitle(string title)
    {
        var trimmed = title?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            return TodoItemErrors.TitleRequired;
        }

        if (trimmed.Length > TitleMaxLength)
        {
            return TodoItemErrors.TitleTooLong;
        }

        return trimmed;
    }
}
