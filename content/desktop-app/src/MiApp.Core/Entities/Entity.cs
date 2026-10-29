using MiApp.Core.ValueObjects;

namespace MiApp.Core.Entities;

/// <summary>Entidad base: dos entidades son iguales si tienen el mismo identificador.</summary>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : struct, IEntityId<TId>
{
    public TId Id { get; protected init; } = TId.New();

    public bool Equals(Entity<TId>? other) =>
        other is not null && other.GetType() == GetType() && other.Id.Equals(Id);

    public override bool Equals(object? obj) => Equals(obj as Entity<TId>);
    public override int GetHashCode() => Id.GetHashCode();
}
