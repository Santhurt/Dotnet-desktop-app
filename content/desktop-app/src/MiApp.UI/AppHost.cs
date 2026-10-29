using MiApp.Application;
using MiApp.Infrastructure;
using MiApp.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MiApp.UI;

/// <summary>
/// Raíz de composición: el único sitio donde se conocen todas las capas.
/// Usa el Generic Host de .NET para configuración (appsettings.json), logging e inyección de dependencias.
/// </summary>
public static class AppHost
{
    public static IHost Create(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Args = args,
            // appsettings.json se busca junto al ejecutable, no en el directorio actual.
            ContentRootPath = AppContext.BaseDirectory,
        });

        builder.Services
            .AddApplication()
            .AddInfrastructure(builder.Configuration)
            .AddPresentation();

        return builder.Build();
    }

    private static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddSingleton<IPageFactory, ScopedPageFactory>();
        services.AddSingleton<MainWindowViewModel>();

        // Las páginas se crean dentro del ámbito de navegación (ver ScopedPageFactory).
        services.AddTransient<HomeViewModel>();
        services.AddTransient<TodosViewModel>();
        return services;
    }
}
