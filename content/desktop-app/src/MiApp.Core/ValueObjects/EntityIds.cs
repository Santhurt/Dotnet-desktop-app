namespace MiApp.Core.ValueObjects;

/// <summary>
/// Contrato para identificadores fuertemente tipados. El miembro estático abstracto
/// permite crear IDs de forma genérica (por ejemplo, en los conversores de EF Core).
/// </summary>
public interface IEntityId<TSelf> where TSelf : struct, IEntityId<TSelf>
{
    Guid Value { get; }
    static abstract TSelf From(Guid value);
    static abstract TSelf New();
}

// Guid v7 (.NET 9): ordenable por fecha de creación, ideal como clave primaria.
// Para cada entidad nueva, copia este record cambiando el nombre.
public readonly record struct TodoItemId(Guid Value) : IEntityId<TodoItemId>
{
    public static TodoItemId From(Guid value) => new(value);
    public static TodoItemId New() => new(Guid.CreateVersion7());
    public override string ToString() => Value.ToString();
}
