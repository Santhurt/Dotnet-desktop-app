using MiApp.Core.Entities;
using MiApp.Core.Results;

namespace MiApp.Core.Errors;

/// <summary>Catálogo de errores de las tareas: un único sitio para códigos y mensajes.</summary>
public static class TodoItemErrors
{
    public static readonly Error TitleRequired =
        Error.Validation("TodoItem.TitleRequired", "La tarea debe tener un título.");

    public static readonly Error TitleTooLong =
        Error.Validation("TodoItem.TitleTooLong", $"El título no puede superar {TodoItem.TitleMaxLength} caracteres.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("TodoItem.NotFound", $"No se encontró la tarea con id '{id}'.");
}
