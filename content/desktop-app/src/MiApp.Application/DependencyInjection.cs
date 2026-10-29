using MiApp.Application.Interfaces;
using MiApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MiApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // TimeProvider (.NET 8+) permite controlar "la fecha de hoy" en los tests.
        services.TryAddSingleton(TimeProvider.System);

        // Scoped: cada pantalla de la UI trabaja con su propio ámbito (y su propio DbContext).
        services.AddScoped<ITodoService, TodoService>();

        return services;
    }
}
