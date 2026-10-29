using System.Globalization;
using Avalonia;
using MiApp.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MiApp.UI;

internal static class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        using var host = AppHost.Create(args);
        ApplyCulture(host.Services.GetRequiredService<IConfiguration>()["Culture"]);

        // Todavía no hay contexto de sincronización de la UI: esperar aquí de forma síncrona es seguro.
        host.Services.InitializeDatabaseAsync().GetAwaiter().GetResult();

        return BuildAvaloniaApp(() => new App(host.Services)).StartWithClassicDesktopLifetime(args);
    }

    /// <summary>Punto de entrada que usa el diseñador/previsualizador de Avalonia.</summary>
    public static AppBuilder BuildAvaloniaApp() => BuildAvaloniaApp(() => new App());

    private static AppBuilder BuildAvaloniaApp(Func<App> appFactory) =>
        AppBuilder.Configure(appFactory)
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    private static void ApplyCulture(string? name)
    {
        var culture = CultureInfo.GetCultureInfo(string.IsNullOrWhiteSpace(name) ? "es-ES" : name);
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = culture;
    }
}
