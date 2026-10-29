using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MiApp.Infrastructure.Configuration;

/// <summary>
/// Un único conversor genérico para todos los IDs fuertemente tipados.
/// Usa el miembro estático abstracto <see cref="IEntityId{TSelf}.From"/> a través de un método auxiliar,
/// porque los árboles de expresión no pueden llamar directamente a miembros estáticos de interfaz.
/// </summary>
public sealed class StronglyTypedIdConverter<TId>() : ValueConverter<TId, Guid>(id => id.Value, value => Create(value))
    where TId : struct, IEntityId<TId>
{
    private static TId Create(Guid value) => TId.From(value);
}
